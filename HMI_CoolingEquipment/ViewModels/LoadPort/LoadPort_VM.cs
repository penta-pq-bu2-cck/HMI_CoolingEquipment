using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using PQ.HMI;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using GEMPro300.Model;
using System.Windows.Forms;
using System.Windows.Controls.Primitives;
using Application = System.Windows.Application;
using System.Runtime.CompilerServices;
using HMI_CoolingEquipment.Views;

namespace HMI_CoolingEquipment.ViewModels
{
    public class LoadPort_VM : ViewModelBase
    {
        private static WaitingForHostWindow popup;

        private System.Timers.Timer _timer;
        private PTLService ptlService;
        public string IDFromInterface { get; set; }
        public List<PTLConnectionInfo> ConnectionInfos { get; set; }

        #region Models
        private SolidColorBrush _loadPortColourAbort = Brushes.Transparent;
        public SolidColorBrush LoadPortColourAbort
        {
            get { return _loadPortColourAbort; }
            set
            {
                if (_loadPortColourAbort != value)
                {
                    _loadPortColourAbort = value;
                    RaisePropertyChanged(nameof(LoadPortColourAbort));
                }
            }
        }
        
        private SolidColorBrush _loadPortColourLoading = Brushes.Transparent;
        public SolidColorBrush LoadPortColourLoading
        {
            get { return _loadPortColourLoading; }
            set
            {
                if (_loadPortColourLoading != value)
                {
                    _loadPortColourLoading = value;
                    RaisePropertyChanged(nameof(LoadPortColourLoading));
                }
            }
        }

        private SolidColorBrush _loadPortColourUnloading = Brushes.Transparent;
        public SolidColorBrush LoadPortColourUnloading
        {
            get { return _loadPortColourUnloading; }
            set
            {
                if (_loadPortColourUnloading != value)
                {
                    _loadPortColourUnloading = value;
                    RaisePropertyChanged(nameof(LoadPortColourUnloading));
                }
            }
        }

        private SolidColorBrush _loadPortDisabledColour = Brushes.Transparent;
        public SolidColorBrush LoadPortDisabledColour
        {
            get { return _loadPortDisabledColour; }
            set
            {
                if (_loadPortDisabledColour != value)
                {
                    _loadPortDisabledColour = value;
                    RaisePropertyChanged(nameof(LoadPortDisabledColour));
                }
            }
        }

        private SolidColorBrush _loadPortEmptyColour = Brushes.Transparent;
        public SolidColorBrush LoadPortEmptyColour
        {
            get { return _loadPortEmptyColour; }
            set
            {
                if (_loadPortEmptyColour != value)
                {
                    _loadPortEmptyColour = value;
                    RaisePropertyChanged(nameof(LoadPortEmptyColour));
                }
            }
        }

        private SolidColorBrush _loadPortLoadedColour = Brushes.Transparent;
        public SolidColorBrush LoadPortLoadedColour
        {
            get { return _loadPortLoadedColour; }
            set
            {
                if (_loadPortLoadedColour != value)
                {
                    _loadPortLoadedColour = value;
                    RaisePropertyChanged(nameof(LoadPortLoadedColour));
                }
            }
        }

        private SolidColorBrush _loadPortCoolingCompletedColour = Brushes.Transparent;
        public SolidColorBrush LoadPortCoolingCompletedColour
        {
            get { return _loadPortCoolingCompletedColour; }
            set
            {
                if (_loadPortCoolingCompletedColour != value)
                {
                    _loadPortCoolingCompletedColour = value;
                    RaisePropertyChanged(nameof(LoadPortCoolingCompletedColour));
                }
            }
        }

        private SolidColorBrush _loadPortErrorColour = Brushes.Transparent;
        public SolidColorBrush LoadPortErrorColour
        {
            get { return _loadPortErrorColour; }
            set
            {
                if (_loadPortErrorColour != value)
                {
                    _loadPortErrorColour = value;
                    RaisePropertyChanged(nameof(LoadPortErrorColour));
                }
            }
        }

        private List<LoadPortConfigurationModel> _loadPortConfig = new List<LoadPortConfigurationModel>();
        public List<LoadPortConfigurationModel> LoadPortConfig
        {
            get { return _loadPortConfig; }
            set
            {
                if (_loadPortConfig != value)
                {
                    _loadPortConfig = value;
                    RaisePropertyChanged(nameof(LoadPortConfig));
                }
            }
        }

        private List<LoadPortModel> _loadPortModels = new List<LoadPortModel>();
        public List<LoadPortModel> LoadPortModels
        {
            get { return _loadPortModels; }
            set
            {
                if (_loadPortModels != value)
                {
                    _loadPortModels = value;
                    RaisePropertyChanged(nameof(LoadPortModels));
                }
            }
        }

        private RackModel _rackModel = new RackModel();
        public RackModel RackModel
        {
            get { return _rackModel; }
            set
            {
                if (value != _rackModel)
                {
                    _rackModel = value;
                    RaisePropertyChanged(nameof(RackModel));
                }
            }
        }

        private SetupJobModel _setupJobModel = new SetupJobModel();
        public SetupJobModel SetupJobModel
        {
            get { return _setupJobModel; }
            set
            {
                if (value != _setupJobModel)
                {
                    _setupJobModel = value;
                    RaisePropertyChanged(nameof(SetupJobModel));
                }
            }
        }

        private UserAccountModel _userAccount = null;
        public UserAccountModel UserAccount
        {
            get { return _userAccount; }
            set
            {
                if (value != _userAccount)
                {
                    _userAccount = value;
                    RaisePropertyChanged(nameof(UserAccount));
                }
            }
        }
        #endregion

        public LoadPort_VM()
        {
            InitializeTimer();
            Messenger.Default.Register<List<LoadPortConfigurationModel>>(this, UpdateLoadPortConfig);
            Messenger.Default.Register<List<LoadPortModel>>(this, UpdateLoadPortModels);
            Messenger.Default.Register<UserAccountModel>(this, UpdateUserAccount);
            Messenger.Default.Register<PTLService>(this, UpdatePTLService);
            Messenger.Default.Register<List<PTLConnectionInfo>>(this, UpdatePTLConnectionInfo);
            OnUpdateLoadPortModelData += UpdateLoadPortModels;
        }

        ~LoadPort_VM()
        {
            if (_timer != null)
            {
                if (_timer.Enabled)
                {
                    _timer.Stop();
                } 
            }
        }

        private void UpdateLoadPortConfig(List<LoadPortConfigurationModel> loadportConfigModel)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    LoadPortConfig = loadportConfigModel;

                    loadportConfigModel.ForEach(x =>
                    {
                        switch (x.LoadPortStatusState)
                        {
                            case LoadPortState.Disabled:
                                LoadPortDisabledColour = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                break;
                            case LoadPortState.Empty:
                                LoadPortEmptyColour = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                break;
                            case LoadPortState.Loaded:
                                LoadPortLoadedColour = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                break;
                            case LoadPortState.CoolingCompleted:
                                LoadPortCoolingCompletedColour = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                break;
                            case LoadPortState.Error:
                                LoadPortErrorColour = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                break;
                            case LoadPortState.UnloadProcess:
                                LoadPortColourUnloading = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                break;
                            case LoadPortState.LoadProcess:
                                LoadPortColourLoading = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                break;
                            case LoadPortState.Abort:
                                LoadPortColourAbort = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                break;
                        }
                    });
                }));
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }
            finally
            {
                UpdateLoadPortColour();
            }
        }

        private void UpdateLoadPortColour()
        {
            if (RackModel.TubeRack.SelectMany(x => x.RackLoadPorts).ToList().Any(x => !x.LoadPortStateAndConnection.Item3))
            {
                RackModel.TubeRack.SelectMany(x => x.RackLoadPorts).ToList().ForEach(y =>
                {
                    if (!y.LoadPortStateAndConnection.Item3)
                    {
                        if (y.LoadPortStateAndConnection.Item2)
                        {
                            if (y.LoadPortStateAndConnection.Item4)
                            {
                                y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Error)).LoadPortLEDColour);
                            }
                            else
                            {
                                switch (y.LoadPortStateAndConnection.Item1)
                                {
                                    case LoadPortState.Disabled:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Disabled)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.Empty:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Empty)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.Loaded:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Loaded)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.CoolingCompleted:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.CoolingCompleted)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.Error:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Error)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.UnloadProcess:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.UnloadProcess)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.LoadProcess:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.LoadProcess)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.Abort:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Abort)).LoadPortLEDColour);
                                        break;
                                }
                            }
                        }
                    }
                });
            }

            if (RackModel.TrayRack.SelectMany(x => x.RackLoadPorts).ToList().Any(x => !x.LoadPortStateAndConnection.Item3))
            {
                RackModel.TrayRack.SelectMany(x => x.RackLoadPorts).ToList().ForEach(y =>
                {
                    if (!y.LoadPortStateAndConnection.Item3)
                    {
                        if (y.LoadPortStateAndConnection.Item2)
                        {
                            if (y.LoadPortStateAndConnection.Item4)
                            {
                                y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Error)).LoadPortLEDColour);
                            }
                            else
                            {
                                switch (y.LoadPortStateAndConnection.Item1)
                                {
                                    case LoadPortState.Disabled:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Disabled)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.Empty:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Empty)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.Loaded:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Loaded)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.CoolingCompleted:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.CoolingCompleted)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.Error:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Error)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.UnloadProcess:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.UnloadProcess)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.LoadProcess:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.LoadProcess)).LoadPortLEDColour);
                                        break;
                                    case LoadPortState.Abort:
                                        y.LoadPortColour = GlobalClassFunction.ColourMap(LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Abort)).LoadPortLEDColour);
                                        break;
                                }
                            }
                        }
                    }
                });
            }
        }

        private void UpdateLoadPortModels(List<LoadPortModel> loadportmodels)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                lock (LoadPortModels)
                {
                    LoadPortModels = loadportmodels;
                }
                UpdateLoadPortRackList(loadportmodels);
                UpdateLoadPortColour();
            }));
        }

        private void UpdateLoadPortModels(LoadPortModel loadportmodel)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if(LoadPortModels.Any(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)))
                {
                    if (!LoadPortModels.Find(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)).LoadPortState.Equals(loadportmodel.LoadPortState) && loadportmodel.LoadPortState.Equals(LoadPortState.Abort))
                    {
                        ptlService.LoadUnloadProcessSetLed(loadportmodel, loadportmodel.LoadPortState);
                    }

                    LoadPortModels.Find(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)).PreAssignLoad = loadportmodel.PreAssignLoad;
                    LoadPortModels.Find(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)).LoadPortCarrierLoad = loadportmodel.LoadPortCarrierLoad;
                    LoadPortModels.Find(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)).LoadPortState = loadportmodel.LoadPortState;
                    LoadPortModels.Find(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)).LoadPortError = loadportmodel.LoadPortError;
                }

                UpdateLoadPortRackList(LoadPortModels);
                UpdateLoadPortColour();

                RaisePropertyChanged(nameof(LoadPortModels));
            }));
        }

        private void UpdateLoadPortRackList(List<LoadPortModel> loadportmodels)
        {
            lock (RackModel)
            {
                RackModel = GlobalClassFunction.RackModelMap(loadportmodels);
            }
        }

        private void UpdateUserAccount(UserAccountModel model)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    UserAccount = model;
                }));
            }
            catch (Exception ex)
            {
                LogHelper.PTL(5, ex.Message);
            }
        }

        private void UpdatePTLService(PTLService service)
        {
            ptlService = service;
        }

        private void UpdatePTLConnectionInfo(List<PTLConnectionInfo> list)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ConnectionInfos = list;
            }));
        }

        private void InitializeTimer()
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += _timerBlinkingDisplay;
            _timer.Interval = 500;
            _timer.Start();
        }

        private void _timerBlinkingDisplay(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (LoadPortConfig.Any(x => x.LoadPortLEDState.Equals(LEDState.Blinking)))
                    {
                        LoadPortConfig.Where(x => x.LoadPortLEDState.Equals(LEDState.Blinking)).ToList().ForEach(x =>
                        {
                            switch (x.LoadPortStatusState)
                            {
                                case LoadPortState.Disabled:
                                    if (LoadPortDisabledColour.Equals(GlobalClassFunction.ColourMap(x.LoadPortLEDColour)))
                                    {
                                        LoadPortDisabledColour = Brushes.Transparent;
                                    }
                                    else
                                    {
                                        LoadPortDisabledColour = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                    }
                                    break;
                                case LoadPortState.Empty:
                                    if (LoadPortEmptyColour.Equals(GlobalClassFunction.ColourMap(x.LoadPortLEDColour)))
                                    {
                                        LoadPortEmptyColour = Brushes.Transparent;
                                    }
                                    else
                                    {
                                        LoadPortEmptyColour = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                    }
                                    break;
                                case LoadPortState.Loaded:
                                    if (LoadPortLoadedColour.Equals(GlobalClassFunction.ColourMap(x.LoadPortLEDColour)))
                                    {
                                        LoadPortLoadedColour = Brushes.Transparent;
                                    }
                                    else
                                    {
                                        LoadPortLoadedColour = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                    }
                                    break;
                                case LoadPortState.CoolingCompleted:
                                    if (LoadPortCoolingCompletedColour.Equals(GlobalClassFunction.ColourMap(x.LoadPortLEDColour)))
                                    {
                                        LoadPortCoolingCompletedColour = Brushes.Transparent;
                                    }
                                    else
                                    {
                                        LoadPortCoolingCompletedColour = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                    }
                                    break;
                                case LoadPortState.Error:
                                    if (LoadPortErrorColour.Equals(GlobalClassFunction.ColourMap(x.LoadPortLEDColour)))
                                    {
                                        LoadPortErrorColour = Brushes.Transparent;
                                    }
                                    else
                                    {
                                        LoadPortErrorColour = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                    }
                                    break;
                                case LoadPortState.UnloadProcess:
                                    if (LoadPortColourUnloading.Equals(GlobalClassFunction.ColourMap(x.LoadPortLEDColour)))
                                    {
                                        LoadPortColourUnloading = Brushes.Transparent;
                                    }
                                    else
                                    {
                                        LoadPortColourUnloading = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                    }
                                    break;
                                case LoadPortState.LoadProcess:
                                    if (LoadPortColourLoading.Equals(GlobalClassFunction.ColourMap(x.LoadPortLEDColour)))
                                    {
                                        LoadPortColourLoading = Brushes.Transparent;
                                    }
                                    else
                                    {
                                        LoadPortColourLoading = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                    }
                                    break;
                                case LoadPortState.Abort:
                                    if (LoadPortColourAbort.Equals(GlobalClassFunction.ColourMap(x.LoadPortLEDColour)))
                                    {
                                        LoadPortColourAbort = Brushes.Transparent;
                                    }
                                    else
                                    {
                                        LoadPortColourAbort = GlobalClassFunction.ColourMap(x.LoadPortLEDColour);
                                    }
                                    break;
                            }
                        });
                    }

                    if (RackModel.TubeRack.SelectMany(x => x.RackLoadPorts).ToList().Any(x => x.LoadPortStateAndConnection.Item3))
                    {
                        RackModel.TubeRack.SelectMany(x => x.RackLoadPorts).ToList().ForEach(y =>
                        {
                            if (y.LoadPortStateAndConnection.Item3)
                            {
                                if (y.LoadPortStateAndConnection.Item2)
                                {
                                    if (y.LoadPortStateAndConnection.Item4)
                                    {
                                        y.LoadPortColour = LoadPortErrorColour;
                                    }
                                    else
                                    {
                                        switch (y.LoadPortStateAndConnection.Item1)
                                        {
                                            case LoadPortState.Disabled:
                                                y.LoadPortColour = LoadPortDisabledColour;
                                                break;
                                            case LoadPortState.Empty:
                                                y.LoadPortColour = LoadPortEmptyColour;
                                                break;
                                            case LoadPortState.Loaded:
                                                y.LoadPortColour = LoadPortLoadedColour;
                                                break;
                                            case LoadPortState.CoolingCompleted:
                                                y.LoadPortColour = LoadPortCoolingCompletedColour;
                                                break;
                                            case LoadPortState.Error:
                                                y.LoadPortColour = LoadPortErrorColour;
                                                break;
                                            case LoadPortState.UnloadProcess:
                                                y.LoadPortColour = LoadPortColourUnloading;
                                                break;
                                            case LoadPortState.LoadProcess:
                                                y.LoadPortColour = LoadPortColourLoading;
                                                break;
                                            case LoadPortState.Abort:
                                                y.LoadPortColour = LoadPortColourAbort;
                                                break;
                                        }
                                    }
                                }
                            }
                        });
                    }

                    if (RackModel.TrayRack.SelectMany(x => x.RackLoadPorts).ToList().Any(x => x.LoadPortStateAndConnection.Item3))
                    {
                        RackModel.TrayRack.SelectMany(x => x.RackLoadPorts).ToList().ForEach(y =>
                        {
                            if (y.LoadPortStateAndConnection.Item3)
                            {
                                if (y.LoadPortStateAndConnection.Item2)
                                {
                                    if (y.LoadPortStateAndConnection.Item4)
                                    {
                                        y.LoadPortColour = LoadPortErrorColour;
                                    }
                                    else
                                    {
                                        switch (y.LoadPortStateAndConnection.Item1)
                                        {
                                            case LoadPortState.Disabled:
                                                y.LoadPortColour = LoadPortDisabledColour;
                                                break;
                                            case LoadPortState.Empty:
                                                y.LoadPortColour = LoadPortEmptyColour;
                                                break;
                                            case LoadPortState.Loaded:
                                                y.LoadPortColour = LoadPortLoadedColour;
                                                break;
                                            case LoadPortState.CoolingCompleted:
                                                y.LoadPortColour = LoadPortCoolingCompletedColour;
                                                break;
                                            case LoadPortState.Error:
                                                y.LoadPortColour = LoadPortErrorColour;
                                                break;
                                            case LoadPortState.UnloadProcess:
                                                y.LoadPortColour = LoadPortColourUnloading;
                                                break;
                                            case LoadPortState.LoadProcess:
                                                y.LoadPortColour = LoadPortColourLoading;
                                                break;
                                            case LoadPortState.Abort:
                                                y.LoadPortColour = LoadPortColourAbort;
                                                break;
                                        }
                                    }
                                }
                            }
                        });
                    }
                }));
            }

        }

        public delegate void UpdateLoadPortModelData(LoadPortModel model);
        public static event UpdateLoadPortModelData OnUpdateLoadPortModelData;
        public static void LoadPortModelData(LoadPortModel model)
        {
            try
            {
                if (OnUpdateLoadPortModelData != null)
                {
                    EF_Service.EF_UpdateLoadPort(model);
                    OnUpdateLoadPortModelData(model);
                }
            }
            catch (Exception ex) { LogHelper.General(5, ex.ToString()); }
        }

        #region Loading
        public delegate void UpdateLoadingLogData(string log);
        public static event UpdateLoadingLogData OnUpdateLoadingLogData;
        public static void LoadingLogData(string log)
        {
            try
            {
                if (OnUpdateLoadingLogData != null)
                {
                    OnUpdateLoadingLogData(log);
                }
            }
            catch (Exception ex) { LogHelper.General(5, ex.ToString()); }
        }

        public delegate void UpdateLoadingLocationData(LogValue log);
        public static event UpdateLoadingLocationData OnUpdateLoadingLocationData;
        public static void LoadingLocationData(LogValue log)
        {
            try
            {
                if (OnUpdateLoadingLocationData != null)
                {
                    OnUpdateLoadingLocationData(log);
                }
            }
            catch (Exception ex) { LogHelper.General(5, ex.ToString()); }
        }
        
        public delegate void UpdateLoadingLogMessageFromPTL(LogValue log);
        public static event UpdateLoadingLogMessageFromPTL OnUpdateLoadingLogMessageFromPTL;
        public static void LoadingLogMessageFromPTL(LogValue log)
        {
            try
            {
                if (OnUpdateLoadingLogMessageFromPTL != null)
                {
                    OnUpdateLoadingLogMessageFromPTL(log);
                }
            }
            catch (Exception ex) { LogHelper.General(5, ex.ToString()); }
        }
        public static void TogglePopup(string message = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (message != null)
                {
                    popup = new WaitingForHostWindow(message);
                    popup.Show();
                }
                else
                {
                    popup.Close();
                    popup = null;
                }
            });
        }

        public async void LoadingCarrierEvent()
        {
            GlobalAttributesClass.HMINoActiveJob = true;
            string txtbox = IDFromInterface;
            string carrierID = txtbox.ToUpper();

            string LotID = EF_Service.EF_GetLotIDFromCarrierID(carrierID);
            string JobID = EF_Service.EF_GetJobIDFromCarrierID(carrierID);
            bool isJobPurged = EF_Service.EF_CheckJobPurged(JobID);

            List<SETUPJOB> tmp_job = EF_Service.EF_GetErrorJobFromCarrierID(JobStatus.LoadingTimeExpired);

            bool isEnableScan = false;

            LogHelper.General(1, $"Carrier ID Scanned: {carrierID}");
            LogHelper.General(1, $"Loading Mode: {GlobalAttributesClass.LoadingMode}");

            // check LoadingMode
            if (GlobalAttributesClass.LoadingMode.Equals(0))  //load by carrier ID
            {
                try
                {
                    if (LoadPortModels.Any(x => x.LoadPortState.Equals(LoadPortState.LoadProcess))) //Third FAT
                    {
                        LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Mode: {GlobalAttributesClass.LoadingMode}] [Loading] - Loadports in <LoadProcess>.");
                        LoadPortModels.FindAll(x => x.LoadPortState.Equals(LoadPortState.LoadProcess)).ToList().ForEach(async y =>
                        {
                            //LoadPortModel loadPortID = LoadPortModels.Find(x => x.LoadPortState.Equals(LoadPortState.LoadProcess));
                            LoadPortModel loadPortID = y;
                            string tmp_carrierID = y.PreAssignLoad;
                            ptlService.LoadUnloadProcessSetLed(loadPortID, LoadPortState.Empty);

                            string tmp_LotID = EF_Service.EF_GetLotIDFromCarrierID(carrierID);
                            string tmp_JobID = EF_Service.EF_GetJobIDFromCarrierID(carrierID);

                            LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading] - Carrier ID = [{tmp_carrierID}], Lot ID = [{tmp_LotID}], Cancel loading process."); 
                            LoadingLogData($"Carrier ID = [{tmp_carrierID}], Lot ID = [{tmp_LotID}] - Cancel Carrier Loading Process");
                            EF_Service.EF_InsertEventLog("TRACE", "LOADING", tmp_JobID, tmp_LotID, tmp_carrierID, loadPortID.LoadPortID, UserAccount.UserName, "Cancel Carrier Loading Process");

                            //Sent LoadPortEmpty
                            //await SecsGemService.SendEventSecsGemAsync(103, $"{loadPortID.LoadPortID}", 101);
                            SecsGemService.SendLoadPortReadyToLoad(loadPortID.LoadPortID).Wait(200);

                            EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", tmp_JobID, tmp_LotID, tmp_carrierID, loadPortID.LoadPortID, UserAccount.UserName, "Sent Event 101-LoadPortEmpty");
                        });
                    }

                    if (string.IsNullOrEmpty(LotID))
                    {
                        LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading] - Carrier ID = [{carrierID}] - Not In Database. Denied Loading Request.");
                        LoadingLogData($"Carrier ID = [{carrierID}] - Not In Database. Denied Loading Request");
                        EF_Service.EF_InsertEventLog("WARNING", "LOADING", "", "", carrierID, "", UserAccount.UserName, "Carrier ID Not In Database. Denied Loading Request");
                        AlarmService.DefineAlarm(1001); //Warning
                    }
                    else if (isJobPurged)
                    {
                        LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading] - Carrier ID = [{carrierID}] - Lot Is Purged. Denied Loading Request.");
                        LoadingLogData($"Carrier ID = [{carrierID}] - Lot Is Purged. Denied Loading Request");
                        EF_Service.EF_InsertEventLog("WARNING", "LOADING", "", "", carrierID, "", UserAccount.UserName, "Lot Is Purged. Denied Loading Request");
                        AlarmService.DefineAlarm(1006); //Warning
                    }
                    else
                    {
                        if (tmp_job.Count <= 0)
                        {
                            isEnableScan = true;
                        }
                        else if (tmp_job.Count > 0 && tmp_job.Any(x => x.JobID.Equals(JobID) && x.LotNo.Equals(LotID)))
                        {
                            isEnableScan = true;
                        }
                        else
                        {
                            LoadingLogData($"Carrier ID = [{carrierID}], Lot ID = [{LotID}] - Carrier Scanned");
                            EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrierID, "", UserAccount.UserName, "Carrier Scanned");
                            LoadingLogData($"Finish The Lots With Loading Time Exceeded First. Denied Loading Request");
                            LoadingLogData($"Lot ID = [{tmp_job.First().LotNo}] - Loading Time Exceeded.");
                            EF_Service.EF_InsertEventLog("WARNING", "LOADING", "", "", carrierID, "", UserAccount.UserName, "Finish The Lots Loading Time Exceeded First. Denied Loading Request");
                            EF_Service.EF_InsertEventLog("WARNING", "LOADING", "", tmp_job.First().LotNo, "", "", UserAccount.UserName, "Loading Time Exceeded");
                        }

                        if (isEnableScan)
                        {
                            //if (LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.LoadProcess)).Equals(0) && LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).Equals(0) && LoadPortModels.Count(x => x.LoadPortError).Equals(0))
                            if (LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).Equals(0) && LoadPortModels.Count(x => x.LoadPortError).Equals(0)) //Third FAT
                            {
                                LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading isEnableScan] - No loadports are in <UnloadProcess> and no loadports in <Error> state.");
                                AlarmService.ClearAlarm(); //Reset the error or warning

                                bool isFirstCarrier = false;
                                List<JOBDETAILS> carrierList = EF_Service.EF_GetCarrierListFromJobID(JobID, ref isFirstCarrier);

                                if (carrierList.Find(x => x.CarrierID.Equals(carrierID) && x.JobID.Equals(JobID) && x.LotNo.Equals(LotID)).CarrierStatus.Equals(JobStatus.Initialize) || carrierList.Find(x => x.CarrierID.Equals(carrierID) && x.JobID.Equals(JobID) && x.LotNo.Equals(LotID)).CarrierStatus.Equals(JobStatus.Loading))
                                {
                                    if (isFirstCarrier)
                                    {
                                        LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading isEnableScan] - Carrier ID = [{carrierID}], Lot ID = [{LotID}] - First carrier scanned.");
                                        LoadingLogData($"Carrier ID = [{carrierID}], Lot ID = [{LotID}] - First carrier scanned");
                                        EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrierID, "", UserAccount.UserName, "First Carrier Scanned");

                                        if (PreAssignTrayToLoadPort(carrierList))
                                        {
                                            //Sent Job Queued
                                            //await SecsGemService.SendEventSecsGemAsync(1060, $"{JobID},Queued,{LotID}", 104);
                                            SecsGemService.SendJobTracker(JobID, LotID, $"Queued", carrierID).Wait(200);

                                            EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", JobID, LotID, "", "", UserAccount.UserName, "Sent Event 104-JobQueued");

                                            //Sent Job Selected
                                            //await SecsGemService.SendEventSecsGemAsync(1060, $"{JobID},Selected,{LotID}", 104);
                                            SecsGemService.SendJobTracker(JobID, LotID, $"Selected", carrierID).Wait(200);

                                            EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", JobID, LotID, "", "", UserAccount.UserName, "Sent Event 104-JobSelected");

                                            SecsGemService.StartJobName = string.Empty;

                                            //Sent Job WaitingForStart
                                            //await SecsGemService.SendEventSecsGemAsync(1060, $"{JobID},WaitingForStart,{LotID}", 104);
                                            SecsGemService.SendJobTracker(JobID, LotID, $"WaitingForStart", carrierID).Wait(200);

                                            EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", JobID, LotID, "", "", UserAccount.UserName, "Sent Event 104-JobWaitingForStart");

                                            string initTime = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);
                                            DateTime timeLimit = DateTime.ParseExact(initTime, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider).AddSeconds(10);
                                            DateTime dt_Now = DateTime.ParseExact(initTime, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);

                                            LoadingLogData($"Waiting For Host");

                                            bool isReceiveStartJob = false;

                                            if (SecsGemService.mGEMPro.SECSProControl.IsPortOpened && ConfigurationManager.AppSettings["BypassStartJob"].Equals("0"))
                                            {
                                                await Task.Run(() =>
                                                {
                                                    GlobalAttributesClass.HMIWaitingForHost = true;

                                                    //Open PopUp
                                                    TogglePopup("Waiting For Host");

                                                    do
                                                    {
                                                        initTime = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);
                                                        dt_Now = DateTime.ParseExact(initTime, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);

                                                        if (DateTime.Compare(dt_Now, timeLimit) > 0)
                                                        {
                                                            SecsGemService.StartJobName = string.Empty;
                                                            break;
                                                        }

                                                        if (!string.IsNullOrEmpty(SecsGemService.StartJobName))
                                                        {
                                                            if (SecsGemService.StartJobName.Equals(JobID))
                                                            {
                                                                isReceiveStartJob = true;
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                LogHelper.General(4, $"{nameof(LoadingCarrierEvent)}| StartJob Command Received but wrong Job ID = [{SecsGemService.StartJobName}]");
                                                                AlarmService.DefineAlarm(1004); //Warning
                                                                SecsGemService.StartJobName = string.Empty;
                                                            }
                                                        }
                                                    }
                                                    while (true);
                                                }).ConfigureAwait(false);

                                                //Close PopUp
                                                TogglePopup();
                                                GlobalAttributesClass.HMIWaitingForHost = false;
                                            }
                                            else
                                            {
                                                LogHelper.General(4, $"{nameof(LoadingCarrierEvent)}| SecsGem Port is not open. Bypass receive STARTJOB Command");
                                                LoadingLogData($"SecsGem Port is not open. Bypass receive STARTJOB Command");
                                                isReceiveStartJob = true;
                                            }

                                            if (!isReceiveStartJob)
                                            {
                                                LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading isEnableScan] - Host Not Response To Event 104-JobWaitingForStart. Denied Loading Request.");
                                                LoadingLogData($"Host Not Response To Event 104-JobWaitingForStart. Denied Loading Request");
                                                EF_Service.EF_InsertEventLog("ERROR", "LOADING", JobID, LotID, "", "", UserAccount.UserName, "Host Not Response To Event 104-JobWaitingForStart");
                                                EF_Service.EF_InsertEventLog("ERROR", "LOADING", JobID, LotID, "", "", UserAccount.UserName, "Removed All Pre-Assigned LoadPort Location For This Job/Lot");
                                                EF_Service.EF_InsertEventLog("ERROR", "LOADING", JobID, LotID, "", "", UserAccount.UserName, "Denied Loading Request");
                                                AlarmService.DefineAlarm(1); //Error
                                                RevertDataLoadPortModel(LotID, JobID);
                                            }
                                            else
                                            {
                                                LoadingLogData($"Received Command From Host");
                                                GlobalAttributesClass.HMINoActiveJob = false;
                                                UpdateDataLoadPortModel();
                                                SecsGemService.StartJobName = string.Empty;
                                                Thread.Sleep(100);

                                                //Assign LD Time Start/End
                                                EF_Service.EF_SetDateTimeSetupJob(JobID, LotID, JobStatus.Loading);
                                                EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, "", "", UserAccount.UserName, "Loading Time Started");

                                                List<LoadPortModel> tempLoadPortModel = LoadPortModels.Where(x => !string.IsNullOrEmpty(x.PreAssignLoad)).ToList();
                                                LoadPortModel loadportreadytoloadModel = tempLoadPortModel.Find(x => x.PreAssignLoad.Equals(carrierID));
                                                string loadportreadytoload = tempLoadPortModel.Find(x => x.PreAssignLoad.Equals(carrierID)).LoadPortID;

                                                LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [{carrierID}]  LoadPort Location = [{loadportreadytoload}]");
                                                LoadingLocationData(new LogValue() { Value = $"[{carrierID}]  LoadPort Location = [{loadportreadytoload}]" });
                                                LoadingLogData($"Carrier ID = [{carrierID}] - LoadPort Location = [{loadportreadytoload}]");

                                                //Sent LoadPortReadyToLoad
                                                //await SecsGemService.SendEventSecsGemAsync(103, $"{loadportreadytoload}", 101);
                                                SecsGemService.SendLoadPortReadyToLoad(loadportreadytoload).Wait(200);

                                                EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", JobID, LotID, carrierID, loadportreadytoload, UserAccount.UserName, "Sent Event 101-LoadPortReadyToLoad");

                                                ptlService.LoadUnloadProcessSetLed(loadportreadytoloadModel, LoadPortState.LoadProcess);
                                                EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrierID, loadportreadytoload, UserAccount.UserName, "LoadPort Ready To Load");

                                            }
                                        }

                                    }
                                    else
                                    {
                                        LoadingLogData($"Carrier ID = [{carrierID}], Lot ID = [{LotID}] - Carrier Scanned");
                                        EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrierID, "", UserAccount.UserName, "Carrier Scanned");

                                        GlobalAttributesClass.HMINoActiveJob = false;
                                        List<LoadPortModel> tempLoadPortModel = LoadPortModels.Where(x => !string.IsNullOrEmpty(x.PreAssignLoad)).ToList();
                                        LoadPortModel loadportreadytoloadModel = tempLoadPortModel.Find(x => x.PreAssignLoad.Equals(carrierID));
                                        string loadportreadytoload = tempLoadPortModel.Find(x => x.PreAssignLoad.Equals(carrierID)).LoadPortID;

                                        LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [{carrierID}]  LoadPort Location = [{loadportreadytoload}]");
                                        LoadingLocationData(new LogValue() { Value = $"[{carrierID}]  LoadPort Location = [{loadportreadytoload}]" });
                                        LoadingLogData($"Carrier ID = [{carrierID}] - LoadPort Location = [{loadportreadytoload}]");

                                        //Sent LoadPortReadyToLoad
                                        //await SecsGemService.SendEventSecsGemAsync(103, $"{loadportreadytoload}", 101);
                                        SecsGemService.SendLoadPortReadyToLoad(loadportreadytoload).Wait(200);

                                        EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", JobID, LotID, carrierID, loadportreadytoload, UserAccount.UserName, "Sent Event 101-LoadPortReadyToLoad");

                                        ptlService.LoadUnloadProcessSetLed(loadportreadytoloadModel, LoadPortState.LoadProcess);
                                        EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrierID, loadportreadytoload, UserAccount.UserName, "LoadPort Ready To Load");
                                    }
                                }
                                else
                                {
                                    LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading isEnableScan] - Carrier ID = [{carrierID}], Lot ID = [{LotID}] - Carrier Multiple Scanned.");
                                    LoadingLogData($"Carrier ID = [{carrierID}], Lot ID = [{LotID}] - Carrier Multiple Scanned");
                                    LoadingLocationData(new LogValue() { Value = $"[{carrierID}]  LoadPort Location = [{carrierList.Find(x => x.CarrierID.Equals(carrierID)).PortLocation}]" });
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", JobID, LotID, carrierID, carrierList.Find(x => x.CarrierID.Equals(carrierID)).PortLocation, UserAccount.UserName, "Carrier Multiple Scanned");
                                }
                            }
                            else
                            {
                                LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading isEnableScan] - Count of loadports in <UnloadProcess>: {LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess))} and Count of loadports in <Error>: {LoadPortModels.Count(x => x.LoadPortError)}.");

                                LoadingLogData($"Carrier ID = [{carrierID}], Lot ID = [{LotID}] - Carrier Scanned");
                                EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrierID, "", UserAccount.UserName, "Carrier Scanned");

                                if (!LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).Equals(0))
                                {
                                    carrierID = LoadPortModels.Find(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).LoadPortCarrierLoad;
                                    string tmp_LotID = EF_Service.EF_GetLotIDFromCarrierID(carrierID);
                                    string tmp_JobID = EF_Service.EF_GetJobIDFromCarrierID(carrierID);

                                    LoadingLogData($"Pending Unloading Process To Complete For Lot ID = [{tmp_LotID}].");
                                    LoadingLogData($"Please open the Unloading Page to complete the Unloading Process. Denied Loading Request");
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", tmp_JobID, tmp_LotID, "", "", UserAccount.UserName, "Pending Unloading Process To Complete");
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", JobID, LotID, txtbox.ToUpper(), "", UserAccount.UserName, "Denied Loading Request");

                                    AlarmService.DefineAlarm(1002); //Warning
                                }
                                else if (!LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.LoadProcess)).Equals(0))
                                {
                                    carrierID = LoadPortModels.Find(x => x.LoadPortState.Equals(LoadPortState.LoadProcess)).PreAssignLoad;
                                    string tmp_LotID = EF_Service.EF_GetLotIDFromCarrierID(carrierID);
                                    string tmp_JobID = EF_Service.EF_GetJobIDFromCarrierID(carrierID);

                                    LoadingLogData($"Pending Loading Process To Complete For Lot ID = [{tmp_LotID}]");
                                    LoadingLogData($"Carrier ID = [{carrierID}] LoadPort Location = [{LoadPortModels.Find(x => x.PreAssignLoad.Equals(carrierID)).LoadPortID}]");
                                    LoadingLocationData(new LogValue() { Value = $"[{carrierID}]  LoadPort Location = [{LoadPortModels.Find(x => x.PreAssignLoad.Equals(carrierID)).LoadPortID}]" });
                                    LogHelper.General(3, $"Pending Loading Process To Complete, [{carrierID}]  LoadPort Location = [{LoadPortModels.Find(x => x.PreAssignLoad.Equals(carrierID)).LoadPortID}]");
                                    EF_Service.EF_InsertEventLog("TRACE", "WARNING", tmp_JobID, tmp_LotID, carrierID, LoadPortModels.Find(x => x.PreAssignLoad.Equals(carrierID)).LoadPortID, UserAccount.UserName, "Pending Loading Process To Complete");
                                    LoadingLogData($"Denied Loading Request");
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", JobID, LotID, txtbox.ToUpper(), "", UserAccount.UserName, "Denied Loading Request");

                                    AlarmService.DefineAlarm(1003); //Warning
                                }
                                else if (!LoadPortModels.Count(x => x.LoadPortError).Equals(0))
                                {
                                    LoadingLogData($"Error Occurs On The LoadPort. Denied Loading Request");
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", "", "", "", $"{LoadPortModels.Select(x => x.LoadPortError).First()}", UserAccount.UserName, "Error Occurs On The LoadPort. Denied Loading Request");
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", JobID, LotID, txtbox.ToUpper(), "", UserAccount.UserName, "Denied Loading Request");
                                    AlarmService.DefineAlarm(1005); //Warning
                                }
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    LogHelper.General(5, ex.ToString());
                }
                finally
                {
                    UpdateDataLoadPortModel();
                }
            }  //load by carrier ID
            else if (GlobalAttributesClass.LoadingMode.Equals(1))  //load by lot ID
            {
                try
                {
                    if (LoadPortModels.Any(x => x.LoadPortState.Equals(LoadPortState.LoadProcess))) //19-12-2024
                    {
                        LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Mode: {GlobalAttributesClass.LoadingMode}] [Loading] - Loadports in <LoadProcess>.");
                        LoadPortModels.FindAll(x => x.LoadPortState.Equals(LoadPortState.LoadProcess)).ToList().ForEach(async y =>
                        {
                            //LoadPortModel loadPortID = LoadPortModels.Find(x => x.LoadPortState.Equals(LoadPortState.LoadProcess));
                            LoadPortModel loadPortID = y;
                            string tmp_carrierID = y.PreAssignLoad;
                            ptlService.LoadUnloadProcessSetLed(loadPortID, LoadPortState.Empty);

                            string tmp_LotID = EF_Service.EF_GetLotIDFromCarrierID(carrierID);
                            string tmp_JobID = EF_Service.EF_GetJobIDFromCarrierID(carrierID);

                            LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading] - Carrier ID = [{tmp_carrierID}], Lot ID = [{tmp_LotID}], Cancel loading process.");
                            LoadingLogData($"Lot ID = [{tmp_LotID}] - Cancel Carrier Loading Process");
                            EF_Service.EF_InsertEventLog("TRACE", "LOADING", tmp_JobID, tmp_LotID, tmp_carrierID, loadPortID.LoadPortID, UserAccount.UserName, "Cancel Carrier Loading Process");

                            //Sent LoadPortEmpty
                            //await SecsGemService.SendEventSecsGemAsync(103, $"{loadPortID.LoadPortID}", 101);
                            SecsGemService.SendLoadPortReadyToLoad(loadPortID.LoadPortID).Wait(200);

                            EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", tmp_JobID, tmp_LotID, tmp_carrierID, loadPortID.LoadPortID, UserAccount.UserName, "Sent Event 101-LoadPortEmpty");
                        });
                    }

                    if (string.IsNullOrEmpty(LotID))
                    {
                        LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading] - Carrier ID = [{carrierID}] - Not In Database. Denied Loading Request.");
                        LoadingLogData($"Carrier ID = [{carrierID}] - Not In Database. Denied Loading Request");
                        EF_Service.EF_InsertEventLog("WARNING", "LOADING", "", "", carrierID, "", UserAccount.UserName, "Carrier ID Not In Database. Denied Loading Request");
                        AlarmService.DefineAlarm(1001); //Warning
                    }
                    else if (isJobPurged)
                    {
                        LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading] - Carrier ID = [{carrierID}] - Lot Is Purged. Denied Loading Request.");
                        LoadingLogData($"Carrier ID = [{carrierID}] - Lot Is Purged. Denied Loading Request");
                        EF_Service.EF_InsertEventLog("WARNING", "LOADING", "", "", carrierID, "", UserAccount.UserName, "Lot Is Purged. Denied Loading Request");
                        AlarmService.DefineAlarm(1006); //Warning
                    }
                    else
                    {
                        if (tmp_job.Count <= 0)
                        {
                            isEnableScan = true;
                        }
                        else if (tmp_job.Count > 0 && tmp_job.Any(x => x.JobID.Equals(JobID) && x.LotNo.Equals(LotID)))
                        {
                            isEnableScan = true;
                        }
                        else
                        {
                            LoadingLogData($"Lot ID = [{LotID}] - Carrier Scanned");
                            EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrierID, "", UserAccount.UserName, "Carrier Scanned");
                            LoadingLogData($"Finish The Lots With Loading Time Exceeded First. Denied Loading Request");
                            LoadingLogData($"Lot ID = [{tmp_job.First().LotNo}] - Loading Time Exceeded.");
                            EF_Service.EF_InsertEventLog("WARNING", "LOADING", "", "", carrierID, "", UserAccount.UserName, "Finish The Lots Loading Time Exceeded First. Denied Loading Request");
                            EF_Service.EF_InsertEventLog("WARNING", "LOADING", "", tmp_job.First().LotNo, "", "", UserAccount.UserName, "Loading Time Exceeded");
                        }

                        if (isEnableScan)
                        {
                            if (LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).Equals(0) && LoadPortModels.Count(x => x.LoadPortError).Equals(0)) //Third FAT
                            {
                                LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading isEnableScan] - No loadports are in <UnloadProcess> and no loadports in <Error> state.");
                                AlarmService.ClearAlarm(); //Reset the error or warning

                                bool isFirstCarrier = false;
                                List<JOBDETAILS> carrierList = EF_Service.EF_GetCarrierListFromJobID(JobID, ref isFirstCarrier);

                                //if (carrierList.Find(x => x.CarrierID.Equals(carrierID) && x.JobID.Equals(JobID) && x.LotNo.Equals(LotID)).CarrierStatus.Equals(JobStatus.Initialize) || carrierList.Find(x => x.CarrierID.Equals(carrierID) && x.JobID.Equals(JobID) && x.LotNo.Equals(LotID)).CarrierStatus.Equals(JobStatus.Loading))
                                if (carrierList.Exists(x => x.JobID.Equals(JobID) && x.LotNo.Equals(LotID) && x.CarrierStatus.Equals(JobStatus.Initialize)) || carrierList.Exists(x => x.JobID.Equals(JobID) && x.LotNo.Equals(LotID) && x.CarrierStatus.Equals(JobStatus.Loading)))
                                {
                                    if (isFirstCarrier)
                                    {
                                        LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading isEnableScan] - Lot ID = [{LotID}] - First carrier scanned.");
                                        LoadingLogData($"Lot ID = [{LotID}] - First carrier scanned");
                                        EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrierID, "", UserAccount.UserName, "First Carrier Scanned");

                                        if (PreAssignTrayToLoadPort(carrierList))
                                        {
                                            //Sent Job Queued
                                            //await SecsGemService.SendEventSecsGemAsync(1060, $"{JobID},Queued,{LotID}", 104);
                                            SecsGemService.SendJobTracker(JobID, LotID, $"Queued", carrierID).Wait(200);

                                            EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", JobID, LotID, "", "", UserAccount.UserName, "Sent Event 104-JobQueued");

                                            //Sent Job Selected
                                            //await SecsGemService.SendEventSecsGemAsync(1060, $"{JobID},Selected,{LotID}", 104);
                                            SecsGemService.SendJobTracker(JobID, LotID, $"Selected", carrierID).Wait(200);

                                            EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", JobID, LotID, "", "", UserAccount.UserName, "Sent Event 104-JobSelected");

                                            SecsGemService.StartJobName = string.Empty;

                                            //Sent Job WaitingForStart
                                            //await SecsGemService.SendEventSecsGemAsync(1060, $"{JobID},WaitingForStart,{LotID}", 104);
                                            SecsGemService.SendJobTracker(JobID, LotID, $"WaitingForStart", carrierID).Wait(200);

                                            EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", JobID, LotID, "", "", UserAccount.UserName, "Sent Event 104-JobWaitingForStart");

                                            string initTime = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);
                                            DateTime timeLimit = DateTime.ParseExact(initTime, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider).AddSeconds(10);
                                            DateTime dt_Now = DateTime.ParseExact(initTime, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);

                                            LoadingLogData($"Waiting For Host");

                                            bool isReceiveStartJob = false;

                                            if (SecsGemService.mGEMPro.SECSProControl.IsPortOpened && ConfigurationManager.AppSettings["BypassStartJob"].Equals("0"))
                                            {
                                                await Task.Run(() =>
                                                {
                                                    GlobalAttributesClass.HMIWaitingForHost = true;
                                                    TogglePopup("Waiting For Host");
                                                    do
                                                    {
                                                        initTime = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);
                                                        dt_Now = DateTime.ParseExact(initTime, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);

                                                        if (DateTime.Compare(dt_Now, timeLimit) > 0)
                                                        {
                                                            SecsGemService.StartJobName = string.Empty;
                                                            break;
                                                        }

                                                        if (!string.IsNullOrEmpty(SecsGemService.StartJobName))
                                                        {
                                                            if (SecsGemService.StartJobName.Equals(JobID))
                                                            {
                                                                isReceiveStartJob = true;
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                LogHelper.General(4, $"{nameof(LoadingCarrierEvent)}| StartJob Command Received but wrong Job ID = [{SecsGemService.StartJobName}]");
                                                                AlarmService.DefineAlarm(1004); //Warning
                                                                SecsGemService.StartJobName = string.Empty;
                                                            }
                                                        }
                                                    }
                                                    while (true);
                                                }).ConfigureAwait(false);
                                                TogglePopup();
                                                GlobalAttributesClass.HMIWaitingForHost = false;
                                            }
                                            else
                                            {
                                                LogHelper.General(4, $"{nameof(LoadingCarrierEvent)}| SecsGem Port is not open. Bypass receive STARTJOB Command");
                                                LoadingLogData($"SecsGem Port is not open. Bypass receive STARTJOB Command");
                                                isReceiveStartJob = true;
                                            }

                                            if (!isReceiveStartJob)
                                            {
                                                LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading isEnableScan] - Host Not Response To Event 104-JobWaitingForStart. Denied Loading Request.");
                                                LoadingLogData($"Host Not Response To Event 104-JobWaitingForStart. Denied Loading Request");
                                                EF_Service.EF_InsertEventLog("ERROR", "LOADING", JobID, LotID, "", "", UserAccount.UserName, "Host Not Response To Event 104-JobWaitingForStart");
                                                EF_Service.EF_InsertEventLog("ERROR", "LOADING", JobID, LotID, "", "", UserAccount.UserName, "Removed All Pre-Assigned LoadPort Location For This Job/Lot");
                                                EF_Service.EF_InsertEventLog("ERROR", "LOADING", JobID, LotID, "", "", UserAccount.UserName, "Denied Loading Request");
                                                AlarmService.DefineAlarm(1); //Error
                                                RevertDataLoadPortModel(LotID, JobID);
                                            }
                                            else
                                            {
                                                LoadingLogData($"Received Command From Host");
                                                GlobalAttributesClass.HMINoActiveJob = false;
                                                UpdateDataLoadPortModel();
                                                SecsGemService.StartJobName = string.Empty;
                                                Thread.Sleep(100);

                                                //Assign LD Time Start/End
                                                EF_Service.EF_SetDateTimeSetupJob(JobID, LotID, JobStatus.Loading);
                                                EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, "", "", UserAccount.UserName, "Loading Time Started");

                                                List<LoadPortModel> tempLoadPortModel = LoadPortModels.Where(x => !string.IsNullOrEmpty(x.PreAssignLoad)).ToList();
                                                carrierList.ForEach(carrier =>
                                                {
                                                    LoadPortModel loadportreadytoloadModel = tempLoadPortModel.Find(x => x.PreAssignLoad.Equals(carrier.CarrierID) && x.LoadPortState.Equals(LoadPortState.Empty));
                                                    if (loadportreadytoloadModel != null)
                                                    {
                                                        string loadportreadytoload = tempLoadPortModel.Find(x => x.PreAssignLoad.Equals(carrier.CarrierID)).LoadPortID;

                                                        LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [{LotID}]  LoadPort Location = [{loadportreadytoload}]");
                                                        LoadingLocationData(new LogValue() { Value = $"[{LotID}]  LoadPort Location = [{loadportreadytoload}]" });
                                                        LoadingLogData($"Lot ID = [{LotID}] - LoadPort Location = [{loadportreadytoload}]");

                                                        //Sent LoadPortReadyToLoad
                                                        //await SecsGemService.SendEventSecsGemAsync(103, $"{loadportreadytoload}", 101);
                                                        SecsGemService.SendLoadPortReadyToLoad(loadportreadytoload).Wait(200);

                                                        EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", JobID, LotID, carrier.CarrierID, loadportreadytoload, UserAccount.UserName, "Sent Event 101-LoadPortReadyToLoad");

                                                        ptlService.LoadUnloadProcessSetLed(loadportreadytoloadModel, LoadPortState.LoadProcess);
                                                        EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrier.CarrierID, loadportreadytoload, UserAccount.UserName, "LoadPort Ready To Load");
                                                    }
                                                });


                                            }
                                        }

                                    }
                                    else
                                    {
                                        LoadingLogData($"Lot ID = [{LotID}] - Carrier Scanned");
                                        EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrierID, "", UserAccount.UserName, "Carrier Scanned");

                                        GlobalAttributesClass.HMINoActiveJob = false;
                                        List<LoadPortModel> tempLoadPortModel = LoadPortModels.Where(x => !string.IsNullOrEmpty(x.PreAssignLoad)).ToList();
                                        carrierList.ForEach(carrier =>
                                        {
                                            LoadPortModel loadportreadytoloadModel = tempLoadPortModel.Find(x => x.PreAssignLoad.Equals(carrier.CarrierID) && x.LoadPortState.Equals(LoadPortState.Empty));
                                            if (loadportreadytoloadModel != null)
                                            {
                                                string loadportreadytoload = tempLoadPortModel.Find(x => x.PreAssignLoad.Equals(carrier.CarrierID)).LoadPortID;

                                                LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [{LotID}]  LoadPort Location = [{loadportreadytoload}]");
                                                LoadingLocationData(new LogValue() { Value = $"[{LotID}]  LoadPort Location = [{loadportreadytoload}]" });
                                                LoadingLogData($"Lot ID = [{LotID}] - LoadPort Location = [{loadportreadytoload}]");

                                                //Sent LoadPortReadyToLoad
                                                //await SecsGemService.SendEventSecsGemAsync(103, $"{loadportreadytoload}", 101);
                                                SecsGemService.SendLoadPortReadyToLoad(loadportreadytoload).Wait(200);

                                                EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", JobID, LotID, carrier.CarrierID, loadportreadytoload, UserAccount.UserName, "Sent Event 101-LoadPortReadyToLoad");

                                                ptlService.LoadUnloadProcessSetLed(loadportreadytoloadModel, LoadPortState.LoadProcess);
                                                EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrier.CarrierID, loadportreadytoload, UserAccount.UserName, "LoadPort Ready To Load");
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading isEnableScan] - Lot ID = [{LotID}] - Please Scan Another Carrier.");
                                    LoadingLogData($"Lot ID = [{LotID}] - Please Scan Another Carrier");
                                    LoadingLocationData(new LogValue() { Value = $"[{carrierID}]  LoadPort Location = [{carrierList.Find(x => x.CarrierID.Equals(carrierID)).PortLocation}]" });
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", JobID, LotID, carrierID, carrierList.Find(x => x.CarrierID.Equals(carrierID)).PortLocation, UserAccount.UserName, "Please Scan Another Carrier");
                                }
                            }
                            else
                            {
                                LogHelper.General(3, $"{nameof(LoadingCarrierEvent)}| [Loading isEnableScan] - Lot ID = [{LotID}] - Carrier Scanned.");
                                LoadingLogData($"Lot ID = [{LotID}] - Carrier Scanned");
                                EF_Service.EF_InsertEventLog("TRACE", "LOADING", JobID, LotID, carrierID, "", UserAccount.UserName, "Carrier Scanned");

                                if (!LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).Equals(0))
                                {
                                    carrierID = LoadPortModels.Find(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).LoadPortCarrierLoad;
                                    string tmp_LotID = EF_Service.EF_GetLotIDFromCarrierID(carrierID);
                                    string tmp_JobID = EF_Service.EF_GetJobIDFromCarrierID(carrierID);

                                    LoadingLogData($"Pending Unloading Process To Complete For Lot ID = [{tmp_LotID}].");
                                    LoadingLogData($"Please open the Unloading Page to complete the Unloading Process. Denied Loading Request");
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", tmp_JobID, tmp_LotID, "", "", UserAccount.UserName, "Pending Unloading Process To Complete");
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", JobID, LotID, txtbox.ToUpper(), "", UserAccount.UserName, "Denied Loading Request");

                                    AlarmService.DefineAlarm(1002); //Warning
                                }
                                else if (!LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.LoadProcess)).Equals(0))
                                {
                                    carrierID = LoadPortModels.Find(x => x.LoadPortState.Equals(LoadPortState.LoadProcess)).PreAssignLoad;
                                    string tmp_LotID = EF_Service.EF_GetLotIDFromCarrierID(carrierID);
                                    string tmp_JobID = EF_Service.EF_GetJobIDFromCarrierID(carrierID);

                                    LoadingLogData($"Pending Loading Process To Complete For Lot ID = [{tmp_LotID}]");
                                    LoadingLogData($"Carrier ID = [{carrierID}] LoadPort Location = [{LoadPortModels.Find(x => x.PreAssignLoad.Equals(carrierID)).LoadPortID}]");
                                    LoadingLocationData(new LogValue() { Value = $"[{carrierID}]  LoadPort Location = [{LoadPortModels.Find(x => x.PreAssignLoad.Equals(carrierID)).LoadPortID}]" });
                                    LogHelper.General(3, $"Pending Loading Process To Complete, [{carrierID}]  LoadPort Location = [{LoadPortModels.Find(x => x.PreAssignLoad.Equals(carrierID)).LoadPortID}]");
                                    EF_Service.EF_InsertEventLog("TRACE", "WARNING", tmp_JobID, tmp_LotID, carrierID, LoadPortModels.Find(x => x.PreAssignLoad.Equals(carrierID)).LoadPortID, UserAccount.UserName, "Pending Loading Process To Complete");
                                    LoadingLogData($"Denied Loading Request");
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", JobID, LotID, txtbox.ToUpper(), "", UserAccount.UserName, "Denied Loading Request");

                                    AlarmService.DefineAlarm(1003); //Warning
                                }
                                else if (!LoadPortModels.Count(x => x.LoadPortError).Equals(0))
                                {
                                    LoadingLogData($"Error Occurs On The LoadPort. Denied Loading Request");
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", "", "", "", $"{LoadPortModels.Select(x => x.LoadPortError).First()}", UserAccount.UserName, "Error Occurs On The LoadPort. Denied Loading Request");
                                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", JobID, LotID, txtbox.ToUpper(), "", UserAccount.UserName, "Denied Loading Request");
                                    AlarmService.DefineAlarm(1005); //Warning
                                }
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    LogHelper.General(5, ex.ToString());
                }
                finally
                {
                    UpdateDataLoadPortModel();
                }
            } // load by lot ID
        }

        private bool PreAssignTrayToLoadPort(List<JOBDETAILS> carrierList)
        {
            bool isEnoughLoadPort = true;
            try
            {
                List<string> typecarrier = carrierList.Select(x => x.Type).Distinct().ToList();

                if (typecarrier.Count > 0)
                {
                    typecarrier.ForEach(x =>
                    {
                        int NoOfCarrier = carrierList.Where(y => y.Type.Equals(x)).Count();
                        int NoOfAvailableLoadPort = LoadPortModels.Where(y => y.LoadPortNodeConnectionStatus.Equals(true) && y.LoadPortState.Equals(LoadPortState.Empty) && string.IsNullOrEmpty(y.PreAssignLoad) && y.LoadPortType.Equals(x)).Count();

                        if (NoOfCarrier > NoOfAvailableLoadPort)
                        {
                            isEnoughLoadPort = false;
                            LoadingLogData($"Not Enough Available LoadPort. (Carrier Type = [{x}]) No Of Carrier = [{NoOfCarrier}] and No of Available LoadPort = [{NoOfAvailableLoadPort}]");
                            LogHelper.General(1, $"Not Enough Available LoadPort. (Carrier Type = [{x}]) No Of Carrier = [{NoOfCarrier}] and No of Available LoadPort = [{NoOfAvailableLoadPort}]");
                            EF_Service.EF_InsertEventLog("WARNING", "LOADING", carrierList.First().JobID, carrierList.First().LotNo, "", "", UserAccount.UserName, "Not Enough Available LoadPort");
                            AlarmService.DefineAlarm(3); //Warning
                        }
                    });
                }

                if (isEnoughLoadPort)
                {
                    List<string> loadportlocationPreAssign = new List<string>();
                    int count = 0;

                    foreach (LoadPortModel x in LoadPortModels.OrderBy(x => int.Parse(x.LoadPortPriority)).ToList())
                    {
                        if (x.LoadPortType.Equals(carrierList[count].Type))
                        {
                            if (x.LoadPortNodeConnectionStatus)
                            {
                                if (x.LoadPortIsEnable)
                                {
                                    if (loadportlocationPreAssign.Count().Equals(carrierList.Count()))
                                    {
                                        break;
                                    }
                                    else if(x.LoadPortState.Equals(LoadPortState.Empty) && string.IsNullOrEmpty(x.PreAssignLoad))
                                    {
                                        loadportlocationPreAssign.Add(x.LoadPortID);
                                    } 
                                    else
                                    {
                                        loadportlocationPreAssign.Clear();
                                    }
                                }
                            }
                        }
                    }

                    if (ConfigurationManager.AppSettings["EnhanceCarrierLogic"].Equals("1"))
                    {
                        if (loadportlocationPreAssign.Count().Equals(carrierList.Count()))
                        {
                            bool isProceed = false;
                            string FirstCabinet = loadportlocationPreAssign.First().Split('-').First();
                            string LastCabinet = loadportlocationPreAssign.Last().Split('-').First();
                            LogHelper.General(1, $"First Cabinet = {FirstCabinet}");
                            LogHelper.General(1, $"Last Cabinet = {LastCabinet}");

                            switch (FirstCabinet)
                            {
                                case "AA":
                                    isProceed = LastCabinet == "AA" || LastCabinet == "AB";
                                    break;
                                case "AB":
                                    isProceed = LastCabinet == "AA" || LastCabinet == "AB" || LastCabinet == "AC";
                                    break;
                                case "AC":
                                    isProceed = LastCabinet == "AB" || LastCabinet == "AC" || LastCabinet == "BA";
                                    break;
                                case "BA":
                                    isProceed = LastCabinet == "AC" || LastCabinet == "BA" || LastCabinet == "BB";
                                    break;
                                case "BB":
                                    isProceed = LastCabinet == "BA" || LastCabinet == "BB";
                                    break;
                            }

                            if (!isProceed)
                            {
                                loadportlocationPreAssign = new List<string>();

                                foreach (LoadPortModel x in LoadPortModels.OrderBy(x => x.LoadPortRack).ThenBy(x => int.Parse(x.LoadPortPriority)).ToList())
                                {
                                    if (x.LoadPortType.Equals(carrierList[count].Type))
                                    {
                                        if (x.LoadPortNodeConnectionStatus)
                                        {
                                            if (x.LoadPortIsEnable)
                                            {
                                                if (loadportlocationPreAssign.Count().Equals(carrierList.Count()))
                                                {
                                                    break;
                                                }
                                                else if (x.LoadPortState.Equals(LoadPortState.Empty) && string.IsNullOrEmpty(x.PreAssignLoad))
                                                {
                                                    loadportlocationPreAssign.Add(x.LoadPortID);
                                                }
                                                else
                                                {
                                                    loadportlocationPreAssign.Clear();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        } 
                    }

                    if (loadportlocationPreAssign.Count().Equals(carrierList.Count()))
                    {
                        for (int i = 0; i < carrierList.Count(); i++)
                        {
                            LoadPortModels.Find(x => x.LoadPortID.Equals(loadportlocationPreAssign[i])).PreAssignLoad = carrierList[i].CarrierID;
                            EF_Service.EF_InsertEventLog("TRACE", "LOADING", carrierList.First().JobID, carrierList.First().LotNo, carrierList[i].CarrierID, loadportlocationPreAssign[i], UserAccount.UserName, "Pre-Assigned LoadPort Location");
                            EF_Service.EF_UpdateCarrierStatusAsync(carrierList[i].CarrierID, loadportlocationPreAssign[i], JobStatus.Loading, "");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < carrierList.Count(); i++)
                        {
                            foreach (LoadPortModel x in LoadPortModels.OrderBy(x => int.Parse(x.LoadPortPriority)).ToList())
                            {
                                if (x.LoadPortType.Equals(carrierList[i].Type))
                                {
                                    if (x.LoadPortNodeConnectionStatus)
                                    {
                                        if (x.LoadPortIsEnable)
                                        {
                                            if (x.LoadPortState.Equals(LoadPortState.Empty) && string.IsNullOrEmpty(x.PreAssignLoad))
                                            {
                                                x.PreAssignLoad = carrierList[i].CarrierID;
                                                EF_Service.EF_InsertEventLog("TRACE", "LOADING", carrierList.First().JobID, carrierList.First().LotNo, carrierList[i].CarrierID, x.LoadPortID, UserAccount.UserName, "Pre-Assigned LoadPort Location");
                                                EF_Service.EF_UpdateCarrierStatusAsync(carrierList[i].CarrierID, x.LoadPortID, JobStatus.Loading, "");
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    LoadingLogData($"Denied Loading Request");
                    EF_Service.EF_InsertEventLog("WARNING", "LOADING", carrierList.First().JobID, carrierList.First().LotNo, "", "", UserAccount.UserName, "Denied Loading Request");
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }

            return isEnoughLoadPort;
        }

        public void UpdateDataLoadPortModel()
        {
            EF_Service.EF_UpdateLoadPort(GlobalClassFunction.LoadPortMap(LoadPortModels));
            LogHelper.General(3, $"{nameof(UpdateDataLoadPortModel)}| Data save to database");
        }

        public void RevertDataLoadPortModel(string LotID, string JobID)
        {
            EF_Service.EF_RevertCarrierStatus(JobID, LotID, JobStatus.Initialize);
            EF_Service.EF_UpdateMemory(HMIDBMemory.LOADPORT, nameof(RevertDataLoadPortModel));

            LogHelper.General(3, $"{nameof(RevertDataLoadPortModel)}| Model have been revert with data from database");
        }
        #endregion

        #region Unloading
        public delegate void UpdateUnloadingLogData(string log);
        public static event UpdateUnloadingLogData OnUpdateUnloadingLogData;
        public static void UnloadingLogData(string log)
        {
            try
            {
                if (OnUpdateUnloadingLogData != null)
                {
                    OnUpdateUnloadingLogData(log);
                }
            }
            catch (Exception ex) { LogHelper.General(5, ex.ToString()); }
        }

        public delegate void UpdateUnloadingLocationData(LogValue log);
        public static event UpdateUnloadingLocationData OnUpdateUnloadingLocationData;
        public static void UnloadingLocationData(LogValue log)
        {
            try
            {
                if (OnUpdateUnloadingLocationData != null)
                {
                    OnUpdateUnloadingLocationData(log);
                }
            }
            catch (Exception ex) { LogHelper.General(5, ex.ToString()); }
        }


        public delegate void UpdateUnloadingLogMessageFromPTL(LogValue log);
        public static event UpdateUnloadingLogMessageFromPTL OnUpdateUnloadingLogMessageFromPTL;
        public static void UnloadingLogMessageFromPTL(LogValue log)
        {
            try
            {
                if (OnUpdateUnloadingLogMessageFromPTL != null)
                {
                    OnUpdateUnloadingLogMessageFromPTL(log);
                }
            }
            catch (Exception ex) { LogHelper.General(5, ex.ToString()); }
        }


        public void UnloadingCarrierEvent()
        {
            try
            {
                GlobalAttributesClass.HMINoActiveJob = true;
                string lotID = IDFromInterface.ToUpper();
                string jobID = EF_Service.EF_GetJobIDFromLotID(lotID);

                LogHelper.General(1, $"UNLOADING - Lot Selected {lotID}");

                List<SETUPJOB> tmp_job = EF_Service.EF_GetErrorJobFromCarrierID(JobStatus.StagingTimeExpired);

                bool isEnableScan = false;

                if (!string.IsNullOrEmpty(jobID))
                {
                    UnloadingLogData($"Lot ID = [{lotID}] - Lot Selected/Scanned");
                    EF_Service.EF_InsertEventLog("TRACE", "UNLOADING", jobID, lotID, "", "", UserAccount.UserName, "Lot Selected/Scanned");

                    if (tmp_job.Count <= 0)
                    {
                        isEnableScan = true;
                    }
                    else if (tmp_job.Count > 0 && tmp_job.Any(x => x.JobID.Equals(jobID) && x.LotNo.Equals(lotID)))
                    {
                        isEnableScan = true;
                    }
                    else
                    {
                        isEnableScan = true;
                        //UnloadingLogData($"Finish The Lots With Staging Time Exceeded First. Denied Unloading Request");
                        UnloadingLogData($"Lot ID = [{tmp_job.First().LotNo}] - Staging Time Exceeded.");
                        LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| Lot ID = [{tmp_job.First().LotNo}] - Staging Time Exceeded.");
                        //EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", jobID, lotID, "", "", UserAccount.UserName, "Finish The Lots with Staging Time Exceeded First. Denied Unloading Request");
                        EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", "", tmp_job.First().LotNo, "", "", UserAccount.UserName, "Staging Time Exceeded");
                    }

                    if (isEnableScan)
                    {
                        if (LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.LoadProcess)).Equals(0) /*&& LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).Equals(0)*/ && LoadPortModels.Count(x => x.LoadPortError).Equals(0))
                        {
                            LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [Unload isEnableScan] - No loadports are in <LoadProcess> and no loadports in <Error> state.");
                            AlarmService.ClearAlarm(); //Reset the error or warning

                            bool isFirstCarrier = false;
                            bool isAllowToProceed = true;
                            List<JOBDETAILS> carrierList = EF_Service.EF_GetCarrierListFromJobID(jobID, ref isFirstCarrier);

                            GlobalAttributesClass.HMINoActiveJob = false;

                            if (!LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).Equals(0)) //check any loadport is under UnloadProcess
                            {
                                LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [Unload isEnableScan] - Count of loadports in <UnloadProcess>: {LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess))}");
                                List<LoadPortModel> models = LoadPortModels.Where(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).ToList();
                                string jobid = EF_Service.EF_GetJobIDFromCarrierID(models[0].LoadPortCarrierLoad);
                                List<JOBDETAILS> tmpcarrierList = EF_Service.EF_GetCarrierListFromJobID(jobid, ref isFirstCarrier);

                                foreach (JOBDETAILS x in tmpcarrierList)
                                {
                                    LoadPortModel loadportreadytoloadModel = LoadPortModels.ToList().Find(y => y.LoadPortID.Equals(x.PortLocation));
                                    if (!loadportreadytoloadModel.LoadPortState.Equals(LoadPortState.UnloadProcess)) //check loadport is not under UnloadProcess
                                    {
                                        LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [Unload isAllowToProceed] - CarrierID: {x.CarrierID} / PortLocation: {x.PortLocation} / isAllowToProceed: {isAllowToProceed}");
                                        isAllowToProceed = false;
                                        break;
                                    }
                                }

                                if (isAllowToProceed)
                                {
                                    LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [Unload isEnableScan] - <UnloadProcess> loadports isAllowToProceed: {isAllowToProceed}");
                                    UnloadingLogData($"Lot ID = [{lotID}] - Changing Lot for Unloading");

                                    foreach (JOBDETAILS x in tmpcarrierList)
                                    {
                                        LoadPortModel loadportreadytoloadModel = LoadPortModels.ToList().Find(y => y.LoadPortID.Equals(x.PortLocation));
                                        string loadportreadytounload = x.PortLocation;

                                        LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [{x.CarrierID}]  Changing Lot for Unloading = [{loadportreadytounload}]");

                                        ptlService.LoadUnloadProcessSetLed(loadportreadytoloadModel, LoadPortState.CoolingCompleted);
                                        EF_Service.EF_InsertEventLog("TRACE", "UNLOADING", jobID, lotID, x.CarrierID, loadportreadytounload, UserAccount.UserName, $"Changing Lot for Unloading");
                                    }
                                }
                                else
                                {
                                    UnloadingLogData($"Please Finish The Unloading for Lot ID = [{tmpcarrierList[0].LotNo}]. Denied Changing Lot Request");
                                    EF_Service.EF_InsertEventLog("TRACE", "UNLOADING", tmpcarrierList[0].JobID, tmpcarrierList[0].LotNo, "", "", UserAccount.UserName, $"Please Finish The Unloading for this Lot");
                                    LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [{tmpcarrierList[0].JobID}]  Please Finish The Unloading for this Lot");
                                }
                            }

                            if (isAllowToProceed)
                            {
                                LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [Unload isEnableScan] - isAllowToProceed: {isAllowToProceed}");
                                if (EF_Service.EF_UnloadingStatusCheck(lotID, jobID))
                                {
                                    LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [ReadyToUnload] - LotID: {lotID} / JobID: {jobID}");
                                    UnloadingLogData($"Lot ID = [{lotID}] - No. of Carrier = [{carrierList.Count()}]");
                                    EF_Service.EF_InsertEventLog("TRACE", "UNLOADING", jobID, lotID, "", "", UserAccount.UserName, $"No. of Carrier = [{carrierList.Count()}]");

                                    carrierList.ForEach(x =>
                                    {
                                        LoadPortModel loadportreadytoloadModel = LoadPortModels.ToList().Find(y => y.LoadPortID.Equals(x.PortLocation));
                                        string loadportreadytounload = x.PortLocation;

                                        UnloadingLocationData(new LogValue() { Value = $"[{x.CarrierID}]  LoadPort Location = [{loadportreadytounload}]" });
                                        LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [{x.CarrierID}]  LoadPort Location = [{loadportreadytounload}]");

                                        ptlService.LoadUnloadProcessSetLed(loadportreadytoloadModel, LoadPortState.UnloadProcess);
                                        EF_Service.EF_InsertEventLog("TRACE", "UNLOADING", jobID, lotID, x.CarrierID, loadportreadytounload, UserAccount.UserName, $"LoadPort Ready To Unload");
                                    });
                                }
                                else
                                {
                                    LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [Not ReadyToUnload] - LotID: {lotID} / JobID: {jobID}");
                                    UnloadingLogData($"Lot ID = [{lotID}] - Lot Not Ready For Unloading");
                                    EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", jobID, lotID, "", "", UserAccount.UserName, "Lot Not Ready For Unloading");
                                    UnloadingLogData($"Denied Unloading Request");
                                    EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", jobID, lotID, "", "", UserAccount.UserName, "Denied Unloading Request");
                                    AlarmService.DefineAlarm(1054); //Warning
                                    GlobalAttributesClass.HMINoActiveJob = true;
                                }
                            }
                            else
                            {
                                List<LoadPortModel> models = LoadPortModels.Where(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).ToList();
                                LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [isAllowToProceed = False] - models.Count: {models.Count}");
                                models.ForEach(x =>
                                {
                                    UnloadingLocationData(new LogValue() { Value = $"[{x.LoadPortCarrierLoad}]  LoadPort Location = [{x.LoadPortID}]" });
                                    LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [{x.LoadPortCarrierLoad}]  LoadPort Location = [{x.LoadPortID}]");
                                });
                            }
                        }
                        else
                        {
                            if (!LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.LoadProcess)).Equals(0))
                            {
                                LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [Unload isEnableScan] - Count of loadports in <LoadProcess>: {LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.LoadProcess))}.");

                                string carrierID = LoadPortModels.Find(x => x.LoadPortState.Equals(LoadPortState.LoadProcess)).PreAssignLoad;
                                string LotID = EF_Service.EF_GetLotIDFromCarrierID(carrierID);
                                string JobID = EF_Service.EF_GetJobIDFromCarrierID(carrierID);

                                UnloadingLogData($"Pending Loading Process To Complete For Lot ID = [{LotID}], Carrier ID = [{carrierID}]");
                                UnloadingLogData($"Please open the Loading Page to complete the Loading Process. Denied Unloading Request");
                                EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", JobID, LotID, carrierID, "", UserAccount.UserName, "Pending Loading Process To Complete. Denied Unloading Request");
                                AlarmService.DefineAlarm(1052); //Warning
                            }
                            //else if (!LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).Equals(0))
                            //{
                            //    List<LoadPortModel> models = LoadPortModels.Where(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)).ToList();
                            //    string lotid = EF_Service.EF_GetLotIDFromCarrierID(models[0].LoadPortCarrierLoad);
                            //    string jobid = EF_Service.EF_GetJobIDFromCarrierID(models[0].LoadPortCarrierLoad);

                            //    UpdateLogData($"Pending Unloading Process To Complete for Lot ID = [{lotid}]");
                            //    EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", jobid, lotid, "", "", UserAccount.UserName, "Pending Unloading Process To Complete");
                            //    AlarmService.DefineAlarm(1053); //Warning

                            //    bool isFirstCarrier = false;
                            //    List<JOBDETAILS> carrierList = EF_Service.EF_GetCarrierListFromJobID(jobid, ref isFirstCarrier);

                            //    carrierList = carrierList.Where(x => !x.CarrierStatus.Equals(JobStatus.Complete)).ToList();
                            //    carrierList.ForEach(x =>
                            //    {
                            //        LoadPortModel loadportreadytoloadModel = LoadPortModels.Find(y => y.LoadPortCarrierLoad.Equals(x.CarrierID));
                            //        string loadportreadytounload = LoadPortModels.Find(y => y.LoadPortCarrierLoad.Equals(x.CarrierID)).LoadPortID;

                            //        UnloadPortLocationLogs.Insert(0, new LogValue() { Value = $"[{x.CarrierID}]  LoadPort Location = [{loadportreadytounload}]" });
                            //        LogHelper.General(3, $"Pending Unloading Process To Complete, [{x.CarrierID}]  LoadPort Location = [{loadportreadytounload}]");
                            //        EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", jobid, lotid, x.CarrierID, loadportreadytounload, UserAccount.UserName, "Remaining Carrier For Unloading Process To Complete");
                            //    });

                            //    UpdateLogData($"Denied Unloading Request");
                            //    EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", jobID, lotID, "", "", UserAccount.UserName, "Denied Unloading Request");
                            //} 
                            else if (!LoadPortModels.Count(x => x.LoadPortError).Equals(0))
                            {
                                LogHelper.General(3, $"{nameof(UnloadingCarrierEvent)}| [Unload isEnableScan] - Count of loadports in <Error>: {LoadPortModels.Count(x => x.LoadPortError)}.");

                                UnloadingLogData($"Error Occurs On The LoadPort. Denied Unloading Request");
                                EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", jobID, lotID, "", $"{LoadPortModels.Select(x => x.LoadPortError).First()}", UserAccount.UserName, "Error Occurs On The LoadPort. Denied Unloading Request");
                                AlarmService.DefineAlarm(1055); //Warning
                            }
                        }
                    }
                }
                else
                {
                    UnloadingLogData($"Lot ID = [{lotID}] - Not Assigned To Any Job/Lot");
                    UnloadingLogData($"Denied Unloading Request");
                    EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", "", lotID, "", "", UserAccount.UserName, "Lot Not Assigned To Any Job");
                    EF_Service.EF_InsertEventLog("WARNING", "UNLOADING", "", lotID, "", "", UserAccount.UserName, "Denied Unloading Request");
                    AlarmService.DefineAlarm(1051); //Warning
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.ToString());
            }
            finally
            {
                UpdateDataLoadPortModel();
            }
        } 
        #endregion
    }
}
