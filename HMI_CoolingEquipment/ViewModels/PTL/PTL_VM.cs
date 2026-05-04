using ControlzEx.Standard;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GEMPro300.Model;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace HMI_CoolingEquipment.ViewModels
{
    public class PTL_VM : ViewModelBase
    {
        public ICommand ShowAllAddress { get; set; }
        public ICommand SetBestPollingRangeCommand { get; set; }
        public ICommand EnableLedTestingCommand { get; set; }
        public ICommand DisableLedTestingCommand { get; set; }
        public ICommand EnableSensorTestingCommand { get; set; }
        public ICommand DisableSensorTestingCommand { get; set; }
        public ICommand SaveLoadPortConfigCommand { get; set; }
        PTLService ptlService;

        private List<PTLConnectionInfo> connectionInfos = new List<PTLConnectionInfo>();
        public List<PTLConnectionInfo> ConnectionInfos
        {
            get { return connectionInfos; }
            set
            {
                if (connectionInfos != value)
                {
                    connectionInfos = value;
                    RaisePropertyChanged(nameof(ConnectionInfos));
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

        public PTL_VM()
        {
            Messenger.Default.Register<List<PTLConnectionInfo>>(this, UpdatePTLConnectionInfo);
            Messenger.Default.Register<List<LoadPortModel>>(this, UpdateLoadPortModels);
            Messenger.Default.Register<List<LoadPortConfigurationModel>>(this, UpdateLoadPortConfig);
            ShowAllAddress = new RelayCommand<object>(ShowAllAddressEvent);
            SetBestPollingRangeCommand = new RelayCommand(SetBestPollingRangeCommandEvent);
            EnableLedTestingCommand = new RelayCommand<object>(EnableLedTestingCommandEvent);
            DisableLedTestingCommand = new RelayCommand(DisableLedTestingCommandEvent);
            EnableSensorTestingCommand = new RelayCommand(EnableSensorTestingCommandEvent);
            DisableSensorTestingCommand = new RelayCommand(DisableSensorTestingCommandEvent);
            SaveLoadPortConfigCommand = new RelayCommand(SaveLoadPortConfigCommandEvent);
            InitializePTL();
        }

        private async void InitializePTL()
        {
            await Task.Delay(1000);

            ptlService = new PTLService();
            Messenger.Default.Send(ptlService);
        }

        private void ConnectPTLCommandEvent(PTLConnectionInfo info)
        {
            ptlService.ConnectToPTL(info.GatewayID);
            ptlService.ResetLedState();

            MessageBox.Show("Connect Done", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void DisconnectPTLCommandEvent(PTLConnectionInfo info)
        {
            ptlService.DisconnectFromPTL(info.GatewayID);
            MessageBox.Show("Disconnect Done", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetBestPollingRangeCommandEvent()
        {
            ptlService.SetDeviceBestPollRange();
            MessageBox.Show("Set Best Polling Range Done", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EnableLedTestingCommandEvent(object param)
        {
            ptlService.EnableLightLEDTesting(int.Parse(param.ToString()));
            MessageBox.Show("LED light testing Done", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void DisableLedTestingCommandEvent()
        {
            ptlService.DisableLightLEDTesting();
        }
        
        private void EnableSensorTestingCommandEvent()
        {
            ptlService.EnableSensorTesting();
            MessageBox.Show("Sensor Testing Enable", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void DisableSensorTestingCommandEvent()
        {
            MessageBox.Show("Sensor Testing Disable", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            ptlService.DisableSensorTesting();
        }
        
        private void SaveLoadPortConfigCommandEvent()
        {
            LoadPortConfig.ForEach(x =>
            {
                if (x.LoadPortLEDColour.Equals(LEDColour.NoColour))
                {
                    x.LoadPortLEDState = LEDState.NoColour;
                }
                else if (!x.LoadPortLEDColour.Equals(LEDColour.NoColour) && x.LoadPortLEDState.Equals(LEDState.NoColour))
                {
                    x.LoadPortLEDState = LEDState.NoBlinking;
                }
            });
            EF_Service.EF_UpdateLoadPortConfig(GlobalClassFunction.LoadPortConfigurationMap(LoadPortConfig));
            MessageBox.Show("PTL LED Configuration Saved", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowAllAddressEvent(object param)
        {
            try
            {
                switch (param)
                {
                    case "Shows":
                        ConnectionInfos.ForEach(x =>
                        {
                            if (x.GatewayConnectionCode.Equals(7))
                            {
                                PTLUtils.AB_LB_DspAddr(x.GatewayID, -252);
                            }
                        });
                        MessageBox.Show("Shows all address done", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case "Clear":
                        ConnectionInfos.ForEach(x =>
                        {
                            if (x.GatewayConnectionCode.Equals(7))
                            {
                                PTLUtils.AB_LB_DspNum(x.GatewayID, -252, 0, 0, -3);
                                PTLUtils.AB_AHA_ClrDsp(x.GatewayID, -252); //clear AHA Display
                            }
                        });
                        ptlService.ResetLedState();
                        MessageBox.Show("Hide all address done", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }
        }

        private void UpdatePTLConnectionInfo(List<PTLConnectionInfo> connectionInfos)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if(ConnectionInfos.Count <= 0)
                {
                    ConnectionInfos = connectionInfos;
                    ConnectionInfos.ForEach(x => x.ConnectPTLCommand = new RelayCommand<PTLConnectionInfo>(ConnectPTLCommandEvent));
                    ConnectionInfos.ForEach(x => x.DisconnectPTLCommand = new RelayCommand<PTLConnectionInfo>(DisconnectPTLCommandEvent));
                }
                else
                {
                    ConnectionInfos.ForEach(x =>
                    {
                        PTLConnectionInfo tmp = connectionInfos.Find(y => y.GatewayIP.Equals(x.GatewayIP));

                        x.GatewayID = tmp.GatewayID;
                        x.GatewayIP = tmp.GatewayIP;
                        x.GatewayPort = tmp.GatewayPort;
                        x.GatewayConnectionCode = tmp.GatewayConnectionCode;
                        x.GatewayConnectionValue = tmp.GatewayConnectionValue;
                    });
                }
            }));
        }

        private void UpdateLoadPortModels(List<LoadPortModel> loadportmodels)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if(LoadPortModels.Count <= 0)
                {
                    LoadPortModels = loadportmodels;
                    LoadPortModels.ForEach(x =>
                    {
                        x.LoadPortIsEnableCommand = new RelayCommand<LoadPortModel>(UpdateLoadPortDB);
                    });
                }
                else
                {
                    LoadPortModels.ForEach(x =>
                    {
                        LoadPortModel tmp = loadportmodels.Find(y => y.LoadPortID.Equals(x.LoadPortID));

                        x.LoadPortID = tmp.LoadPortID;
                        x.LoadPortRack = tmp.LoadPortRack;
                        x.LoadPortColumn = tmp.LoadPortColumn;
                        x.LoadPortRow = tmp.LoadPortRow;
                        x.LoadPortType = tmp.LoadPortType;
                        x.LoadPortCarrierLoad = tmp.LoadPortCarrierLoad;
                        x.LoadPortIPAddress = tmp.LoadPortIPAddress;
                        x.LoadPortNodeAddress = tmp.LoadPortNodeAddress;
                        x.LoadPortNodeConnectionStatus = tmp.LoadPortNodeConnectionStatus;
                        x.LoadPortError = tmp.LoadPortError;
                        x.LoadPortState = tmp.LoadPortState;
                        x.PreAssignLoad = tmp.PreAssignLoad;
                        x.LoadPortPriority = tmp.LoadPortPriority;
                        x.LoadPortIsEnable = tmp.LoadPortIsEnable;
                    });
                }
            }));
        }

        private async void UpdateLoadPortDB(LoadPortModel loadportmodel)
        {
            string LoadPortEnabled = loadportmodel.LoadPortIsEnable ? "Enabled" : "Disabled";
            LogHelper.General(2, $"Location {loadportmodel.LoadPortID} Button Pressed: {LoadPortEnabled}");

            try
            {
                LoadPortModel tmpModel = LoadPortModels.Find(x => x.LoadPortID.Equals(loadportmodel.LoadPortID));
                if ((tmpModel.LoadPortState.Equals(LoadPortState.Empty) || tmpModel.LoadPortState.Equals(LoadPortState.Disabled) || tmpModel.LoadPortState.Equals(LoadPortState.Abort)) && string.IsNullOrEmpty(tmpModel.PreAssignLoad))
                {
                    loadportmodel.LoadPortState = loadportmodel.LoadPortIsEnable ? LoadPortState.Empty : LoadPortState.Disabled;

                    LOADPORT tmp = new LOADPORT()
                    {
                        LoadPortID = loadportmodel.LoadPortID,
                        LoadPortRack = loadportmodel.LoadPortRack,
                        LoadPortColumn = loadportmodel.LoadPortColumn,
                        LoadPortRow = loadportmodel.LoadPortRow,
                        LoadPortType = loadportmodel.LoadPortType,
                        LoadPortCarrierLoad = loadportmodel.LoadPortCarrierLoad,
                        LoadPortIPAddress = loadportmodel.LoadPortIPAddress,
                        LoadPortNodeAddress = loadportmodel.LoadPortNodeAddress,
                        LoadPortNodeConnectionStatus = loadportmodel.LoadPortNodeConnectionStatus,
                        LoadPortError = loadportmodel.LoadPortError,
                        LoadPortState = loadportmodel.LoadPortIsEnable ? LoadPortState.Empty : LoadPortState.Disabled,
                        PreAssignLoad = loadportmodel.PreAssignLoad,
                        LoadPortPriority = loadportmodel.LoadPortPriority,
                        LoadPortIsEnable = loadportmodel.LoadPortIsEnable,
                    };

                    LoadPort_VM.LoadPortModelData(loadportmodel);
                    ptlService.LoadUnloadProcessSetLed(loadportmodel, loadportmodel.LoadPortState);

                    await Task.Run(() =>
                    {
                        EF_Service.EF_UpdateLoadPort(tmp);

                    }).ConfigureAwait(false);
                    
                    LogHelper.General(2, $"Location {loadportmodel.LoadPortID} Button Updated: {LoadPortEnabled}");
                }
                else
                {
                    MessageBox.Show($"Loadport have carrier/pre-assign", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                LogHelper.PTL(5, ex.Message);
            }
            finally
            {
                EF_Service.EF_UpdateMemory(HMIDBMemory.LOADPORT, nameof(UpdateLoadPortDB));
            }
        }

        private void UpdateLoadPortConfig(List<LoadPortConfigurationModel> loadportConfigModel)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    LoadPortConfig = loadportConfigModel;
                    LoadPortConfig.ForEach(x =>
                    {
                        x.ComboBoxValues = Enum.GetNames(typeof(LEDColour)).ToList();
                    });
                }));
            }
            catch (Exception ex)
            {
                LogHelper.PTL(5, ex.Message);
            }
        }
    }
}
