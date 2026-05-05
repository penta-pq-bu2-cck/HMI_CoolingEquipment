# Manual Unload Buttons

This document lists the buttons in the HMI that let an operator manually unload a carrier / lot, where they live in the source, and how the flow connects.

## Summary

Yes — the HMI provides manual unload controls. There are **three buttons** involved across two screens, plus a related **Purge** button (which is an abort, not a true unload).

| # | Button | Screen | View file | Command |
|---|--------|--------|-----------|---------|
| 1 | **Unload Carrier** | Operator (entry) | `Views/Operator/OperatorView.xaml` | `UnloadingButtonCommand` |
| 2 | **Scan** | Unloading Load Port | `Views/UnloadingLoadPort/UnloadingLoadPort.xaml` | `UnloadLoadPortCommand` |
| 3 | **View Job List** → **Select** | Unload Job Details dialog | `Views/UnloadingLoadPort/UnloadJobDetailsWindow.xaml` | `JobListDetailsCommand` → `DataGridButtonClickCommand` |
| – | **Purge** (abort, not unload) | Operator – Lot Status grid | `Views/Operator/OperatorView.xaml` | `DataGridPurgeButtonClickCommand` |

---

## 1. "Unload Carrier" — Operator screen (top-level entry)

**View:** `HMI_CoolingEquipment/Views/Operator/OperatorView.xaml:17`

```xaml
<Button Content="Unload Carrier"
        Command="{Binding ViewModel.UnloadingButtonCommand}"
        Height="80" FontSize="17"/>
```

**Handler:** `HMI_CoolingEquipment/ViewModels/Operator/Operator_VM.cs:76`

```csharp
private void UnloadingButtonCommandEvent()
{
    Messenger.Default.Send(new OperatorModel
    {
        Name = "Unloading"
    });
}
```

The handler broadcasts an `OperatorModel { Name = "Unloading" }` via MvvmLight `Messenger`, which the shell `Frame_VM` picks up and uses to navigate to the **Unloading Load Port** screen. This button itself does not move any carrier — it just routes the operator to the unload UI.

---

## 2. "Scan" — Unloading Load Port screen (the actual unload trigger)

**View:** `HMI_CoolingEquipment/Views/UnloadingLoadPort/UnloadingLoadPort.xaml:33`

```xaml
<Button Grid.Column="1" Content="Scan"
        Margin="10 0 0 0"
        Command="{Binding ViewModel.UnloadLoadPortCommand}"
        CommandParameter="{Binding ElementName=LotIDTxtBox}"
        IsDefault="True"
        IsEnabled="{Binding ViewModel.UnloadBtnEnable}"/>
```

- Operator types / scans a Lot ID into the `LotIDTxtBox` next to it.
- `IsDefault="True"` — pressing **Enter** in the textbox triggers the same command.
- `IsEnabled` is bound to `UnloadBtnEnable`, so the button is greyed out until prerequisites are met.

**Handler:** `HMI_CoolingEquipment/ViewModels/UnloadBin/UnloadingLoadPort_VM.cs:113`

```csharp
private void UnloadLoadPortCommandEvent(TextBox txtBox) { ... }
```

This is the function that performs the manual unload of the scanned Lot.

---

## 3. "View Job List" + "Select" — pick a specific job to unload manually

**View:** `HMI_CoolingEquipment/Views/UnloadingLoadPort/UnloadingLoadPort.xaml:34`

```xaml
<Button Grid.Column="2" Content="View Job List"
        Margin="10 0 0 0"
        Command="{Binding ViewModel.JobListDetailsCommand}"/>
```

This opens **`UnloadJobDetailsWindow`** (`Views/UnloadingLoadPort/UnloadJobDetailsWindow.xaml`), which lists jobs with a per-row **Select** button:

```xaml
<Button Content="Select" Command="{Binding DataGridButtonClickCommand}">
    <Button.CommandParameter>
        <!-- bound row -->
    </Button.CommandParameter>
</Button>
```

Operators use this when they want to **manually choose** which job/lot to unload from a list rather than scanning a Lot ID.

---

## Related (NOT an unload): "Purge" button

**View:** `HMI_CoolingEquipment/Views/Operator/OperatorView.xaml:49`
**Handler:** `HMI_CoolingEquipment/ViewModels/Operator/Operator_VM.cs:84` — `DataGridPurgeButtonClickCommandEvent`

This button appears on each row of the Lot Status grid and lets the operator **purge** (abort) a job. It is **not** a manual unload — it removes the lot from the active list and writes a `WARNING / ABORT` event log. The Purge row is hidden once `JobStatus == 8`.

---

## End-to-end manual unload flow

```
Operator screen
   └─ [Unload Carrier]                      (OperatorView.xaml:17)
         └─► Messenger: OperatorModel("Unloading")   (Operator_VM.cs:76)
              └─► Frame_VM navigates to Unloading Load Port

Unloading Load Port screen                  (UnloadingLoadPort.xaml)
   ├─ Type/scan Lot ID + [Scan]   ─────────► UnloadLoadPortCommandEvent
   │                                          (UnloadingLoadPort_VM.cs:113)
   └─ [View Job List] ──► UnloadJobDetailsWindow
         └─ per-row [Select] ─────────────► DataGridButtonClickCommand
```

## Verdict

There is **no dedicated "Manual Unload Override"** button (e.g. to force-unload bypassing checks). Manual unloading is done via the **Operator → Unload Carrier → Scan / View Job List → Select** flow described above. Forced removal of a stuck lot is handled by **Purge**, which aborts the job rather than unloading it.

---

## External API / Service Calls Triggered by These Operations

The HMI does **not** call any HTTP / REST / web API. "External" here means anything that crosses the process boundary: equipment hardware, the SECS/GEM host, ModBus PLC, the PTL controller, and the local Entity Framework DB. The table and per-button breakdown below show exactly which of these are hit by each manual-unload button.

### Transports in use

| Channel | How it talks | Where |
|---------|--------------|-------|
| **SECS/GEM** (host MES) | `GEMPro` SDK over TCP (HSMS) | `Communication/SecsGem/SecsGemService.cs` |
| **ModBus TCP** (PLC / I/O) | Raw `Socket` + `ModbusTcpCodec` | `Communication/ModBus/ModBus_Service.cs:5,15,17,33-39` |
| **PTL** (Pick-To-Light controller) | `PTLService` over its own connection (`PTLConnectionInfo`) | `Communication/PTL/PTLService.cs` |
| **Local DB** (EF Core / SQLite) | `DB_Context` → `GEM.accdb` | `Communication/EntityFramework/EF_Service.cs` |
| **HTTP / REST** | — none — | n/a |

### Per-button summary

| Button | Local DB (EF) | PTL | SECS/GEM | ModBus | Notes |
|--------|:-------------:|:---:|:--------:|:------:|-------|
| **Unload Carrier** (Operator entry) | ✗ | ✗ | ✗ | ✗ | Pure UI navigation (`Messenger.Send`). No external call. |
| **Scan** (Unloading Load Port) | ✓ | ✓ | ✓ *(indirectly, on cooling-end)* | ✗ | Triggers `UnloadingCarrierEvent`. See below. |
| **View Job List** | ✗ | ✗ | ✗ | ✗ | Just opens `UnloadJobDetailsWindow`. |
| **Select** (in job list dialog) | ✓ | ✓ | ✓ *(indirectly)* | ✗ | Routes back through `Scan` via `TriggerUnloadFromWindows`. |
| **Purge** (abort) | ✓ | ✗ | ✗ | ✗ | DB-only: `EF_PurgeJob` + event log. |

### Detail: what **Scan** actually calls

Entry: `UnloadingLoadPort_VM.UnloadLoadPortCommandEvent` (`ViewModels/UnloadBin/UnloadingLoadPort_VM.cs:113`)
→ runs `LoadPort_VM.UnloadingCarrierEvent` on a worker thread (`UnloadingLoadPort_VM.cs:129`).

Inside `LoadPort_VM.UnloadingCarrierEvent` (`ViewModels/LoadPort/LoadPort_VM.cs:1575`):

1. **Local DB (Entity Framework)** — many calls:
   - `EF_Service.EF_GetJobIDFromLotID(lotID)` (line 1581)
   - `EF_Service.EF_GetErrorJobFromCarrierID(...)` (1585)
   - `EF_Service.EF_GetCarrierListFromJobID(...)` (1621, 1630)
   - `EF_Service.EF_GetJobIDFromCarrierID(...)` (1629)
   - `EF_Service.EF_UnloadingStatusCheck(lotID, jobID)` (1670)
   - `EF_Service.EF_GetLotIDFromCarrierID(...)`, `EF_GetJobIDFromCarrierID(...)` (1717-1718)
   - `EF_Service.EF_InsertEventLog(...)` — many places, writes audit rows.

2. **PTL controller** — sends LED commands to the physical Pick-To-Light hardware:
   - `ptlService.LoadUnloadProcessSetLed(loadportreadytoloadModel, LoadPortState.CoolingCompleted)` (line 1655)
   - `ptlService.LoadUnloadProcessSetLed(loadportreadytoloadModel, LoadPortState.UnloadProcess)` (line 1684)
   - Implementation: `Communication/PTL/PTLService.cs:1348` — pushes a state change out over the PTL connection.

3. **SECS/GEM host (MES)** — *not* called directly inside `UnloadingCarrierEvent`. The unload-side SECS event (`SendLoadPortReadyToUnload`) fires automatically when cooling time ends, from the EF cooling-timer path:
   - `EF_Service.EF_SentLoadPortReadyToUnloadAsync(jobID)` (`EF_Service.cs:1570`) → `SecsGemService.SendLoadPortReadyToUnload(...)` (`EF_Service.cs:1584`).
   - Called from the cooling-completion branch at `EF_Service.cs:1849`, which also fires `SecsGemService.SendJobTracker(... "Completed")` (line 1833) and `... "Destroyed"` (line 1839).
   - So the operator pressing **Scan** does **not** itself send a SECS event — the host has already been notified that the lot is ready. Scan only updates DB state and PTL LEDs.

4. **Alarm subsystem (in-process, not external):**
   - `AlarmService.ClearAlarm()` (1617)
   - `AlarmService.DefineAlarm(1051 / 1052 / 1054 / 1055)` on the various denied-unload paths (1695, 1723, 1758, 1769). Alarms can themselves cause SECS S5 alarm reports to the host via `SecsGemService`, depending on `AlarmService` config.

5. **ModBus** — *not* called by the unload flow. ModBus is initialized for general I/O (`ModBus_Service.cs:33-39`) but is not invoked by `UnloadingCarrierEvent`.

6. **No HTTP / REST.** The project references `Microsoft.AspNetCore.*` packages, but they are not used to make outbound HTTP calls in the unload path.

### Detail: what **Purge** calls

`Operator_VM.DataGridPurgeButtonClickCommandEvent` (`ViewModels/Operator/Operator_VM.cs:84`):
- `EF_Service.EF_InsertEventLog("WARNING", "ABORT", ...)` — local DB.
- `EF_Service.EF_PurgeJob(jobID, lotNo)` — local DB.
- No PTL, no SECS/GEM, no ModBus, no HTTP.

### TL;DR

- Pressing **Unload Carrier** = no external call (pure navigation).
- Pressing **Scan / Select** (the actual manual-unload trigger) = **DB writes + PTL LED commands**. SECS/GEM was already notified earlier in the lifecycle, when cooling ended.
- Pressing **Purge** = **DB only**.
- Nothing in the manual-unload flow makes HTTP/REST calls or talks to ModBus.
