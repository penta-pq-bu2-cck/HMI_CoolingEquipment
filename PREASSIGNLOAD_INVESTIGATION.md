# `LOADPORT.PreAssignLoad` — Mutation Investigation

> **Scope:** Every method in this project that *writes* to the `PreAssignLoad` column of the `LOADPORT` table (or the matching property on the in-memory `LoadPortModel`). Reads are excluded. Use this as a reference when changing the assignment lifecycle, debugging stuck slots, or auditing why a carrier "vanished" from a port.

The column itself is declared at `HMI_CoolingEquipment/Models/EntityFramework/LOADPORT.cs:46`:

```csharp
public string? PreAssignLoad { get; set; } = null;
```

Its in-memory mirror lives on `HMI_CoolingEquipment/Models/Global/LoadPortModel.cs:179` (raises `PropertyChanged` on set).

---

## Summary

### A. DB persistence (these actually issue `UPDATE` against SQL Server)

| # | Method | File:line | What it does to `PreAssignLoad` |
|---|--------|-----------|---------------------------------|
| 1 | `EF_Service.EF_AbortJob(jobID, lotID)` | `Communication/EntityFramework/EF_Service.cs:387` → write at `:402` | **Clears to `null`** for every carrier of the aborted job (also sets `LoadPortState = Empty`). |
| 2 | `EF_Service.EF_UpdateLoadPort(List<LOADPORT>)` | `EF_Service.cs:871` → write at `:898` | Bulk overwrite — copies `x.PreAssignLoad` onto each tracked `LOADPORT` row. |
| 3 | `EF_Service.EF_UpdateLoadPort(LoadPortModel)` | `EF_Service.cs:918` → write at `:942` | Single-row overwrite — same column-for-column copy. |
| 4 | `EF_Service.EF_UpdateLoadPort(LOADPORT)` | `EF_Service.cs:727-742` | `context.LOADPORT.Update(loadport)` — persists whatever `PreAssignLoad` is on the passed entity. **Rethrows on failure.** |

### B. In-memory mutation of the runtime `LoadPortModels` collection

These don't hit the DB by themselves; they are flushed later via one of (A) — typically `LoadPort_VM.UpdateDataLoadPortModel()` → `EF_UpdateLoadPort(...)`.

| # | Method | File:line | What it does |
|---|--------|-----------|--------------|
| 5 | `LoadPort_VM.PreAssignTrayToLoadPort(carrierList)` | `ViewModels/LoadPort/LoadPort_VM.cs:1350` → writes at `:1470` and `:1489` | **Primary assignment site.** Picks the highest-priority empty slot and sets `PreAssignLoad = carrierID`. |
| 6 | `LoadPort_VM.UpdateLoadPortModels(LoadPortModel)` | `LoadPort_VM.cs:399` → write at `:410` | Mirror inbound update onto the cached row. |
| 7 | `PTL_VM.UpdateLoadPortModels(List<LoadPortModel>)` | `ViewModels/PTL/PTL_VM.cs:216` → write at `:245` | Mirror full-list refresh. |
| 8 | `PTL_VM.UpdateLoadPortDB(LoadPortModel)` | `PTL_VM.cs:253` → object init at `:278`, persists via `EF_UpdateLoadPort` at `:288` | Operator toggles enable/disable on a port; `PreAssignLoad` is carried along. |
| 9 | `PTLService.UpdateLoadPortModels(LoadPortModel)` | `Communication/PTL/PTLService.cs:118` → write at `:131` | Mirror inbound update on the service-side cached row. |
| 10 | `PTLService.UpdateLoadPortState_LoadProcess(...)` | `PTLService.cs:1625` → write at `:1640` | **Clears to `null`** when the physical load completes — moves the value into `LoadPortCarrierLoad`. Followed by `EF_UpdateCarrierStatusAsync(...)` at `:1648`. |

### C. Mappers (object initializers, not mutations of an existing record)

- `GlobalClassFunction.LoadPortMap(List<LOADPORT>)` — `Global/GlobalClassFunction.cs:315` (init at `:336`) — entity → model.
- `GlobalClassFunction.LoadPortMap(List<LoadPortModel>)` — `GlobalClassFunction.cs:345` (init at `:366`) — model → entity.

### D. Dead code (no callers)

- `Communication/PTL/PTLService_Old.cs:859` and `:1219` — both clear `PreAssignLoad` to `null`. `PTLService_Old` is not referenced anywhere in the project. Safe to ignore or delete.

---

## Lifecycle

```
ASSIGN  : LoadPort_VM.PreAssignTrayToLoadPort      [1470 / 1489]   (in-memory)
          → flushed by EF_UpdateLoadPort           [898 / 942]     (DB write)

CLEAR   : PTLService.UpdateLoadPortState_LoadProcess [1640]        (in-memory, on physical load done)
          → flushed by EF_UpdateLoadPort           [898 / 942]
   OR   : EF_Service.EF_AbortJob                    [402]          (DB write directly, on abort)

MIRROR  : LoadPort_VM.UpdateLoadPortModels          [410]
          PTL_VM.UpdateLoadPortModels               [245]
          PTLService.UpdateLoadPortModels           [131]          (cache sync only)
```

Outline of a successful carrier assignment, end-to-end:

1. SECS/GEM host sends `S2F41 STARTJOB` → handler in `LoadPort_VM` (~`:928`).
2. `PreAssignTrayToLoadPort` runs (`LoadPort_VM.cs:1350`):
   - Filters `LoadPortState == Empty && string.IsNullOrEmpty(PreAssignLoad)`.
   - Sorts by `LoadPortPriority`.
   - Sets `PreAssignLoad = carrierID` on the chosen slot (line `:1470` or `:1489`).
   - Calls `EF_UpdateCarrierStatusAsync` to write `JOBDETAILS.PortLocation`.
3. `UpdateDataLoadPortModel()` flushes the whole collection through `EF_UpdateLoadPort(List<LOADPORT>)` → DB row gains `PreAssignLoad`.
4. PTL controller signals "carrier physically loaded" → `PTLService.UpdateLoadPortState_LoadProcess` (`:1625`) clears `PreAssignLoad` to `null`, copies the value into `LoadPortCarrierLoad`, and calls `EF_UpdateCarrierStatusAsync` to advance to `Cooling`.
5. If the operator hits **Purge** or the host sends an abort, `EF_AbortJob` (`:387`) clears `PreAssignLoad` directly in the DB.

---

## Outcome / Findings

### 1. `EF_PurgeJob` does NOT clear `PreAssignLoad` (likely bug)

`EF_Service.cs:484-505` walks the same `JOBDETAILS` rows as `EF_AbortJob`, finds the matching `LOADPORT` via `PreAssignLoad.Equals(x.CarrierID)` (line `:487`), but only flips `LoadPortState = Empty` (line `:490`). It never sets `port.PreAssignLoad = null` the way `EF_AbortJob` does at `:402`.

**Effect:** A purged carrier leaves a stale `PreAssignLoad` on the slot. The next `PreAssignTrayToLoadPort` will *skip* that slot because its filter requires `string.IsNullOrEmpty(PreAssignLoad)` (`LoadPort_VM.cs:1392`, `:1487`). Slot becomes effectively unusable until something else overwrites it.

**Fix:** add `port.PreAssignLoad = null;` between lines `:490` and `:491`.

### 2. `EF_UpdateLoadPort(LOADPORT)` rethrows on failure

`EF_Service.cs:727-742` is the only `EF_UpdateLoadPort` overload that does `throw ex;` (line `:740`). It is called from `PTL_VM.UpdateLoadPortDB` at `Communication/PTL/PTLService.cs:288` inside a `Task.Run(...)`. If the EF call faults (DB connection drop, concurrency exception, etc.), the rethrow surfaces on a worker task with no observer — which, given there is no global `TaskScheduler.UnobservedTaskException` handler in `App.xaml.cs`, can tear down the process. Cross-reference: see `CRASH_INVESTIGATION.md` candidates #1 and #9.

### 3. There is no manual per-carrier slot picker

Every write site above is either automatic (priority-driven `PreAssignTrayToLoadPort`) or a clear/mirror. No UI command lets the operator hand-pick which slot a specific carrier occupies. If that feature is wanted, the natural insertion point is between steps 2 and 3 of the lifecycle: a slot-picker dialog overrides `loadportlocationPreAssign[i]` before `EF_UpdateLoadPort` is called. Cross-reference: `FLOW.md`.

### 4. Three independent in-memory mirrors

`LoadPort_VM.LoadPortModels`, `PTL_VM.LoadPortModels`, and `PTLService.LoadPortModels` are kept in sync only via the three `UpdateLoadPortModels` mirror methods (#6, #7, #9). If any one of them is invoked from a thread that bypasses the WPF dispatcher path, divergence is possible — a slot can show "free" in one VM and "pre-assigned" in another. Worth a unit test or a single source of truth refactor.

### 5. Mapper symmetry is fine

Both `GlobalClassFunction.LoadPortMap` overloads copy `PreAssignLoad` 1:1, so round-tripping `LOADPORT ↔ LoadPortModel` does not lose the value.

---

## Cross-references

- `MANUAL_UNLOAD.md` — operator-facing unload flow that consumes `PreAssignLoad` indirectly via `JOBDETAILS.PortLocation`.
- `FLOW.md` — overall rack/location data flow; explains why `PreAssignLoad` is the only "current assignment" column.
- `CRASH_INVESTIGATION.md` — candidate #9 (EF DbContext threading) and #1 (no global unhandled-exception handler) interact with finding #2 above.
