using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GEMPro300.Model;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using HMI_CoolingEquipment.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HMI_CoolingEquipment.ViewModels
{
    public class UnloadingLoadPort_VM : ViewModelBase
    {
        UnloadJobDetailsWindow UnloadJobListDetails = new UnloadJobDetailsWindow(new UnloadJobDetails_VM());
        private int errorLogCounts = 0;
        private Thread UnloadThread;
        public TextBox txtBox;

        public ICommand JobListDetailsCommand { get; set; }
        public ICommand UnloadLoadPortCommand { get; set; }

        private bool _unloadBtnEnable = true;
        public bool UnloadBtnEnable
        {
            get { return _unloadBtnEnable; }
            set
            {
                if (_unloadBtnEnable != value)
                {
                    _unloadBtnEnable = value;
                    RaisePropertyChanged(nameof(UnloadBtnEnable));
                }
            }
        }

        private ObservableCollection<LogValue> _unloadPortLogs = new ObservableCollection<LogValue>();
        public ObservableCollection<LogValue> UnloadPortLogs
        {
            get { return _unloadPortLogs; }
            set
            {
                if (_unloadPortLogs != value)
                {
                    _unloadPortLogs = value;
                    RaisePropertyChanged(nameof(UnloadPortLogs));
                }
            }
        }

        private ObservableCollection<LogValue> _unloadPortLocationLogs = new ObservableCollection<LogValue>();
        public ObservableCollection<LogValue> UnloadPortLocationLogs
        {
            get { return _unloadPortLocationLogs; }
            set
            {
                if (_unloadPortLocationLogs != value)
                {
                    _unloadPortLocationLogs = value;
                    RaisePropertyChanged(nameof(UnloadPortLocationLogs));
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

        public UnloadingLoadPort_VM(LoadPort_VM loadPort_VM)
        {
            LoadPort_VM = loadPort_VM;
            Messenger.Default.Register<SetupJobModel>(this, TriggerUnloadFromWindows);
            LoadPort_VM.OnUpdateUnloadingLocationData += UpdateLocationLogValue;
            LoadPort_VM.OnUpdateUnloadingLogData += UpdateLogData;
            LoadPort_VM.OnUpdateUnloadingLogMessageFromPTL += UpdateLogValue;
            Messenger.Default.Register<LogValue>(this, UpdateLogValue);
            Messenger.Default.Register<HMIReset>(this, ResetErrorLog);
            JobListDetailsCommand = new RelayCommand(JobListDetailsCommandEvent);
            UnloadLoadPortCommand = new RelayCommand<TextBox>(UnloadLoadPortCommandEvent);
        }

        private void UpdateLocationLogValue(LogValue log)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UnloadPortLocationLogs.Insert(0, log);
            });
        }

        private void JobListDetailsCommandEvent()
        {
            UnloadJobListDetails.Show();
        }

        private void UnloadLoadPortCommandEvent(TextBox txtBox)
        {
            try
            {
                UnloadBtnEnable = false;
                UnloadPortLocationLogs.Clear();

                if (!string.IsNullOrEmpty(txtBox.Text))
                {
                    LogHelper.General(3, $"UnloadCarrier True and Thread: {(UnloadThread?.IsAlive ?? false).ToString()}");

                    if (!(UnloadThread?.IsAlive ?? false))
                    {
                        LoadPort_VM.IDFromInterface = txtBox.Text;
                        UnloadThread?.Abort();
                        UnloadThread = null;
                        UnloadThread = new Thread(LoadPort_VM.UnloadingCarrierEvent);
                        UnloadThread.Name = nameof(LoadPort_VM.UnloadingCarrierEvent);
                        UnloadThread.Start();
                        LogHelper.General(3, $"UnloadCarrier Started Thread");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }
            finally
            {
                txtBox.Clear();
                txtBox.Focus();
                UnloadBtnEnable = true; 
            }
        }

        private void TriggerUnloadFromWindows(SetupJobModel model)
        {
            if (model != null)
            {
                LogHelper.General(3, $"[TriggerUnloadFromWindows] = Unload JodID: {model.JobID} / LotNo: {model.LotNo} / JobStatus: {model.JobStatus}");
                txtBox.Text = model.LotNo;
                UnloadLoadPortCommandEvent(txtBox);
            }
        }

        private void UpdateLogData(string logValue)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (UnloadPortLogs.Count >= 30)
                {
                    UnloadPortLogs.RemoveAt(29);
                }

                UnloadPortLogs.Insert(errorLogCounts, new LogValue() { Value = $"{DateTime.Now.ToString(GlobalAttributesClass.TimeFormat)}| {logValue}" });

            }));
        } 

        private void UpdateLogValue(LogValue value)
        {
            Task.Delay(100);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (value.Page.Equals(HMIPage.UnloadingPage))
                {
                    if (UnloadPortLogs.Count >= 30)
                    {
                        UnloadPortLogs.RemoveAt(29);
                    }

                    if (value.Value.Contains("ERROR"))
                    {
                        if (!UnloadPortLogs.ToList().Where(x => x.Value.Contains("ERROR")).ToList().Any(x => x.Value.Contains(value.Value)))
                        {
                            errorLogCounts += 1;
                            UnloadPortLogs.Insert(0, new LogValue() { Value = $"{DateTime.Now.ToString(GlobalAttributesClass.TimeFormat)}| {value.Value}" });
                        }
                    }
                    else if (value.Value.Contains("CLEARED"))
                    {
                        string logValue = value.Value.Split('-')[1];

                        if (UnloadPortLogs.ToList().Where(x => x.Value.Contains("ERROR")).ToList().Any(x => x.Value.Contains(logValue)))
                        {
                            LogValue tmp = UnloadPortLogs.ToList().Where(x => x.Value.Contains("ERROR")).ToList().Find(x => x.Value.Contains(logValue));
                            UnloadPortLogs.Remove(tmp);
                            errorLogCounts -= 1;
                        }
                    }
                    else
                    {
                        UnloadPortLogs.Insert(errorLogCounts, new LogValue() { Value = $"{DateTime.Now.ToString(GlobalAttributesClass.TimeFormat)}| {value.Value}" });

                        if (value.Value.Contains("Removed"))
                        {
                            string logValue = value.Value.Split(' ')[0];

                            if (UnloadPortLocationLogs.ToList().ToList().Any(x => x.Value.Contains(logValue)))
                            {
                                LogValue tmp = UnloadPortLocationLogs.ToList().ToList().Find(x => x.Value.Contains(logValue));
                                UnloadPortLocationLogs.Remove(tmp);
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
                            UnloadPortLogs.RemoveAt(0);
                            errorLogCounts -= 1;
                        }
                    }));
                }
            });
        }

    }
}
