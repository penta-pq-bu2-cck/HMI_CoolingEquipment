using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using HMI_CoolingEquipment.Views;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Input;

namespace HMI_CoolingEquipment.ViewModels
{
    public class SecsGem_VM : ViewModelBase
    {
        public ICommand OpenSecsGemSettingsCommand { get; set; }
        public ICommand BtnConnectionCommand { get; set; }
        public ICommand BtnSentEventTesterCommand { get; set; }
        public ICommand IconCEIDCommand { get; set; }
        public ICommand IconVarIDCommand { get; set; }

        private ObservableCollection<LogValue> _secsGemLog = new ObservableCollection<LogValue>();
        public ObservableCollection<LogValue> SecsGemLog
        {
            get { return _secsGemLog; }
            set
            {
                _secsGemLog = value;
                RaisePropertyChanged(nameof(SecsGemLog));
            }
        }

        private SecsGemConnectionValue _secsGemConnection = new SecsGemConnectionValue();
        public SecsGemConnectionValue SecsGemConnection
        {
            get { return _secsGemConnection; }
            set
            {
                if( _secsGemConnection != value)
                {
                    _secsGemConnection = value;
                    RaisePropertyChanged(nameof(SecsGemConnection));
                }
            }
        }
        
        private Visibility _secsGemConnectionBtn = Visibility.Collapsed;
        public Visibility SecsGemConnectionBtn
        {
            get { return _secsGemConnectionBtn; }
            set
            {
                if( _secsGemConnectionBtn != value)
                {
                    _secsGemConnectionBtn = value;
                    RaisePropertyChanged(nameof(SecsGemConnectionBtn));
                }
            }
        }

        private Visibility _connectBtn = Visibility.Collapsed;
        public Visibility ConnectBtn
        {
            get { return _connectBtn; }
            set
            {
                if (_connectBtn != value)
                {
                    _connectBtn = value;
                    RaisePropertyChanged(nameof(ConnectBtn));
                }
            }
        }

        private Visibility _disConnectBtn = Visibility.Collapsed;
        public Visibility DisconnectBtn
        {
            get { return _disConnectBtn; }
            set
            {
                if (_disConnectBtn != value)
                {
                    _disConnectBtn = value;
                    RaisePropertyChanged(nameof(DisconnectBtn));
                }
            }
        }

        private Visibility _cancelBtn = Visibility.Collapsed;
        public Visibility CancelBtn
        {
            get { return _cancelBtn; }
            set
            {
                if (_cancelBtn != value)
                {
                    _cancelBtn = value;
                    RaisePropertyChanged(nameof(CancelBtn));
                }
            }
        }

        private bool _connectBtnIsEnable = false;
        public bool ConnectBtnIsEnable
        {
            get { return _connectBtnIsEnable; }
            set
            {
                if (_connectBtnIsEnable != value)
                {
                    _connectBtnIsEnable = value;
                    RaisePropertyChanged(nameof(ConnectBtnIsEnable));
                }
            }
        }

        public string _variableID = string.Empty;
        public string VariableID
        {
            get { return _variableID; }
            set
            {
                if( _variableID != value )
                {
                    _variableID = value;
                    RaisePropertyChanged(nameof(VariableID));
                }
            }
        }
        
        public string _variableValue = string.Empty;
        public string VariableValue
        {
            get { return _variableValue; }
            set
            {
                if( _variableValue != value )
                {
                    _variableValue = value;
                    RaisePropertyChanged(nameof(VariableValue));
                }
            }
        }
        
        public string _cEID = string.Empty;
        public string CEID
        {
            get { return _cEID; }
            set
            {
                if( _cEID != value )
                {
                    _cEID = value;
                    RaisePropertyChanged(nameof(CEID));
                }
            }
        }

        public SecsGem_VM()
        {
            Messenger.Default.Register<LogValue>(this, UpdateSecsLog);
            Messenger.Default.Register<SecsGemConnectionValue>(this, UpdateSecsConnection);
            Messenger.Default.Register<SecsGemCusEvent>(this, UpdateSecsGemCusEvent);
            Messenger.Default.Register<SecsGemCusVar>(this, UpdateSecsGemCusVariable);
            OpenSecsGemSettingsCommand = new RelayCommand(OpenSecsGemSettingsCommandEvent);
            BtnConnectionCommand = new RelayCommand<object>(BtnConnectionCommandEvent);
            BtnSentEventTesterCommand = new RelayCommand<object>(BtnSentEventTesterCommandEvent);
            IconCEIDCommand = new RelayCommand(IconCEIDCommandEvent);
            IconVarIDCommand = new RelayCommand(IconVarIDCommandEvent);
            SecsGemService.InitializeSecsGem();

            if (ConfigurationManager.AppSettings["EnableAutoConnectSecsGem"].Equals("1"))
            {
                SecsGemService.ConnectSecsGem();
            }
            else
            {
                SecsGemConnectionBtn = Visibility.Visible;
                if (SecsGemConnection.IsConnected)
                {
                    DisconnectBtn = Visibility.Visible;
                    ConnectBtn = Visibility.Collapsed;
                    CancelBtn = Visibility.Collapsed;
                    ConnectBtnIsEnable = true;
                }
                else
                {
                    DisconnectBtn = Visibility.Collapsed;
                    ConnectBtn = Visibility.Visible;
                    CancelBtn = Visibility.Collapsed;
                    ConnectBtnIsEnable = true;
                }
            }
            
        }

        private void IconVarIDCommandEvent()
        {
            SecsGemCusVarWindow secsGemCusVarWindow = new SecsGemCusVarWindow(new SecsGemCusVar_VM());
            secsGemCusVarWindow.Show();
        }
        
        private void IconCEIDCommandEvent()
        {
            SecsGemCusEventWindow secsGemCusEventWindow = new SecsGemCusEventWindow(new SecsGemCusEvent_VM());
            secsGemCusEventWindow.Show();
        }

        private void BtnSentEventTesterCommandEvent(object button)
        {
            //SecsGemService.SendEventSecsGem(0, "Test", 0);
            //if (button.Equals("Clear"))
            //{
            //    VariableID = string.Empty;
            //    VariableValue = string.Empty;
            //    CEID = string.Empty;
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(VariableID) && !string.IsNullOrEmpty(VariableValue) && !string.IsNullOrEmpty(CEID))
            //    {
            //        if (int.TryParse(VariableID, out int varID) && int.TryParse(CEID, out int ceid))
            //        {
            //            if (SecsGemService.SendEventSecsGem(varID, VariableValue, ceid))
            //            {
            //                MessageBox.Show("Event successfully sent", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            //                VariableID = string.Empty;
            //                VariableValue = string.Empty;
            //                CEID = string.Empty;
            //            }
            //            else
            //            {
            //                MessageBox.Show("There is an error on sending Event. Please check the SecsGem connection", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("Variable ID and CEID should be in number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Don't leave it blank", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}

        }

        private void BtnConnectionCommandEvent(object button)
        {
            if (button.Equals("Connect"))
            {
                SecsGemService.ConnectSecsGem();
                CancelBtn = Visibility.Visible;
                ConnectBtnIsEnable = false;
            }
            else if(button.Equals("Disconnect"))
            {
                SecsGemService.DisconnectSecsGem();
            }
            else
            {
                SecsGemService.CancelConnectionSecsGem();
                CancelBtn = Visibility.Collapsed;
                ConnectBtnIsEnable = true;
            }

        }

        private void OpenSecsGemSettingsCommandEvent()
        {
            SecsGemService.OpenSettingsSecsGem();
        }

        private void UpdateSecsConnection(SecsGemConnectionValue value)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SecsGemConnection = value;

                if (SecsGemConnection.IsConnected)
                {
                    DisconnectBtn = Visibility.Visible;
                    ConnectBtn = Visibility.Collapsed;
                    CancelBtn = Visibility.Collapsed;
                    ConnectBtnIsEnable = true;
                }
                else
                {
                    DisconnectBtn = Visibility.Collapsed;
                    ConnectBtn = Visibility.Visible;
                    CancelBtn = Visibility.Collapsed;
                    ConnectBtnIsEnable = true;
                }
            }));
        }

        private void UpdateSecsGemCusVariable(SecsGemCusVar value)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                VariableID = value.VarID;
            }));
        }

        private void UpdateSecsGemCusEvent(SecsGemCusEvent value)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                CEID = value.EventID;
            }));
        }

        private void UpdateSecsLog(LogValue value)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (value.Page.Equals(HMIPage.SecsGemPage))
                {
                    if (SecsGemLog.Count < 50)
                    {
                        SecsGemLog.Insert(0, value);
                    }
                    else
                    {
                        SecsGemLog.RemoveAt(49);
                        SecsGemLog.Insert(0, value);
                    } 
                }
            }));
        }
    }
}
