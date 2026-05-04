using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HMI_CoolingEquipment.ViewModels
{
    public class AlarmMessage_VM : ViewModelBase
    {
        public ICommand AlarmMessageWindowClosing { get; set; }
        public ICommand FindAlarmHistoryCommand { get; set; }
        public ICommand AlarmStatusTabControlCommand { get; set; }
        public ICommand GenerateAlarmCSVCommand { get; set; }
        public ICommand ResetAlarmHistoryCommand { get; set; }
        public ICommand WarningErrorCheckedCommand { get; set; }
        public ICommand WarningErrorUncheckedCommand { get; set; }

        private Visibility _isVisibleWarningErrorGB = Visibility.Collapsed;
        public Visibility IsVisibleWarningErrorGB
        {
            get { return _isVisibleWarningErrorGB; }
            set
            {
                if( _isVisibleWarningErrorGB != value)
                {
                    _isVisibleWarningErrorGB = value;
                    RaisePropertyChanged(nameof(IsVisibleWarningErrorGB));
                }
            }
        }
        
        private Visibility _isVisibleWarning = Visibility.Collapsed;
        public Visibility IsVisibleWarning
        {
            get { return _isVisibleWarning; }
            set
            {
                if( _isVisibleWarning != value)
                {
                    _isVisibleWarning = value;
                    RaisePropertyChanged(nameof(IsVisibleWarning));
                }
            }
        }

        private Visibility _isVisibleError = Visibility.Collapsed;
        public Visibility IsVisibleError
        {
            get { return _isVisibleError; }
            set
            {
                if( _isVisibleError != value)
                {
                    _isVisibleError = value;
                    RaisePropertyChanged(nameof(IsVisibleError));
                }
            }
        }
        
        private bool _isCheckedWarning = true;
        public bool IsCheckedWarning
        {
            get { return _isCheckedWarning; }
            set
            {
                if( _isCheckedWarning != value)
                {
                    _isCheckedWarning = value;
                    RaisePropertyChanged(nameof(IsCheckedWarning));
                }
            }
        }

        private bool _isCheckedError = true;
        public bool IsCheckedError
        {
            get { return _isCheckedError; }
            set
            {
                if( _isCheckedError != value)
                {
                    _isCheckedError = value;
                    RaisePropertyChanged(nameof(IsCheckedError));
                }
            }
        }

        private DateTime _fromDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
        public DateTime FromDate
        {
            get { return _fromDate; }
            set
            {
                if(_fromDate != value)
                {
                    _fromDate = value;
                    RaisePropertyChanged(nameof(FromDate));
                }
            }
        }
        
        private DateTime _toDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
        public DateTime ToDate
        {
            get { return _toDate; }
            set
            {
                if(_toDate != value)
                {
                    _toDate = value;
                    RaisePropertyChanged(nameof(ToDate));
                }
            }
        }

        private ObservableCollection<AlarmCurrentModel> _currentAlarmMessage = new ObservableCollection<AlarmCurrentModel>();
        public ObservableCollection<AlarmCurrentModel> CurrentAlarmMessage
        {
            get { return _currentAlarmMessage; }
            set
            {
                if(_currentAlarmMessage != value)
                {
                    _currentAlarmMessage = value;
                    RaisePropertyChanged(nameof(CurrentAlarmMessage));
                }
            }
        }

        private ObservableCollection<AlarmHistoryModel> _historyAlarmMessage = new ObservableCollection<AlarmHistoryModel>();
        public ObservableCollection<AlarmHistoryModel> HistoryAlarmMessage
        {
            get { return _historyAlarmMessage; }
            set
            {
                if(_historyAlarmMessage != value)
                {
                    _historyAlarmMessage = value;
                    RaisePropertyChanged(nameof(HistoryAlarmMessage));
                }
            }
        }
        
        private List<AlarmHistoryModel> _historyAlarmMessage_BK = new List<AlarmHistoryModel>();
        public List<AlarmHistoryModel> HistoryAlarmMessage_BK
        {
            get { return _historyAlarmMessage_BK; }
            set
            {
                if(_historyAlarmMessage_BK != value)
                {
                    _historyAlarmMessage_BK = value;
                    RaisePropertyChanged(nameof(HistoryAlarmMessage_BK));
                }
            }
        }

        public AlarmMessage_VM()
        {
            Messenger.Default.Register<List<AlarmCurrentModel>>(this, UpdateAlarmCurrentMessage);
            Messenger.Default.Register<List<AlarmHistoryModel>>(this, UpdateAlarmHistoryMessage);
            AlarmMessageWindowClosing = new RelayCommand<EventArgsModel>(AlarmMessageWindowClosingEvent);
            FindAlarmHistoryCommand = new RelayCommand(FindAlarmHistoryCommandEvent);
            GenerateAlarmCSVCommand = new RelayCommand(GenerateAlarmCSVCommandEvent);
            AlarmStatusTabControlCommand = new RelayCommand<TabControl>(AlarmStatusTabControlCommandEvent);
            ResetAlarmHistoryCommand = new RelayCommand(ResetAlarmHistoryCommandEvent);
            WarningErrorCheckedCommand = new RelayCommand<object>(WarningErrorCheckedCommandEvent);
            WarningErrorUncheckedCommand = new RelayCommand<object>(WarningErrorUncheckedCommandEvent);
        }

        public void AlarmStatusTabControlCommandEvent(TabControl sender)
        {
            TabItem tab = sender.SelectedItem as TabItem;
            string header = tab.Header.ToString();

            if (header.Equals("History"))
            {
                FromDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                ToDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);

                UpdateAlarmHistoryMessage(EF_Service.EF_GetAlarmHistory(FromDate.Date, ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)));
            }
        }

        private void UpdateAlarmCurrentMessage(List<AlarmCurrentModel> model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                CurrentAlarmMessage.Clear();

                if (model.Count > 0)
                {
                    model.ForEach(x =>
                    {
                        CurrentAlarmMessage.Insert(0, x);
                    }); 
                }

            }));
        }

        private void UpdateAlarmHistoryMessage(List<AlarmHistoryModel> model)
        {
            string dt_Now = DateTime.Now.ToShortDateString();

            IsVisibleWarning = Visibility.Collapsed;
            IsVisibleError = Visibility.Collapsed;
            IsVisibleWarningErrorGB = Visibility.Collapsed;

            IsCheckedWarning = false;
            IsCheckedError = false;

            HistoryAlarmMessage_BK = model;

            if (HistoryAlarmMessage_BK.Any(x => x.AlarmType.Equals(AlarmType.Warning)))
            {
                IsVisibleWarning = Visibility.Visible;
                IsVisibleWarningErrorGB = Visibility.Visible;
                IsCheckedWarning = true;
            }

            if (HistoryAlarmMessage_BK.Any(x => x.AlarmType.Equals(AlarmType.Error)))
            {
                IsVisibleError = Visibility.Visible;
                IsVisibleWarningErrorGB = Visibility.Visible;
                IsCheckedError = true;
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                HistoryAlarmMessage.Clear();

                HistoryAlarmMessage_BK.ForEach(x =>
                {
                    HistoryAlarmMessage.Insert(0, x);
                });
            }));
        }

        private void AlarmMessageWindowClosingEvent(EventArgsModel eventArgs)
        {
            try
            {
                CancelEventArgs cancelEventArgs = eventArgs.EventArgs as CancelEventArgs;
                cancelEventArgs.Cancel = true;

                Window window = eventArgs.Parameter as Window;
                window.Hide();
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }
        }

        public void FindAlarmHistoryCommandEvent()
        {
            TimeSpan difference = ToDate.Date - FromDate.Date;
            if (difference.Days >= 0)
            {
                if (difference.Days > 93)
                {
                    MessageBox.Show("Max Date Range is 90 Days", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    UpdateAlarmHistoryMessage(EF_Service.EF_GetAlarmHistory(FromDate.Date, ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)));
                }
            }
            else
            {
                MessageBox.Show("\"Date From\" Must Before or Same With \"Date To\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetAlarmHistoryCommandEvent()
        {
            FromDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
            ToDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);

            UpdateAlarmHistoryMessage(EF_Service.EF_GetAlarmHistory(FromDate.Date, ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)));
        }

        private void GenerateAlarmCSVCommandEvent()
        {
            try
            {
                if (HistoryAlarmMessage.Count > 0)
                {
#if DEBUG
                    string path = @$"D:\HMI_Reports\Alarms\";
#else
                    string path = @$"E:\HMI_Reports\Alarms\"; 
#endif
                    string fileName = string.Empty;

                    if (IsCheckedWarning)
                    {
                        fileName += "Warning";
                    }

                    if (IsCheckedError)
                    {
                        fileName += "Error";
                    }

                    string folderName = $"AlarmReport-{fileName} ({FromDate.Date:yyyy.MM.dd}-{ToDate.Date:yyyy.MM.dd}).csv";

                    if (File.Exists(path + folderName))
                    {
                        File.Delete(path + folderName);
                    }

                    HistoryAlarmMessage.OrderBy(x => x.AlarmDateTime);

                    HistoryAlarmMessage.ToList().ForEach(x =>
                    {
                        GlobalClassFunction.CsvWriter(path, folderName, new Dictionary<string, string>
                        {
                            [nameof(x.AlarmDateTime)] = x.AlarmDateTime.ToString(),
                            [nameof(x.AlarmType)] = x.AlarmType.ToString(),
                            [nameof(x.AlarmCode)] = x.AlarmCode.ToString(),
                            [nameof(x.AlarmModule)] = x.AlarmModule.ToString(),
                            [nameof(x.AlarmMessage)] = x.AlarmMessage.ToString(),
                            [nameof(x.AlarmAction)] = x.AlarmAction.ToString(),
                        });
                    });
                    MessageBox.Show($"Export Alarm History Done.\nFilePath: {path + folderName}", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export Failed.\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WarningErrorCheckedCommandEvent(object obj)
        {
            List<AlarmHistoryModel> tempModel = new List<AlarmHistoryModel>();

            if(IsCheckedWarning)
            {
                if (HistoryAlarmMessage_BK.Any(x => x.AlarmType.Equals(AlarmType.Warning)))
                {
                    tempModel.AddRange(HistoryAlarmMessage_BK.Where(x => x.AlarmType.Equals(AlarmType.Warning)).ToList()); 
                }
            }

            if(IsCheckedError)
            {
                if (HistoryAlarmMessage_BK.Any(x => x.AlarmType.Equals(AlarmType.Error)))
                {
                    tempModel.AddRange(HistoryAlarmMessage_BK.Where(x => x.AlarmType.Equals(AlarmType.Error)).ToList());
                }
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                HistoryAlarmMessage.Clear();

                tempModel.ForEach(x =>
                {
                    HistoryAlarmMessage.Insert(0, x);
                });
            }));
        }

        private void WarningErrorUncheckedCommandEvent(object obj)
        {
            List<AlarmHistoryModel> tempModel = new List<AlarmHistoryModel>();

            if (IsCheckedWarning)
            {
                if (HistoryAlarmMessage_BK.Any(x => x.AlarmType.Equals(AlarmType.Warning)))
                {
                    tempModel.AddRange(HistoryAlarmMessage_BK.Where(x => x.AlarmType.Equals(AlarmType.Warning)).ToList());
                }
            }

            if (IsCheckedError)
            {
                if (HistoryAlarmMessage_BK.Any(x => x.AlarmType.Equals(AlarmType.Error)))
                {
                    tempModel.AddRange(HistoryAlarmMessage_BK.Where(x => x.AlarmType.Equals(AlarmType.Error)).ToList());
                }
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                HistoryAlarmMessage.Clear();

                tempModel.ForEach(x =>
                {
                    HistoryAlarmMessage.Insert(0, x);
                });
            }));
        }
    }
}
