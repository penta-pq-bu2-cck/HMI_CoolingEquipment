# HMI Cooling Equipment — Bug Investigation & Resolution Report

> **Confidential — Internal Use Only**
> Prepared: 2025

---

## Table of Contents

- [Issue 1 — Stale PreAssignLoad Value Persisted in Database](#issue-1--stale-preassignload-value-persisted-in-database)
- [Issue 2 — HMI Auto-Crash / Freeze](#issue-2--hmi-auto-crash--freeze)
- [Issue 3 — Carrier List Shows Empty for Existing Job](#issue-3--carrier-list-shows-empty-for-existing-job)
- [Summary — All Issues & Action Items](#summary--all-issues--action-items)

---

# Issue 1 — Stale PreAssignLoad Value Persisted in Database

## 1.1 Issue Summary

| Field | Detail |
|---|---|
| **Severity** | High — Causes load ports to become permanently unavailable |
| **Component** | `LOADPORT` table / `PTLService` / `EF_Service` |
| **Symptom** | `LoadPortState = Empty` but `PreAssignLoad` is non-null in DB. The affected slot is invisible to `PreAssignTrayToLoadPort` (filtered out), so no new carrier can be assigned to it. |
| **Frequency** | Accumulates over days/weeks of production. Typically 1–5 slots per week depending on network stability. |

---

## 1.2 Background — PreAssignLoad Lifecycle

The column `LOADPORT.PreAssignLoad` stores the carrier ID pre-assigned to a physical slot before the carrier is physically loaded. The expected lifecycle is:

1. **ASSIGN** — `PreAssignTrayToLoadPort` sets `PreAssignLoad = carrierID` (in-memory, then flushed to DB)
2. **CONFIRM** — When carrier is physically placed, `UpdateLoadPortState_LoadProcess` clears `PreAssignLoad = null` and moves value to `LoadPortCarrierLoad`
3. **ABORT** — `EF_AbortJob` sets `PreAssignLoad = null` directly in DB

`PreAssignTrayToLoadPort` filters candidate slots using:
```
LoadPortState == Empty && string.IsNullOrEmpty(PreAssignLoad)
```
Any slot with a non-null `PreAssignLoad` is permanently skipped.

---

## 1.3 Root Cause — GatewayStatusChecking Race Condition

The primary root cause is a race condition between `EF_AbortJob` and the `GatewayStatusChecking` background thread in `PTLService`.

> **Root Cause:** `GatewayStatusChecking` runs every 10ms on a dedicated background thread. Whenever a PTL gateway status changes (connected/disconnected), it executes a full bulk flush of ALL `LoadPortModels` to DB — including the `PreAssignLoad` column. If this flush fires within the ~2–8ms window between `EF_AbortJob`'s `SaveChanges()` and the completion of `EF_UpdateMemory(LOADPORT)`, it overwrites the null that abort just wrote with the stale non-null value from `PTLService.LoadPortModels`.

### Race Condition Timeline

```
UI Thread:    EF_AbortJob -> SaveChanges()                [DB: PreAssignLoad = null]  ← T=0
              EF_UpdateMemory(LOADPORT) -> SQL SELECT...   [~2-8ms SQL round trip]
                                                           [PTLService NOT yet updated]

BG Thread:    GatewayStatusChecking detects statusCode     [T = ~10ms]
              change (network blip on factory floor)
              -> EF_UpdateLoadPort(LoadPortMap(            [PTLService.LoadPortModels
                 PTLService.LoadPortModels))                still has PreAssignLoad='C001']
              -> WRITES 'C001' BACK TO DB                  [Race won — value latched]

UI Thread:    SQL completes -> PTLService updated to null  [Too late — DB already dirty]
```

---

## 1.4 Secondary Root Cause — EF_PurgeJob Missing Null Clear

**File:** `EF_Service.cs` line ~490

`EF_PurgeJob` correctly sets `LoadPortState = Empty` on each matching slot, but never sets `PreAssignLoad = null`. After a Purge, all slots have `Empty + PreAssignLoad` set. This self-heals only if the operator subsequently clicks Abort (which does clear `PreAssignLoad`). If the operator does not abort, the slots are permanently orphaned.

---

## 1.5 Observations

- **DB observation:** `LoadPortState = Empty`, `PreAssignLoad = non-null`, no matching `JOBDETAILS` or `SETUPJOB` rows.
- **Self-healing cases:** Normal skip/rescan cycle, crash then rescan, abort after purge — all self-heal correctly via existing code paths.
- **Non-healing case:** Race condition during abort — job is deleted from DB but `PreAssignLoad` survives the bulk overwrite from `GatewayStatusChecking`.
- **Frequency driver:** Factory floor network instability. Each gateway status transition is a race trigger. More transitions = more orphaned slots.
- **Sensor bit:** `IsLoaded` is runtime-only from PTL hardware IO events — not stored in DB. Available only during PTL `isInit` events (first connect/reconnect) via `HandleLoadPortDataChange`.

---

## 1.6 Fix — Part A: Self-Healing in HandleLoadPortDataChange (PTLService.cs)

During PTL startup/reconnect, `isInit=true` events carry the current physical sensor state (`IsLoaded`) for every slot. This is the only moment all conditions can be checked simultaneously. Inject the self-heal check at the top of `HandleLoadPortDataChange` before the switch statement.

**File:** `Communication/PTL/PTLService.cs — HandleLoadPortDataChange method`

**Conditions required before clearing:**

1. `isInit == true` — PTL startup/reconnect, sensor state is reliable
2. `IsLoaded == false` — physical sensor confirms slot is empty
3. `model.LoadPortState == Empty` — DB state agrees slot is empty
4. `model.PreAssignLoad` is not null/empty — the symptom exists
5. No active `JOBDETAILS` row for this carrier ID — job is genuinely gone

### New code — add at top of HandleLoadPortDataChange (before existing switch)

```csharp
// ── STARTUP SELF-HEAL ──────────────────────────────────────────
if (isInit
    && !IsLoaded
    && model.LoadPortState == LoadPortState.Empty
    && !string.IsNullOrEmpty(model.PreAssignLoad))
{
    bool hasActiveJob =
        EF_Service.EF_CheckCarrierHasActiveJob(model.PreAssignLoad);

    if (!hasActiveJob)
    {
        LogHelper.General(4, $"[StartupSelfHeal] ORPHAN — " +
            $"Slot={model.LoadPortID} | " +
            $"StalePreAssignLoad=[{model.PreAssignLoad}] | Clearing.");

        EF_Service.EF_InsertEventLog("WARNING", "STARTUP", "", "",
            model.PreAssignLoad, model.LoadPortID, "SYSTEM",
            $"Startup self-heal: orphaned PreAssignLoad " +
            $"[{model.PreAssignLoad}] cleared from [{model.LoadPortID}]");

        model.PreAssignLoad = null;
        LoadPort_VM.LoadPortModelData(model);  // writes null to DB + in-memory
    }
    else
    {
        LogHelper.General(3, $"[StartupSelfHeal] " +
            $"Slot {model.LoadPortID} PreAssignLoad=[{model.PreAssignLoad}] " +
            $"— active job exists, skipping.");
    }
}
// ── END STARTUP SELF-HEAL ────────────────────────────────────────
```

### New helper method — add to EF_Service.cs

```csharp
public static bool EF_CheckCarrierHasActiveJob(string carrierID)
{
    try
    {
        using (var context = new DB_Context())
        {
            // JOBDETAILS only holds active carriers (Initialize/Loading/Cooling).
            // Completed carriers are deleted from JOBDETAILS and moved to
            // JOBDETAILSHISTORY by EF_InsertSetupJobHistory — CarrierStatus
            // = Complete will NEVER exist in this table.
            // Simple existence check is sufficient and correct.
            return context.JOBDETAILS.Any(x => x.CarrierID == carrierID);
        }
    }
    catch (Exception ex)
    {
        LogHelper.EF(5, $"[EF_CheckCarrierHasActiveJob] Error: {ex.Message}");
        return true;  // fail-safe: if DB error, assume job exists, do NOT clear
    }
}
```

---

## 1.7 Fix — Part B: Self-Healing After Unloading (PTLService.cs)

During unloading, `UpdateLoadPortState_UnloadProcess` sets `LoadPortState = Empty` and `LoadPortCarrierLoad = null`. At this point `PreAssignLoad` should already be null (cleared during loading). As a defensive measure, add an explicit null clear to ensure any residual stale value is always removed when the slot becomes physically empty via unload.

**File:** `Communication/PTL/PTLService.cs — UpdateLoadPortState_UnloadProcess method`

### Add one line before LoadPort_VM.LoadPortModelData(model)

```csharp
// Defensive clear — PreAssignLoad should already be null at this
// stage, but explicitly null it to prevent any residual stale value
// being flushed back to DB by the subsequent LoadPortModelData call.
model.PreAssignLoad = null;   // ← ADD THIS LINE
model.LoadPortCarrierLoad = null;

LoadPort_VM.LoadPortModelData(model);
```

---

## 1.8 Fix — Part C: EF_PurgeJob Missing Null Clear (EF_Service.cs)

**File:** `EF_Service.cs — EF_PurgeJob method, line ~490`

```csharp
// BEFORE:
port.LoadPortState = LoadPortState.Empty;
context.LOADPORT.Update(port);

// AFTER:
port.LoadPortState = LoadPortState.Empty;
port.PreAssignLoad = null;    // ← ADD THIS LINE
context.LOADPORT.Update(port);
```

---

## 1.9 Files Changed

| File | Method / Location | Change Type |
|---|---|---|
| `Communication/PTL/PTLService.cs` | `HandleLoadPortDataChange` | Add startup self-heal block |
| `Communication/PTL/PTLService.cs` | `UpdateLoadPortState_UnloadProcess` | Add defensive `PreAssignLoad = null` |
| `Communication/EntityFramework/EF_Service.cs` | `EF_PurgeJob` ~line 490 | Add `port.PreAssignLoad = null` |
| `Communication/EntityFramework/EF_Service.cs` | `EF_CheckCarrierHasActiveJob` | New helper method |

---

# Issue 2 — HMI Auto-Crash / Freeze

## 2.1 Issue Summary

| Field | Detail |
|---|---|
| **Severity** | High — Application terminates unexpectedly during production |
| **Component** | `App.xaml.cs` / `EF_Service.cs` / `PTLService.cs` |
| **Symptom** | HMI process closes without user action. Sometimes preceded by a freeze/hang lasting several seconds. Frequency increases after weeks of production. |

---

## 2.2 Root Causes

### Root Cause A — No Global Exception Handler

**File:** `App.xaml.cs`

The application startup registers no global exception handlers. Any unhandled exception on any thread — SQL timeout, DB connection drop, null reference, or Entity Framework error — propagates to the CLR and terminates the process immediately without logging.

Three handler types are missing:

- `DispatcherUnhandledException` — catches UI thread exceptions
- `AppDomain.CurrentDomain.UnhandledException` — catches background thread exceptions
- `TaskScheduler.UnobservedTaskException` — catches unobserved Task/async void exceptions

**19 `async void` methods** exist across the codebase. Exceptions thrown inside `async void` cannot be caught by callers — they surface as `UnobservedTaskExceptions`, which without a handler terminate the process.

### Root Cause B — Unbounded Table Growth Causing RAM Exhaustion

Three database tables grow indefinitely with no purge mechanism. `EF_UpdateMemory` loads these fully into RAM on every ALL refresh:

| Table | Query Pattern | Risk |
|---|---|---|
| `SETUPJOBHISTORY` | `Include(JobDetails).ToList()` — full JOIN, no filter | **Highest** — grows with every completed job |
| `ALARMHISTORY` | `ToList().Where(today)` — loads ALL rows then filters in memory | **High** — loads full table even for date filter |
| `EVENTLOG` | Written only, never purged — grows with every operation | **Medium** — not loaded but disk usage grows |

---

## 2.3 Observations

- Freeze is most pronounced when PentaIcon or Reset Button is clicked — both trigger `EF_UpdateMemory(ALL)` which executes 9 sequential SQL SELECT queries synchronously.
- Crash frequency increases after weeks of production as `SETUPJOBHISTORY` and `ALARMHISTORY` grow larger, making each `ToList()` heavier.
- No crash log exists because there is no exception handler to write one — the process simply disappears.
- RAM consumption can be measured via Task Manager during a PentaIcon refresh. After months of data, `SETUPJOBHISTORY.Include(JobDetails).ToList()` can load tens of thousands of rows.

---

## 2.4 Fix — Part A: Add Global Exception Handlers (App.xaml.cs)

**File:** `App.xaml.cs — Application_Startup method`

```csharp
private void Application_Startup(object sender, StartupEventArgs e)
{
    // Register global handlers BEFORE anything else
    DispatcherUnhandledException +=
        App_DispatcherUnhandledException;
    AppDomain.CurrentDomain.UnhandledException +=
        CurrentDomain_UnhandledException;
    TaskScheduler.UnobservedTaskException +=
        TaskScheduler_UnobservedTaskException;

    // ... existing duplicate-process check and Frame startup ...
}

// Catches exceptions on UI thread — app continues running
private void App_DispatcherUnhandledException(object sender,
    DispatcherUnhandledExceptionEventArgs e)
{
    LogHelper.General(5,
        $"[CRASH PREVENTED] UI Thread: {e.Exception}");
    MessageBox.Show(
        $"An unexpected error occurred but the application will continue.\n" +
        $"Error: {e.Exception.Message}",
        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    e.Handled = true;  // prevents crash
}

// Catches background thread exceptions — logs before process dies
private void CurrentDomain_UnhandledException(object sender,
    UnhandledExceptionEventArgs e)
{
    Exception ex = e.ExceptionObject as Exception;
    LogHelper.General(5,
        $"[CRASH] Background Thread: {ex?.ToString()}");
}

// Catches unobserved Task/async void exceptions — app continues
private void TaskScheduler_UnobservedTaskException(object sender,
    UnobservedTaskExceptionEventArgs e)
{
    LogHelper.General(5,
        $"[CRASH PREVENTED] Unobserved Task: {e.Exception}");
    e.SetObserved();  // prevents crash
}
```

---

## 2.5 Fix — Part B: Limit SETUPJOBHISTORY and ALARMHISTORY Queries (EF_Service.cs)

**File:** `Communication/EntityFramework/EF_Service.cs — EF_UpdateMemory method`

### SETUPJOBHISTORY — add date filter and row limit

```csharp
// BEFORE — loads entire history table + all JobDetails (JOIN):
setupjobhistory = context.SETUPJOBHISTORY
    .Include(x => x.JobDetails).ToList();

// AFTER — last 90 days, max 500 rows:
DateTime cutoff = DateTime.Now.AddDays(-90);
setupjobhistory = context.SETUPJOBHISTORY
    .Include(x => x.JobDetails)
    .Where(x => x.CreatedOn >= cutoff)
    .OrderByDescending(x => x.CreatedOn)
    .Take(500)
    .ToList();
```

### ALARMHISTORY — filter in SQL with 90-day range (two locations in switch)

```csharp
// BEFORE — loads ENTIRE table into RAM then filters to today only:
alarmHistory = context.ALARMHISTORY.ToList()
    .Where(x => x.AlarmDateTime.Equals(
        DateTime.Now.ToShortDateString())).ToList();

// AFTER — filter in SQL, last 90 days, consistent with SETUPJOBHISTORY:
DateTime alarmCutoff = DateTime.Now.AddDays(-90);
alarmHistory = context.ALARMHISTORY
    .Where(x => x.AlarmDateTime >= alarmCutoff)
    .OrderByDescending(x => x.AlarmDateTime)
    .ToList();
```

> Apply this fix in **both** the `HMIDBMemory.ALARMHISTORY` case **and** the `HMIDBMemory.ALL` case — two separate locations in `EF_UpdateMemory`. `AlarmDateTime` is a `DateTime` column so the `>=` comparison translates directly to a SQL WHERE clause without loading the full table.

---

## 2.6 Files Changed

| File | Method / Location | Change Type |
|---|---|---|
| `App.xaml.cs` | `Application_Startup` | Add 3 global exception handlers |
| `Communication/EntityFramework/EF_Service.cs` | `EF_UpdateMemory — SETUPJOBHISTORY case` | Add date filter + `Take(500)` |
| `Communication/EntityFramework/EF_Service.cs` | `EF_UpdateMemory — ALARMHISTORY case (x2)` | Move filter into SQL query, 90-day range |

---

# Issue 3 — Carrier List Shows Empty for Existing Job

## 3.1 Issue Summary

| Field | Detail |
|---|---|
| **Severity** | Medium — Operator cannot see carrier list; job appears incomplete |
| **Component** | `SecsGemService.cs` / `EF_Service.cs` / `GetCarrierListForDisplay` converter |
| **Symptom** | Job ID and Lot ID appear correctly in the job list (LotStatus and Operator screens) but the Carriers List column is blank. Clicking Reset does not fix it. |

---

## 3.2 Root Cause — Non-Atomic Job Creation + Separate DB Contexts

When a SECS/GEM S2F49 STARTJOB command is received, job creation follows this sequence in `SecsGemService.cs`:

```
EF_Service.EF_InsertJobDetails(jobs);    // Step 1: inserts JOBDETAILS rows
                                          //   -> on failure: logs error, CONTINUES
                                          //   -> finally: EF_UpdateMemory(JOBDETAILS)
                                          //      (no one in LotStatus_VM receives this)
EF_Service.EF_InsertSetupJob(setupjob);  // Step 2: inserts SETUPJOB row
                                          //   -> EF_SetDateTimeSetupJob
                                          //      -> EF_UpdateMemory(SETUPJOB)
                                          //         -> LotStatus_VM renders job list
                                          //            -> converter queries JOBDETAILS
                                          //               -> finds nothing if Step 1 failed
```

The two insert operations use separate `DbContext` instances and separate transactions. If `EF_InsertJobDetails` fails (SQL error, connection blip, constraint violation), it catches the exception, logs it, and returns — but `EF_InsertSetupJob` still executes. The UI receives the `SETUPJOB` broadcast, renders the job row, and the `GetCarrierListForDisplay` converter immediately queries `JOBDETAILS` — which is empty.

> **Key Finding:** `LotStatus_VM` does NOT register for `List<JobDetailsModel>` via Messenger. It only receives `List<SetupJobModel>`. The converter queries the DB directly on every render, making it dependent on DB consistency at the exact moment of render.

---

## 3.3 Observations

- Issue persists after Reset because Reset calls `EF_UpdateMemory(ALL)` which re-broadcasts `SETUPJOB` — triggering the converter to query `JOBDETAILS` again. If `JOBDETAILS` rows were never committed, they still won't be there.
- Issue is transient if the DB was temporarily overloaded at job creation time — a retry or restart can sometimes recover if the `JOBDETAILS` rows were partially written.
- Issue is permanent if `EF_InsertJobDetails` failed due to a constraint violation or duplicate key — the rows will never be inserted.
- **Root cause confirmed location:** `SecsGemService.cs lines 671–672` — the two calls are independent, not transactional.

---

## 3.4 Fix — Atomic Job Creation Method

Create a new method `EF_InsertNewJob` in `EF_Service.cs` that inserts both `JOBDETAILS` and `SETUPJOB` in a single `DbContext` transaction. Either both succeed or both fail — no partial state possible. The broadcast order is also corrected: `JOBDETAILS` is broadcast before `SETUPJOB` so the converter has data available when it fires.

### New method — add to EF_Service.cs

```csharp
/// <summary>
/// Atomically inserts JOBDETAILS and SETUPJOB in a single transaction.
/// Broadcasts JOBDETAILS first so converter has data when SETUPJOB
/// triggers the UI render.
/// </summary>
public static bool EF_InsertNewJob(SETUPJOB setupjob, List<JOBDETAILS> jobdetails)
{
    try
    {
        using (var context = new DB_Context())
        {
            // Both tables in ONE transaction
            context.JOBDETAILS.AddRange(jobdetails);
            context.SETUPJOB.Add(setupjob);
            context.SaveChanges();  // atomic — rolls back both if either fails
        }

        EF_SetDateTimeSetupJob(setupjob.JobID,
            setupjob.LotNo, JobStatus.Initialize);
        return true;
    }
    catch (Exception ex)
    {
        LogHelper.EF(5,
            $"[EF_InsertNewJob] Failed: {ex.Message}");
        return false;
    }
    finally
    {
        // Broadcast JOBDETAILS FIRST — data must be in DB before
        // SETUPJOB triggers the UI converter query
        EF_UpdateMemory(HMIDBMemory.JOBDETAILS,
            nameof(EF_InsertNewJob));
        // Then SETUPJOB — this triggers LotStatus_VM render
        EF_UpdateMemory(HMIDBMemory.SETUPJOB,
            nameof(EF_InsertNewJob));
    }
}
```

### Change in SecsGemService.cs — lines 671–672 (the only call site)

```csharp
// BEFORE (lines 671-672):
EF_Service.EF_InsertJobDetails(jobs);
EF_Service.EF_InsertSetupJob(setupjob);

// AFTER — replace the entire else block with:
else
{
    setupjob.NoOfCarrier = jobs.Count();
    LogHelper.General(2, $"Send setup job command, " +
        $"JobID={setupjob.JobID}, LotNo={setupjob.LotNo}");

    if (!EF_Service.EF_InsertNewJob(setupjob, jobs))
    {
        arg.GEMProCommand.HCACK = 3;
        // HCACK=3 means command rejected by equipment.
        // Host will not assume job is running and can retry.
        LogHelper.SecsGem(5,
            $"[{nameof(GEMPro_CommandNotification)}] " +
            $"EF_InsertNewJob failed — " +
            $"JobID={setupjob.JobID} LotNo={setupjob.LotNo}");
        break;
    }

    EF_Service.EF_InsertEventLog("TRACE", "SECSGEM",
        setupjob.JobID, setupjob.LotNo, "", "", "",
        "Received S2F49-SETUPJOB Command");
    arg.GEMProCommand.HCACK = 0;
}
```

---

## 3.5 Files Changed

| File | Method / Location | Change Type |
|---|---|---|
| `Communication/EntityFramework/EF_Service.cs` | `EF_InsertNewJob` (new method) | New atomic insert method |
| `Communication/SecsGem/SecsGemService.cs` | `GEMPro_CommandNotification` lines 671–672 | Replace 2 calls with `EF_InsertNewJob` |

---

# Summary — All Issues & Action Items

| # | Issue | Root Cause | Files to Change | Risk |
|---|---|---|---|---|
| 1a | PreAssignLoad stale in DB | `GatewayStatusChecking` race during abort | `PTLService.cs`, `EF_Service.cs` | Medium — adds check during PTL init |
| 1b | PreAssignLoad not cleared on unload | Defensive gap in `UpdateLoadPortState_UnloadProcess` | `PTLService.cs` | Low — one line add |
| 1c | PreAssignLoad not cleared on purge | `EF_PurgeJob` missing null assignment | `EF_Service.cs` | Low — one line add |
| 2a | HMI crash on unhandled exception | No global exception handlers in `App.xaml.cs` | `App.xaml.cs` | Low — additive only |
| 2b | HMI freeze / RAM growth | Full table loads into RAM, no pagination | `EF_Service.cs` | Low — query filter change |
| 3 | Carrier list empty for existing job | Non-atomic `JOBDETAILS` + `SETUPJOB` insert | `EF_Service.cs`, `SecsGemService.cs` | Medium — changes job creation path |

---

## Testing Checklist

1. **Issue 1** — Simulate race: create job, pre-assign carriers, disconnect PTL network during abort. Verify after restart all slots show `PreAssignLoad = null` in DB.
2. **Issue 1** — Purge a job without abort. Verify slots have `PreAssignLoad = null` after purge.
3. **Issue 1** — Reconnect PTL with a slot that has stale `PreAssignLoad`. Verify log shows `[StartupSelfHeal] ORPHAN` entry and `PreAssignLoad` is cleared.
4. **Issue 2** — Monitor RAM during PentaIcon refresh before and after fix. RAM spike should reduce significantly.
5. **Issue 2** — Simulate SQL Server connection drop mid-operation. Verify HMI shows warning dialog and continues running instead of crashing.
6. **Issue 3** — Send S2F49 STARTJOB from host. Verify carrier list is populated immediately in both LotStatus and Operator screens.
7. **Issue 3** — Simulate DB failure during `EF_InsertJobDetails`. Verify `HCACK=3` is returned to host and no orphan `SETUPJOB` row is created.
