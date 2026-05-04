using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace HMI_CoolingEquipment.ViewModels
{
    public class LoadingLoadPort_VM : ViewModelBase
    {
        private PTLService ptlService;
        private int errorLogCounts = 0;
        private Thread LoadThread;

        public ICommand ScanCarrierNoCommand { get; set; }

        private bool _loadBtnEnable = true;
        public bool LoadBtnEnable
        {
            get { return _loadBtnEnable; }
            set
            {
                if (_loadBtnEnable != value)
                {
                    _loadBtnEnable = value;
                    RaisePropertyChanged(nameof(LoadBtnEnable));
                }
            }
        }

        private ObservableCollection<LogValue> _loadPortLogs = new ObservableCollection<LogValue>();
        public ObservableCollection<LogValue> LoadPortLogs
        {
            get { return _loadPortLogs; }
            set
            {
                if (_loadPortLogs != value)
                {
                    _loadPortLogs = value;
                    RaisePropertyChanged(nameof(LoadPortLogs));
                }
            }
        }
        
        private ObservableCollection<LogValue> _loadPortLocationLogs = new ObservableCollection<LogValue>();
        public ObservableCollection<LogValue> LoadPortLocationLogs
        {
            get { return _loadPortLocationLogs; }
            set
            {
                if (_loadPortLocationLogs != value)
                {
                    _loadPortLocationLogs = value;
                    RaisePropertyChanged(nameof(LoadPortLocationLogs));
                }
            }
        }

        private LoadPort_VM _loadPort_VM = null;
        public LoadPort_VM LoadPort_VM
        {
            get { return _loadPort_VM; }
            set
            {
                if (value != _loadPort_VM)
                {
                    _loadPort_VM = value;
                    RaisePropertyChanged(nameof(LoadPort_VM));
                }
            }
        }

        public LoadingLoadPort_VM(LoadPort_VM loadPort_VM)
        {
            LoadPort_VM = loadPort_VM;
            LoadPort_VM.OnUpdateLoadingLocationData += UpdateLocationLogValue;
            LoadPort_VM.OnUpdateLoadingLogData += UpdateLogData;
            LoadPort_VM.OnUpdateLoadingLogMessageFromPTL += UpdateLogValue;
            Messenger.Default.Register<LogValue>(this, UpdateLogValue);
            Messenger.Default.Register<HMIReset>(this, ResetErrorLog);
            ScanCarrierNoCommand = new RelayCommand<TextBox>(ScanCarrierNoCommandEventAsync);
        }

        private void ScanCarrierNoCommandEventAsync(TextBox txtbox)
        {
            try
            {
                if (!GlobalAttributesClass.HMIWaitingForHost)
                {
                    LoadBtnEnable = false;
                    LoadPortLocationLogs.Clear();

                    if (!string.IsNullOrEmpty(txtbox.Text))
                    {
                        LogHelper.General(3, $"LoadCarrier True and Thread: {(LoadThread?.IsAlive ?? false).ToString()}");

                        if (!(LoadThread?.IsAlive ?? false))
                        {
                            LoadPort_VM.IDFromInterface = txtbox.Text;
                            LoadThread?.Abort();
                            LoadThread = null;
                            LoadThread = new Thread(LoadPort_VM.LoadingCarrierEvent);
                            LoadThread.Name = nameof(LoadPort_VM.LoadingCarrierEvent);
                            LoadThread.Start();
                            LogHelper.General(3, $"LoadCarrier Started Thread");
                        }
                    }
                }
                else if (GlobalAttributesClass.HMIWaitingForHost)
                {
                    LogHelper.General(3, $"LoadCarrier Thread Waiting For Host Reply: Ignoring Carrier Scan {txtbox.Text}");
                    AlarmService.DefineAlarm(1007);
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }
            finally
            {
                txtbox.Clear();
                txtbox.Focus();
                LoadBtnEnable = true;
            }
        }

        private void UpdateLocationLogValue(LogValue log)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LoadPortLocationLogs.Insert(0, log);
            });
        }

        private void UpdateLogData(string logValue)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (LoadPortLogs.Count >= 30)
                {
                    LoadPortLogs.RemoveAt(29);
                }

                LoadPortLogs.Insert(errorLogCounts, new LogValue() { Value = $"{DateTime.Now.ToString(GlobalAttributesClass.TimeFormat)}| {logValue}" });

            }));
        }

        private void UpdateLogValue(LogValue value)
        {
            Task.Delay(100);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (value.Page.Equals(HMIPage.LoadingPage))
                {
                    if (LoadPortLogs.Count >= 30)
                    {
                        LoadPortLogs.RemoveAt(29);
                    }

                    if (value.Value.Contains("ERROR"))
                    {
                        errorLogCounts += 1;
                        LoadPortLogs.Insert(0, new LogValue() { Value = $"{DateTime.Now.ToString(GlobalAttributesClass.TimeFormat)}| {value.Value}" });
                    }
                    else if (value.Value.Contains("CLEARED"))
                    {
                        string logValue = value.Value.Split('-')[1];

                        if (LoadPortLogs.ToList().Where(x => x.Value.Contains("ERROR")).ToList().Any(x => x.Value.Contains(logValue)))
                        {
                            LogValue tmp = LoadPortLogs.ToList().Where(x => x.Value.Contains("ERROR")).ToList().Find(x => x.Value.Contains(logValue));
                            LoadPortLogs.Remove(tmp);
                            errorLogCounts -= 1;
                        }
                    }
                    else
                    {
                        LoadPortLogs.Insert(errorLogCounts, new LogValue() { Value = $"{DateTime.Now.ToString(GlobalAttributesClass.TimeFormat)}| {value.Value}" });

                        if (value.Value.Contains("Loaded"))
                        {
                            string logValue = string.Empty;

                            if (GlobalAttributesClass.LoadingMode.Equals(0))
                            {
                                logValue = value.Value.Split(' ')[0];
                            }
                            else
                            {
                                logValue = value.Value.Split(' ')[3];
                            }

                            if (LoadPortLocationLogs.ToList().ToList().Any(x => x.Value.Contains(logValue)))
                            {
                                LogValue tmp = LoadPortLocationLogs.ToList().ToList().Find(x => x.Value.Contains(logValue));
                                LoadPortLocationLogs.Remove(tmp);
                            }
                        }
                    }

                }
            }));
        }

        public async void ResetErrorLog(HMIReset reset)
        {
            await Task.Run(() => 
            {
                if (reset.Equals(HMIReset.Reset))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        while (errorLogCounts > 0)
                        {
                            LoadPortLogs.RemoveAt(0);
                            errorLogCounts -= 1;
                        }
                    }));
                }
            });
        }
    }
}
