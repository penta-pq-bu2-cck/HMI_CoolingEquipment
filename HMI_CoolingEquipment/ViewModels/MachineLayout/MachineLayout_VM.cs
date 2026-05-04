using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace HMI_CoolingEquipment.ViewModels
{
    public class MachineLayout_VM : ViewModelBase
    {
        private int errorLogCounts = 0;

        private List<SetupJobModel> _setupJobs = new List<SetupJobModel>();
        public List<SetupJobModel> SetupJobs
        {
            get { return _setupJobs; }
            set
            {
                if (_setupJobs != value)
                {
                    _setupJobs = value;
                    RaisePropertyChanged(nameof(SetupJobs));
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


        private ObservableCollection<LogValue> _mainLogs = new ObservableCollection<LogValue>();
        public ObservableCollection<LogValue> MainLogs
        {
            get { return _mainLogs; }
            set
            {
                if (_mainLogs != value)
                {
                    _mainLogs = value;
                    RaisePropertyChanged(nameof(MainLogs));
                }
            }
        }


        private void UpdateSetupJobModelListCount(List<SetupJobModel> list)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SetupJobs = list;
            }));
        }

        public MachineLayout_VM(LoadPort_VM loadPort_VM)
        {
            LoadPort_VM = loadPort_VM;
            LoadPort_VM.OnUpdateUnloadingLogData += UpdateLogData;
            LoadPort_VM.OnUpdateLoadingLogData += UpdateLogData;
            LoadPort_VM.OnUpdateUnloadingLogMessageFromPTL += UpdateLogValue;
            LoadPort_VM.OnUpdateLoadingLogMessageFromPTL += UpdateLogValue;
            LoadPort_VM.OnUpdateLoadingLogMessageFromPTL += UpdateLogValueError;
            Messenger.Default.Register<LogValue>(this, UpdateLogValue);
            Messenger.Default.Register<LogValue>(this, UpdateLogValue);
            Messenger.Default.Register<List<SetupJobModel>>(this, UpdateSetupJobModelListCount);
        }

        private void UpdateLogData(string logValue)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (MainLogs.Count >= 100)
                {
                    MainLogs.RemoveAt(99);
                }

                MainLogs.Insert(errorLogCounts, new LogValue() { Value = $"{DateTime.Now.ToString(GlobalAttributesClass.TimeFormat)}| {logValue}" });
            }));
        }

        private void UpdateLogValue(LogValue value)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (value.Page.Equals(HMIPage.LoadingPage) || value.Page.Equals(HMIPage.UnloadingPage))
                {
                    if (!value.Value.Contains("ERROR") && !value.Value.Contains("CLEARED"))
                    {
                        if (MainLogs.Count >= 100)
                        {
                            MainLogs.RemoveAt(99);
                        }

                        string logValue = value.Value;

                        if (!MainLogs.ToList().Where(x => !x.Value.Contains("ERROR")).ToList().Any(x => x.Value.Contains(value.Value)))
                        {
                            MainLogs.Insert(errorLogCounts, new LogValue() { Value = $"{DateTime.Now.ToString(GlobalAttributesClass.TimeFormat)}| {value.Value}" });
                        }
                    }
                }
            }));
        }

        private void UpdateLogValueError(LogValue value)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (value.Page.Equals(HMIPage.LoadingPage))
                {
                    if (MainLogs.Count >= 100)
                    {
                        MainLogs.RemoveAt(99);
                    }

                    if (value.Value.Contains("ERROR"))
                    {
                        if (!MainLogs.ToList().Where(x => x.Value.Contains("ERROR")).ToList().Any(x => x.Value.Contains(value.Value)))
                        {
                            errorLogCounts += 1;
                            MainLogs.Insert(0, new LogValue() { Value = $"{DateTime.Now.ToString(GlobalAttributesClass.TimeFormat)}| {value.Value}" }); 
                        }
                    }
                    else if (value.Value.Contains("CLEARED"))
                    {
                        string logValue = value.Value.Split('-')[1];

                        if (MainLogs.ToList().Where(x => x.Value.Contains("ERROR")).ToList().Any(x => x.Value.Contains(logValue)))
                        {
                            LogValue tmp = MainLogs.ToList().Where(x => x.Value.Contains("ERROR")).ToList().Find(x => x.Value.Contains(logValue));
                            MainLogs.Remove(tmp);
                            errorLogCounts -= 1;
                        }
                    }
                }
            }));
        }
    }
}
