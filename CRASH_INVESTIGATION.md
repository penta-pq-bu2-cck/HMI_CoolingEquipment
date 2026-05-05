# Midnight Crash – Root-Cause Candidates

> **Status:** investigation / triage. The HMI is reported to crash overnight at irregular times (often around midnight). No reliable repro and no obvious "feature" trigger. This document lists every code path I can defend as a plausible crash cause, scored by how strongly the evidence points at it. Use this as the seed for a GitHub issue.

## How to read the score

Each candidate has a **Likelihood × Impact** score, both 1–5, and a combined risk.

| Field | Meaning |
|-------|---------|
| **Likelihood (L)** | How likely *this specific code path* is the cause of an unattended midnight crash, given what's in the repo today. |
| **Impact (I)** | If it triggers, how bad. 5 = unhandled exception terminates the process; 3 = silent corruption / freeze; 1 = log spam. |
| **Score** | `L × I` (max 25). |
| **Confidence** | How sure I am the code path *exists as described*. Independent of L/I. |

A real crash usually scores L ≥ 3 and I ≥ 4. Below the table I list each candidate with file:line evidence.

---

## Summary table (sorted by score)

| # | Candidate | L | I | Score | Confidence |
|---|-----------|:-:|:-:|:-----:|:----------:|
| 1 | **No global unhandled-exception handler** (`App.xaml.cs`) | 5 | 5 | **25** | High |
| 2 | **`EF_CompareDateTime` busy-loop with no `Thread.Sleep`** + `async` lambda inside `List.ForEach` (fire-and-forget; exceptions escape the outer try/catch) | 5 | 5 | **25** | High |
| 3 | **`async void` event handlers everywhere** (PTL / EF / SECS) – any throw becomes an unhandled exception that crashes the AppDomain | 5 | 5 | **25** | High |
| 4 | **NLog daily file rollover at 00:00** (`${shortdate}` in 7 targets, `maxArchiveDays="30"`) – delete + open race with concurrent writers | 4 | 4 | **16** | Medium |
| 5 | **Day-rollover in EF history queries / file naming** (`DateTime.Parse` / `ParseExact` with empty `LD_EXPECTED_END`, `CT_END`, `ST_EXPECTED_END`) – `FormatException` on a worker thread | 4 | 4 | **16** | High |
| 6 | **ModBus `BlinkingLED` timer handler – no try/catch**, 100 ms `HeartBeatTimer` re-entrancy, `_socket.Connect` un-guarded | 4 | 4 | **16** | High |
| 7 | **`new Thread(...)` for unload/load/datetime check with no exception boundary** – any throw propagates to AppDomain | 4 | 5 | **20** | High |
| 8 | **`UnloadThread?.Abort()` / `CheckingDateTimeThread.Abort()`** – `Thread.Abort` injects `ThreadAbortException` at arbitrary points, can corrupt EF context state | 3 | 5 | **15** | High |
| 9 | **EF `DbContext` shared across worker threads + `async` lambda** in `EF_CompareDateTime` – `DbContext` is *not* thread-safe; concurrent `SaveChanges` → `InvalidOperationException` | 4 | 4 | **16** | High |
| 10 | **`while (true);` busy-spin** in `LoadingCarrierEvent` (line 900 / 1191) – pegs CPU; if reached, the dependent SECS-handshake never completes and watchdog timers fire | 2 | 4 | **8** | High |
| 11 | **`.First()` / `.First(x =>)` on possibly empty collections** – `InvalidOperationException` on background threads | 3 | 4 | **12** | High |
| 12 | **SQL Server connection drop at SQL Server scheduled maintenance window** – any active EF query throws `SqlException`; many call sites have no retry and several rethrow (`throw ex;`) | 4 | 4 | **16** | Medium |
| 13 | **NLog `internalLogFile="D:\temp\nlog-internal.log"`** – if D:\temp doesn't exist, NLog silently fails its own logging; doesn't crash directly, but masks evidence of #4 | 2 | 1 | **2** | High |
| 14 | **`InitializeAsyn` calls `ModBus_Service.ConnectToModBus()` without try/catch** – TCP connect to tower light at startup; if power-cycled overnight and HMI auto-restarts, throws | 3 | 4 | **12** | High |
| 15 | **`throw ex;`** (rethrow that resets stack) in `Frame_VM` constructor + many EF methods – not a crash *cause* but destroys evidence in the dump | 3 | 2 | **6** | High |
| 16 | **Memory growth over 24 h+** – ObservableCollections (`UnloadPortLogs`, `LoadPortLogs`, etc.) trimmed to 30, but `EF_InsertEventLog` writes to DB on every state change; if DB is down, every Insert may queue | 2 | 4 | **8** | Medium |
| 17 | **Single-instance check kills `Process.GetCurrentProcess().Kill()`** in `App.xaml.cs:26` – not a crash cause, but if the OS auto-restarts the HMI overnight (Task Scheduler, watchdog) and the previous process hasn't fully exited, the new one suicides; visible as "midnight crash" | 2 | 5 | **10** | High |

---

## Detailed evidence

### #1 — No global unhandled-exception handler

- `App.xaml.cs` registers no `DispatcherUnhandledException`, no `AppDomain.CurrentDomain.UnhandledException`, no `TaskScheduler.UnobservedTaskException`.
- Confirmed by repo-wide grep: only one match for `AppDomain.CurrentDomain` and it's just `FriendlyName`.
- **Effect:** any unhandled exception on a worker thread or `async void` continuation tears down the process. There is *no* logging of the final fatal stack — which matches the "we don't know why it crashed" symptom.
- **Fix sketch:**
  ```csharp
  // App.xaml.cs, before Frame.Show()
  AppDomain.CurrentDomain.UnhandledException += (_, e) =>
      LogHelper.General(6, $"FATAL AppDomain: {e.ExceptionObject}");
  Application.Current.DispatcherUnhandledException += (_, e) =>
      { LogHelper.General(6, $"FATAL Dispatcher: {e.Exception}"); e.Handled = false; };
  TaskScheduler.UnobservedTaskException += (_, e) =>
      { LogHelper.General(6, $"FATAL UnobservedTask: {e.Exception}"); e.SetObserved(); };
  ```

### #2 — `EF_CompareDateTime` busy-loop + async-in-ForEach

`Communication/EntityFramework/EF_Service.cs:1773-1889`

```csharp
public static void EF_CompareDateTime()
{
    while (isCheckingDateTimeEnable)        // ← no Thread.Sleep anywhere
    {
        try { ...
            jobs.ForEach(async x =>          // ← async lambda passed to Action<T>
            {
                ...
                SecsGemService.SendJobTracker(...).Wait(200);
                EF_SentLoadPortReadyToUnloadAsync(x.JobID);
                ...
            });
        } catch (Exception ex) { LogHelper.EF(5, ...); }
    }
}
```

Two compounding bugs:
1. **No sleep / delay** — at 100 % CPU the loop hits `context.SETUPJOB.ToList()` (full table scan) thousands of times per second.
2. **`async x => ...` inside `List<T>.ForEach`** — the lambda is silently treated as `async void`. Exceptions inside it do NOT propagate to the outer `try/catch`; they hit the SynchronizationContext (none on a worker thread) and become unhandled → process termination. Combined with #1, this is the single most likely midnight-crash source.

### #3 — `async void` everywhere

35 matches for `async void` across:
- `Communication/PTL/PTLService.cs`: `OpenPTL`, `GatewayStatusChecking`, `GetTagConnectionStatus`, `GetGatewayConnectionStatus`, `SetDeviceBestPollRange`, `EnableLightLEDTesting`, `LoadPortDataChange`
- `Communication/SecsGem/SecsGemService.cs`: `HandleSRVTerminalDisplay`
- `Communication/EntityFramework/EF_Service.cs`: `EF_InsertAlarmToAlarmCurrentTableAsync`, `EF_UpdateCarrierStatusAsync`, `EF_CarrierCheckFirstLastAsync`, `EF_SentLoadPortReadyToUnloadAsync`
- `ViewModels/Frame/Frame_VM.cs`: `InitializeAsyn`, `ResetButtonCommandEvent`
- `ViewModels/PTL/PTL_VM.cs`, `LoadPort_VM.LoadingCarrierEvent`, `UnloadingLoadPort_VM.ResetErrorLog`, `LoadingLoadPort_VM.ResetErrorLog`

`async void` exceptions are unhandled. This is the pattern that combines with #1 to crash the process.

### #4 — NLog daily rollover

`NLog.config:40-46`

```xml
<target name="general" xsi:type="File"
        fileName="${var:basedirCustom}\HMI_Logs\${shortdate}\General.log"
        ... maxArchiveDays="30" />
<!-- 6 more identical targets: opc / secsgem / tcp / ef / ptl / error -->
<target name="alarm" xsi:type="File"
        archiveAboveSize="10000000" maxArchiveFiles="0"
        archiveFileName="...\Archive\${shortdate}\Alarm.csv"
        fileName="...\${shortdate}\Alarm.csv">
```

At 00:00:
1. `${shortdate}` flips, NLog opens 7 new file paths concurrently from many writer threads.
2. `maxArchiveDays="30"` triggers deletion of the oldest folder. If any file in that folder is still locked (e.g. by a Windows backup agent, antivirus, or because the previous HMI instance is still alive), NLog's cleanup throws `IOException`. NLog has `throwExceptions="false"` so it shouldn't crash – **but** the internal log target is `D:\temp\nlog-internal.log` which may not exist, suppressing diagnostics (#13).
3. The CSV alarm log uses `archiveAboveSize` *and* date-based filename – at midnight the archive numbering can collide.

This explains "around midnight" specifically.

### #5 — DateTime parsing on possibly-null DB columns

`EF_Service.cs:1801, 1822, 1856` and `LoadPort_VM.cs:858, 1152`:

```csharp
DateTime.Compare(
    DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, ...),
    DateTime.Parse(x.LD_EXPECTED_END.ToString())   // ← .ToString() on possibly null DateTime?
)
```

`TimeFormat = "MM/dd/yyyy HH:mm:ss"`, `Provider = InvariantCulture`. If `LD_EXPECTED_END` (or `CT_END`, `ST_EXPECTED_END`) is `null` or an empty string in the DB, `DateTime.Parse("")` throws `FormatException`. Risk concentrates around the day-boundary because `EF_CompareDateTime` (running at 100 % CPU per #2) blasts through every active SETUPJOB and hits expired rows that may have null end-times.

### #6 — ModBus timer handlers + connect with no guard

`Communication/ModBus/ModBus_Service.cs`:
- `BlinkingLED` (line 152) – **no try/catch**. Calls `_driver.ExecuteGeneric(_portClient, command)`; if the tower-light box is power-cycled overnight, this throws. `System.Timers.Timer` swallows by default in .NET Framework, but the *driver* state can be left inconsistent → subsequent reads start failing.
- `HeartBeatTimer = new Timer(100)` – fires every 100 ms. `ExecuteReadCommand` is synchronous and uses 2 s send/receive timeouts. If a single read blocks 2 s, ~20 timer ticks queue up, each spawning a thread-pool worker → starvation.
- `ConnectToModBus` line 33-37 – `_socket.Connect(...)` has no try/catch. Called from `Frame_VM.InitializeAsyn` (also `async void`).

### #7 — Raw `new Thread(...)` with thin error boundaries

- `Frame_VM.cs:185`: `CheckingDateTimeThread = new Thread(EF_Service.EF_CompareDateTime); .Start();` – no wrapper; relies on the inner `try/catch` (which #2 bypasses).
- `UnloadingLoadPort_VM.cs:129`: `UnloadThread = new Thread(LoadPort_VM.UnloadingCarrierEvent); .Start();`
- `LoadingLoadPort_VM.cs:111`: `LoadThread = new Thread(LoadPort_VM.LoadingCarrierEvent); .Start();`
- `PTLService.cs:259`: `GatewayStatusCheckingThread = new Thread(GatewayStatusChecking);`

Any uncaught exception on these threads = process termination (no #1 handler to log it).

### #8 — `Thread.Abort`

- `UnloadingLoadPort_VM.cs:127`: `UnloadThread?.Abort();`
- `Frame_VM.cs:321`: `CheckingDateTimeThread.Abort();` (in window-closing path)

`Thread.Abort` injects `ThreadAbortException` at an arbitrary instruction. If the thread is mid-`SaveChanges()`, the EF context can be left in a corrupted state. Subsequent operations on the same `DbContext` will throw on every call thereafter.

### #9 — `DbContext` thread-safety

`EF_CompareDateTime` opens `using (var context = new DB_Context())` then iterates with `jobs.ForEach(async x => { ... context.SETUPJOB.Update(x); context.SaveChanges(); ... });`. The `async` lambda may post continuations to the thread pool, where multiple continuations end up touching the **same** `DbContext` concurrently. EF Core 3.1 explicitly forbids this and throws `InvalidOperationException("A second operation started on this context before a previous operation completed.")`.

### #10 — `while (true);` busy-spin

`LoadPort_VM.cs:900` and `:1191` both have a literal `while (true);` (no body). Surrounding code suggests these are placeholders waiting for a flag, but as written they peg one CPU core forever once entered. If reached at midnight (e.g. a SECS handshake timeout path), the UI thread can starve and the watchdog (#6 HeartBeat at 100 ms) will misfire.

### #11 — `.First()` on possibly empty collections

18 `.First()` / `.First(x =>)` call sites — most against `LoadPortModels`, `tmp_job`, `carrierList`. Each throws `InvalidOperationException("Sequence contains no elements")` when the source is empty. Several occur inside `EF_InsertEventLog(...)` argument lists (e.g. `LoadPort_VM.cs:1369, 1471, 1490, 1504`) — error path, where the precondition was *already* violated, making empty collections more likely.

### #12 — SQL Server outage / maintenance window

Connection string: `Data Source=PTSXD2001\SQLEXPRESS;Initial Catalog=Infineon_WMP`. SQL Server Express defaults schedule daily maintenance / backup tasks at midnight. While maintenance runs, EF queries will throw `SqlException`. Many EF methods rethrow with `throw ex;` (15 + sites — `EF_Service.cs:230, 290, 740, 757, 1006, 1034, 1055, 1094, ...`) — those propagate up async-void / Thread methods and crash the process.

### #13 — NLog internal log path

`NLog.config:7`: `internalLogFile="D:\temp\nlog-internal.log"`. If `D:\temp` doesn't exist on the production machine (most likely), NLog's own diagnostics are silently dropped — meaning when #4 / #6 cause logging IO errors, you never see them.

### #14 — Tower-light TCP connect at startup

If the customer power-cycles the tower-light box overnight and a watchdog auto-restarts the HMI, `ModBus_Service.ConnectToModBus()` (`Frame_VM.InitializeAsyn` line 194) can throw `SocketException` from the `_socket.Connect(...)` line. `InitializeAsyn` is `async void` (#3) → unhandled → crash.

### #15 — `throw ex;` resets stack trace

15 + occurrences. Even when the crash *is* logged, the stack trace points at the rethrow line, not the original throw. Replace with `throw;` to preserve the call site.

### #16 — Memory growth over long sessions

`UnloadPortLogs` / `LoadPortLogs` are capped at 30 entries (`UnloadingLoadPort_VM.cs:162-165`). However:
- `Messenger.Default.Register<...>` is called from many constructors with no `Unregister` — `MachineLayout`, `LoadingLoadPort`, `UnloadingLoadPort`, etc. are constructed in `Frame_VM.InitializeMenuContent`. These constructions happen *once*, so per-instance leak is bounded, but if any reset/reload re-registers without unregistering, the message handler list grows monotonically.
- `EF_InsertEventLog` writes to DB synchronously on every state transition. Under DB outage there is no in-memory queue, so the call simply throws (#12).

Worth investigating with a 24-h memory profile.

### #17 — Single-instance suicide on auto-restart

`App.xaml.cs:20-26`:

```csharp
if (Process.GetProcesses().Count(x => x.ProcessName == ...) > 1)
{
    MessageBox.Show("...Already an application is running...");
    Current.Shutdown();
    Process.GetCurrentProcess().Kill();
}
```

If a watchdog restarts the HMI before the previous process has finished its `OnWindowClosing` cleanup (`Frame_VM.cs:279-345` does `CheckingDateTimeThread.Join()` then `.Abort()` — can take seconds), the new process kills itself. Operators arriving in the morning see a dialog box and a dead app. **This isn't a crash** but might be reported as one.

---

## Cross-cutting fixes (cheap, high-leverage)

1. **Add a global unhandled-exception logger** (#1). One file, ~15 lines. Without this, every other fix is half-blind.
2. **Add `Thread.Sleep(1000)` (or a `Task.Delay`) at the bottom of `EF_CompareDateTime`'s while loop** (#2). Single line. Stops 100 % CPU.
3. **Convert `jobs.ForEach(async x => ...)` to a regular `foreach` with `await` inside an `async Task`** (#2). Or wrap each lambda body in `try/catch`.
4. **Wrap every `new Thread(...)` start delegate in `try/catch` that logs `LogHelper.General(6, ...)`** (#7).
5. **Wrap the `BlinkingLED` body in `try/catch`** (#6). One block.
6. **Replace `throw ex;` with `throw;`** repo-wide (#15). 15 sed replacements.
7. **Fix NLog `internalLogFile`** to `${var:basedirCustom}\HMI_Logs\nlog-internal.log` so the rollover failures (#4) become visible (#13).
8. **Guard every `DateTime.Parse(x.SOMETHING.ToString())`** with `string.IsNullOrEmpty` first, or use `DateTime.TryParse` (#5).

## Open questions for the reporter

These would massively narrow down which candidate is firing:

1. **Does it happen *exactly* at 00:00?** Or scattered between 22:00 and 06:00?
   - Exactly 00:00 → strongly points at #4 (NLog rollover) or #12 (SQL maintenance).
   - Scattered overnight → more likely #2/#3 (the bug fires whenever an async-void hits, just more visible when nobody is using the HMI to clear errors).
2. **Is there anything in `D:\HMI_Logs\<date>\Error.log` for the day of the crash?** (Path from `NLog.config:56`.)
3. **Does Windows Event Viewer → Application** show a `.NET Runtime` error with the exception type? That instantly resolves which of #2/#3/#5/#6/#11/#12 fired.
4. **Is the HMI machine on a domain with overnight Windows Update reboots, or running antivirus scheduled scans at midnight?** (Affects #4 and #17.)
5. **Is `D:\temp` present on the production machine?** (#13)
6. **Is `DBChecking` in App.config set to `"1"`?** (Determines whether `EF_CompareDateTime` thread is even running — if `"0"`, candidates #2 and #9 are off the table.)
7. **Does the HMI auto-restart on crash** (Task Scheduler / RestartManager / watchdog)? If yes, #17 becomes a real candidate.
8. **What does `SELECT @@VERSION` and the SQL Express maintenance plan look like?** (#12.)
9. **Approximate uptime before the crash** — fresh start vs. 24 h+ running? Fresh-start crashes lean toward #14/#17; long-uptime lean toward #2/#9/#16.

## Suggested next steps (in order)

1. Land fix **#1** (global handler) + **#7** for `nlog-internal.log` path. Deploy. Wait for next crash. **You will then have a stack trace.** This step alone reduces uncertainty by ~80 %.
2. Land fixes **#2** (sleep) and the `ForEach(async)` rewrite. These are cheap and almost certainly part of the cause.
3. Re-evaluate the table once a real stack trace lands — drop candidates that are exonerated, raise the ones the trace implicates.

---

*Generated from static analysis of the current `main` branch. No runtime data was inspected. Line numbers are valid against the repo at the time of writing.*
