# HMI_CoolingEquipment

Operator HMI (Human-Machine Interface) for the cooling equipment line. WPF desktop application written in C# on .NET Framework 4.8, using MVVM (MvvmLight), MahApps.Metro / Material Design themes, SQLite via Entity Framework Core, NLog, and SECS/GEM + ModBus + PTL machine communication.

> **Platform note:** This is a **Windows-only** application. It targets `.NET Framework 4.8` and references native Windows DLLs (`SQLite.Interop.dll`, `borlndmm.dll`, `Dapapi.dll`) and Windows-only NuGet packages (`MahApps.Metro`, `Microsoft.Xaml.Behaviors.Wpf`, `Microsoft.Data.SqlClient`). It cannot be built or run natively on macOS or Linux. macOS engineers must use a Windows VM or remote Windows host (see the macOS section below).

---

## 1. Prerequisites

### Windows

| Tool | Version | Purpose |
|------|---------|---------|
| Windows 10 or 11 (x64) | — | Host OS |
| Visual Studio 2022 | 17.5+ (Community / Pro / Enterprise) | IDE & build |
| .NET Framework 4.8 Developer Pack | 4.8 | Target framework |
| .NET Framework 4.8 Runtime | 4.8 | Run the built app |
| Workload: **.NET desktop development** | — | WPF + MSBuild |
| NuGet (bundled with VS) | latest | Dependency restore |
| Git for Windows | latest | Source control |
| (Optional) SQLite browser | e.g. *DB Browser for SQLite* | Inspect local DB |

### macOS

You **cannot build the WPF project natively** on macOS. Pick one of:

1. **Parallels Desktop / VMware Fusion / UTM** — install Windows 11 in a VM, then follow the Windows steps inside the VM.
2. **Boot Camp** (Intel Macs only).
3. **Remote Windows machine** (RDP / Azure Virtual Desktop / a build server) and develop there.
4. **VS Code with Remote-SSH** to a Windows host, just for code editing — actual `msbuild` must run on Windows.

Tools you still want locally on macOS:
- Git (`brew install git`)
- VS Code (`brew install --cask visual-studio-code`) for read-only code review

---

## 2. Getting the Source

```bash
git clone <repo-url> HMI_CoolingEquipment
cd HMI_CoolingEquipment
```

The repository layout has the solution at the root and the WPF project one level down:

```
HMI_CoolingEquipment/                 # repo root
├─ HMI_CoolingEquipment.sln           # ← open this in Visual Studio
├─ HMI_CoolingEquipment/              # the WPF project
└─ Libraries/                         # in-repo binary deps (HMI.dll, Pentamaster.dll, PQ.HMI.dll)
```

---

## 3. External Binary Dependencies

This project references DLLs that are **not** on NuGet. They must be present at the expected paths or the build will fail.

| Reference | Expected path | Notes |
|-----------|---------------|-------|
| `HMI.dll` | `Libraries/HMI.dll` | Committed in `Libraries/` |
| `Pentamaster.dll` | `Libraries/Pentamaster.dll` | Committed in `Libraries/` |
| `PQ.HMI.dll` | `Libraries/PQ.HMI.dll` | Committed in `Libraries/` |
| `GEMPro.dll` | `../../../../Others/ModelDLL/HMIModule/SecsGemNetFramework/SecsGem Setup/GEMPro.dll` | **External** — relative to repo. Ask the team for the SECS/GEM module drop. A copy is also kept in `HMI_CoolingEquipment/SecsGem Setup/` and at the project root for reference. |
| `Dapapi.dll`, `borlndmm.dll`, `SQLite.Interop.dll` | next to the `.exe` at runtime | Already in the project; copied to output. |

If the `GEMPro.dll` hint path does not resolve, fix it in `HMI_CoolingEquipment.csproj` (`<HintPath>` for `GEMPro`) or copy the DLL into `Libraries/` and update the reference.

---

## 4. Build

### Visual Studio (recommended)

1. Open `HMI_CoolingEquipment.sln` in Visual Studio 2022.
2. Right-click the solution → **Restore NuGet Packages**.
3. Pick configuration: `Debug | Any CPU` (development) or `Release | x86` (production / 32-bit deployment).
4. **Build → Build Solution** (`Ctrl+Shift+B`).

Output goes to `HMI_CoolingEquipment/bin/<Configuration>/` (e.g. `bin/Debug/HMI_CoolingEquipment.exe`).

### Command line (Developer PowerShell for VS 2022)

```powershell
# from repo root
nuget restore HMI_CoolingEquipment.sln
msbuild HMI_CoolingEquipment.sln /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU"
```

For a release / x86 build:

```powershell
msbuild HMI_CoolingEquipment.sln /p:Configuration=Release /p:Platform=x86
```

> Note: this is a `packages.config` project (not SDK-style), so `dotnet build` will **not** restore packages correctly. Use `nuget restore` + `msbuild`.

---

## 5. Run

### From Visual Studio
- Set `HMI_CoolingEquipment` as the startup project (it is by default — it's the only project).
- Press **F5** (debug) or **Ctrl+F5** (run without debugger).

### From a built binary
```powershell
cd HMI_CoolingEquipment\bin\Debug
.\HMI_CoolingEquipment.exe
```

The app will:
1. Run `Application_Startup` in `App.xaml.cs` — single-instance check (kills the new process if one is already running).
2. Construct `Frame_VM` and show the main `Frame` window.
3. Open / migrate the local SQLite database (`GEM.accdb` / EF migrations under `Migrations/`).
4. Initialize SECS/GEM, ModBus, and PTL communication services as configured in `App.config` and `GEMPro.config`.

Logs are written via NLog — see `NLog.config` for output paths (typically a `logs/` folder next to the exe).

---

## 6. Configuration Files

| File | Purpose |
|------|---------|
| `App.config` | .NET app settings, connection strings |
| `GEMPro.config` | SECS/GEM host configuration |
| `NLog.config` | Logger targets / levels |
| `Default.xml`, `HostScript.xml` | Equipment defaults & SECS host scripts |
| `IPINDEX` | Communication endpoint index |
| `GEM.accdb` | Local Access/SQLite-style DB (the working DB; `GEM - Copy.accdb` is a backup) |

---

## 7. Project / File Structure

```
HMI_CoolingEquipment/                          # repo root
├─ HMI_CoolingEquipment.sln                    # Visual Studio solution
├─ .gitignore / .gitattributes
├─ Libraries/                                  # In-repo binary references
│  ├─ HMI.dll
│  ├─ Pentamaster.dll
│  └─ PQ.HMI.dll
└─ HMI_CoolingEquipment/                       # WPF project
   ├─ HMI_CoolingEquipment.csproj              # MSBuild project (.NET Framework 4.8, packages.config)
   ├─ packages.config                          # NuGet dependencies
   ├─ App.xaml / App.xaml.cs                   # ⭐ ENTRY POINT (Application_Startup)
   ├─ MainWindow.xaml / MainWindow.xaml.cs     # Legacy / unused root window
   ├─ App.config                               # appSettings, connection strings
   ├─ GEMPro.config                            # SECS/GEM config
   ├─ NLog.config / NLog.xsd                   # Logging
   ├─ Default.xml / HostScript.xml / SXFX.dtd  # Equipment defaults & SECS scripts
   ├─ GEM.accdb / GEM - Copy.accdb             # Local DB + backup
   ├─ pentamasterlogo.ico                      # App icon
   ├─ borlndmm.dll / Dapapi.dll / GEMPro.dll
   ├─ SQLite.Interop.dll                       # Native SQLite (x86)
   │
   ├─ Views/                                   # WPF UserControls / Windows (XAML + code-behind)
   │  ├─ Frame/                                #   ⭐ shell window shown at startup
   │  ├─ Alarm/, CarrierDetails/, Dialog/, LotStatus/, MachineLayout/,
   │  │   LoadingLoadPort/, UnloadingLoadPort/, PTL/, Operator/,
   │  │   Report/, SecsGem/, Settings/, UserAccount/
   │
   ├─ ViewModels/                              # MvvmLight VMs, one folder per feature
   │  ├─ Frame/                                #   Frame_VM constructed in App.xaml.cs
   │  ├─ Alarm/, CarrierDetails/, LoadBin/, LoadPort/, LotStatus/,
   │  │   MachineLayout/, Operator/, PTL/, Report/, SecsGem/,
   │  │   Settings/, UnloadBin/, UserAccount/
   │
   ├─ Models/                                  # POCOs / domain types
   │  ├─ Carrier/, EntityFramework/, Event/, Global/, Menu/,
   │  │   Operator/, PTL/, SecsGem/, Unloading/, UserAccount/
   │
   ├─ Communication/                           # External system integration
   │  ├─ EntityFramework/                      #   DbContext + EF helpers
   │  ├─ ModBus/                               #   ModBus TCP/RTU service
   │  ├─ PTL/                                  #   Pick-To-Light controller
   │  └─ SecsGem/                              #   SECS/GEM host service
   │
   ├─ Global/                                  # Cross-cutting helpers
   │  ├─ Enum.cs, GlobalAttributesClass.cs, GlobalClassFunction.cs
   │  ├─ Alarm/, LoadPort/, Settings/
   │
   ├─ Converter/                               # WPF IValueConverters
   ├─ Interface/                               # Shared interfaces
   ├─ Logger/                                  # NLog wrappers
   ├─ Migrations/                              # EF Core migrations (90+ files, v1.0 … vN)
   ├─ Images/                                  # UI assets
   ├─ Properties/                              # AssemblyInfo, settings
   ├─ SecsGem Setup/                           # SECS/GEM redistributables
   └─ VersionHistory.txt                       # Manual changelog
```

### Entry Point

The application entry point is **`App.xaml` / `App.xaml.cs`**, hooked via `Startup="Application_Startup"`:

```
App.xaml.cs::Application_Startup
   └─► new Frame_VM()                  (HMI_CoolingEquipment/ViewModels/Frame/)
        └─► new Frame(viewModel).Show()  (HMI_CoolingEquipment/Views/Frame/)
```

`MainWindow.xaml` / `MainWindow.xaml.cs` exist in the project but are **not** the runtime entry — `Frame` is. Treat `MainWindow` as legacy / scratch.

---

## 8. Database & Migrations

The app uses **Entity Framework Core 3.1.32** against a local SQLite-backed file (`GEM.accdb`). Migrations live in `HMI_CoolingEquipment/Migrations/`.

To add a migration (Package Manager Console in Visual Studio):

```powershell
Add-Migration v<x.y.z.w>
Update-Database
```

The DbContext lives under `HMI_CoolingEquipment/Communication/EntityFramework/`.

---

## 9. Common First-Run Issues

| Symptom | Fix |
|---------|-----|
| `error MSB3245: Could not resolve this reference … GEMPro` | Place `GEMPro.dll` at the path in the `<HintPath>` (see §3) or update the hint path. |
| `BadImageFormatException` at runtime | Build as **x86** — `SQLite.Interop.dll` shipped is 32-bit. |
| App immediately exits with “Already an application is running” | A previous instance is still in Task Manager — kill it. (See `App.xaml.cs:20`.) |
| NuGet packages not restoring | Right-click solution → *Restore NuGet Packages*, or run `nuget restore`. `dotnet restore` does **not** work for `packages.config`. |
| Missing `Microsoft.Data.SqlClient.SNI.dll` at runtime | Ensure NuGet restored `Microsoft.Data.SqlClient.SNI` and that the `runtimes/` folder was copied to the output. |

---

## 10. Where to Start Reading the Code

If you have just been onboarded, read in this order:

1. `App.xaml.cs` — startup wiring.
2. `ViewModels/Frame/Frame_VM.cs` — top-level navigation / shell.
3. `Communication/EntityFramework/` — DbContext and how data is loaded.
4. `Communication/SecsGem/`, `Communication/ModBus/`, `Communication/PTL/` — equipment protocols.
5. `Global/GlobalClassFunction.cs` and `Global/Enum.cs` — shared state and domain enums.
6. `Views/Frame/` and one feature folder pair (e.g. `Views/MachineLayout` + `ViewModels/MachineLayout`) to learn the MVVM pattern used here.

Welcome aboard.
