# Manual Rack / Location Flow

This document answers two questions:

1. **Where is the rack-location data stored?** — SQL Server, table `LOADPORT`.
2. **Which party supplies the locations?** — They are imported manually by the operator from an **Excel file** in the Settings screen. No SECS/GEM host, no PLC/ModBus, no PTL controller provides this data.

> ⚠️ Naming note: there is no "manual rack picker" UI at runtime. The *manual* step is the Excel import that defines the rack/slot catalog. At runtime, the system **auto-assigns** an empty slot to each incoming carrier by priority. If the question is "can the operator hand-pick a slot per carrier?", the answer is **no — only priority-ordered auto-assignment exists today.**

---

## 1. Where the data lives

**Database:** Microsoft SQL Server (`PTSXD2001\SQLEXPRESS`, catalog `Infineon_WMP`)
- `Communication/EntityFramework/DB_Context.cs:7,26`

**Tables involved**

| Table | Purpose | Key columns | Entity file |
|-------|---------|-------------|-------------|
| `LOADPORT` | Rack/slot catalog + current pre-assignment | `LoadPortID`, `LoadPortRack`, `LoadPortColumn`, `LoadPortRow`, `LoadPortPriority`, `LoadPortType`, `LoadPortState`, **`PreAssignLoad`** (carrier id reserved for this slot) | `Models/EntityFramework/LOADPORT.cs:5-49` |
| `JOBDETAILS` | Per-carrier runtime mapping to a slot | **`PortLocation`** (the resolved `LoadPortID`) | `Models/EntityFramework/JOBDETAILS.cs:7-41` |

**EF write paths**

| Method | File:line | What it writes |
|--------|-----------|----------------|
| `EF_UpdateLoadPortConfiguration` | `Communication/EntityFramework/EF_Service.cs:1063-1100` | Replaces the entire `LOADPORT` catalog (clear + AddRange) when Excel is imported |
| `EF_UpdateLoadPort(List<LOADPORT>)` | `EF_Service.cs:871-916` | Batch updates `PreAssignLoad` for many slots |
| `EF_UpdateLoadPort(LoadPortModel)` | `EF_Service.cs:918-956` | Single-slot `PreAssignLoad` update |
| `EF_UpdateCarrierStatusAsync` | `EF_Service.cs:1102-1185` | Sets `JOBDETAILS.PortLocation = LoadPortID` for the carrier |

---

## 2. Where the locations come from

**Source: operator-imported Excel file** (sheet name `LoadPort`).

| Step | File:line |
|------|-----------|
| UI button: *Import LoadPort Configuration* in Settings | `ViewModels/Settings/Settings_VM.cs:119-144` |
| Excel parser (OleDb, `Microsoft.ACE.OLEDB.12.0`, sheet `LoadPort`) | `Global/LoadPort/LoadPortService.cs:15-81` |
| Persist to DB (clear `LOADPORT` then bulk insert) | `EF_Service.cs:1063-1100` |

The Excel sheet supplies these columns per slot: `LoadPortID, LoadPortRack, LoadPortColumn, LoadPortRow, LoadPortPriority, LoadPortType, LoadPortIPAddress, LoadPortNodeAddress` (`LoadPortService.cs:53-60`).

**No external party provides this data**

- SECS/GEM host (MES): no inbound message populates `LOADPORT`. `S2F41 STARTJOB` only triggers the *assignment* step (see below).
- ModBus / PLC: not consulted for rack identity.
- PTL controller: receives LED commands but does not source rack data.

---

## 3. Runtime: which slot a carrier ends up in

This is **automatic-by-priority**, not a per-carrier manual pick.

`ViewModels/LoadPort/LoadPort_VM.cs:1350-1499` — `PreAssignTrayToLoadPort()`:

1. Filter candidates: `LoadPortState == Empty && string.IsNullOrEmpty(PreAssignLoad)` — line 1392.
2. Sort by `LoadPortPriority` — lines 1380, 1438.
3. Assign carrier to top-priority slot: `LoadPortModels.Find(...).PreAssignLoad = carrierList[i].CarrierID` — line 1470.
4. Persist:
   - `EF_UpdateCarrierStatusAsync(carrierID, slotID, JobStatus.Loading, "")` — line 1472 → writes `JOBDETAILS.PortLocation`.
   - `UpdateDataLoadPortModel()` → `EF_UpdateLoadPort(...)` — `LoadPort_VM.cs:1515-1519` → writes `LOADPORT.PreAssignLoad`.

The trigger that calls `PreAssignTrayToLoadPort` is the SECS/GEM host's `S2F41 STARTJOB` command landing in `LoadPort_VM` (around `LoadPort_VM.cs:928`). The host does **not** tell the HMI which slot to use — only that a job is starting.

---

## 4. High-level flow

```
┌──────────────────────────────────────────────────────────────────┐
│ ONE-TIME / WHENEVER LAYOUT CHANGES — operator-driven catalog load │
└──────────────────────────────────────────────────────────────────┘

  Operator (UI)
     │
     │  [Settings → Import LoadPort Configuration]
     │  Settings_VM.cs:119
     ▼
  Excel file (sheet "LoadPort")
     │
     │  LoadPortService.GetLoadPortConfig()
     │  LoadPortService.cs:15  (OleDb / ACE.OLEDB.12.0)
     ▼
  EF_Service.EF_UpdateLoadPortConfiguration()
  EF_Service.cs:1063
     │   clear + AddRange
     ▼
  ┌─────────────────────────────────────────┐
  │ SQL Server  Infineon_WMP.LOADPORT       │
  │   LoadPortID, LoadPortRack, Col, Row,   │
  │   Priority, Type, State, PreAssignLoad  │
  └─────────────────────────────────────────┘


┌──────────────────────────────────────────────────────────────────┐
│ RUNTIME — host triggers, HMI auto-picks slot by priority          │
└──────────────────────────────────────────────────────────────────┘

  SECS/GEM Host (MES)
     │   S2F41  STARTJOB  (lot/carrier list — NO slot info)
     ▼
  LoadPort_VM (StartJobName handler)            LoadPort_VM.cs:928
     │
     ▼
  PreAssignTrayToLoadPort()                     LoadPort_VM.cs:1350
     │   • filter LOADPORT WHERE State=Empty
     │     AND PreAssignLoad IS NULL
     │   • ORDER BY LoadPortPriority
     │   • assign carrier → slot in memory
     ▼
  EF_UpdateCarrierStatusAsync(carrier, slot)    EF_Service.cs:1102
     │  → JOBDETAILS.PortLocation = LoadPortID
     ▼
  EF_UpdateLoadPort(LoadPortModels)             EF_Service.cs:871
     │  → LOADPORT.PreAssignLoad = CarrierID
     ▼
  PTLService.LoadUnloadProcessSetLed(...)       PTLService.cs:1348
     (lights up the physical slot for the operator)


┌──────────────────────────────────────────────────────────────────┐
│ MANUAL UNLOAD (for reference — see MANUAL_UNLOAD.md)              │
└──────────────────────────────────────────────────────────────────┘

  Operator scans Lot ID  ──►  EF_GetJobIDFromLotID(lotID)
                              EF_GetCarrierListFromJobID(jobID)
                              → carrier.PortLocation reveals the slot
                              (location is read back, never scanned)
```

---

## 5. Summary

| Question | Answer |
|----------|--------|
| Where is rack/location data stored? | SQL Server, DB `Infineon_WMP`, table `LOADPORT` (catalog + `PreAssignLoad`) and `JOBDETAILS.PortLocation` (per-carrier mapping). |
| Which party supplies the locations? | The **operator**, by importing an Excel file in *Settings → Import LoadPort Configuration*. No SECS/GEM host, no ModBus/PLC, no PTL contributes location data. |
| Who decides which slot a given carrier goes into? | The HMI itself, via `PreAssignTrayToLoadPort()` — pure priority-ordered auto-assignment over empty `LOADPORT` rows. The SECS host only triggers job start; it does not name the slot. |
| Is there a per-carrier *manual* slot picker? | No. Adding one would require a UI to override `PreAssignLoad` before `EF_UpdateLoadPort` is called. |
