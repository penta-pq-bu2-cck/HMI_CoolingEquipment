using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using HMI_CoolingEquipment.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace HMI_CoolingEquipment.ViewModels
{
    public class LotStatus_VM : ViewModelBase
    {
        private CarrierDetailsWindow carrierDetailsWindow = new CarrierDetailsWindow(new CarrierDetails_VM());
        private string CurrentHeader = string.Empty;
        private UserAccountModel UserAccountModel = null;

        public ICommand FindJobHistoryCommand { get; set; }
        public ICommand LotStatusTabControlCommand { get; set; }
        public ICommand ResetJobHistoryCommand { get; set; }
        public ICommand GenerateJobCSVCommand { get; set; }

        private DateTime _fromDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
        public DateTime FromDate
        {
            get { return _fromDate; }
            set
            {
                if (_fromDate != value)
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
                if (_toDate != value)
                {
                    _toDate = value;
                    RaisePropertyChanged(nameof(ToDate));
                }
            }
        }

        private List<SetupJobModel> _setupJobModel = new List<SetupJobModel>();
        public List<SetupJobModel> SetupJobModel
        {
            get { return _setupJobModel; }
            set
            {
                if(value != _setupJobModel)
                {
                    _setupJobModel = value;
                    RaisePropertyChanged(nameof(SetupJobModel));
                }
            }
        }

        private List<SetupJobHistoryModel> _setupJobModelHistory = new List<SetupJobHistoryModel>();
        public List<SetupJobHistoryModel> SetupJobModelHistory
        {
            get { return _setupJobModelHistory; }
            set
            {
                if(value != _setupJobModelHistory)
                {
                    _setupJobModelHistory = value;
                    RaisePropertyChanged(nameof(SetupJobModelHistory));
                }
            }
        }

        public LotStatus_VM()
        {
            Messenger.Default.Register<List<SetupJobModel>>(this, UpdateSetupJobModelList);
            Messenger.Default.Register<List<SetupJobHistoryModel>>(this, UpdateSetupJobHistoryModelList);
            Messenger.Default.Register<UserAccountModel>(this, UpdateUserAccount);
            Messenger.Default.Register<HMIReset>(this, ResetJobHistory);
            FindJobHistoryCommand = new RelayCommand(FindJobHistoryCommandEvent);
            LotStatusTabControlCommand = new RelayCommand<TabControl>(LotStatusTabControlCommandEvent);
            ResetJobHistoryCommand = new RelayCommand(ResetJobHistoryCommandEvent);
            GenerateJobCSVCommand = new RelayCommand(GenerateJobCSVCommandEvent);
        }

        private void UpdateUserAccount(UserAccountModel model)
        {
            try
            {
                UserAccountModel = model;

                if(model != null)
                {
                    if (model.UserGroup.Equals("Operator"))
                    {
                        SetupJobModel.ForEach(x => x.SetupJobButtonView = Visibility.Collapsed);
                    }
                    else
                    {
                        SetupJobModel.ForEach(x => x.SetupJobButtonView = Visibility.Visible);
                    }
                }
                else
                {
                    SetupJobModel.ForEach(x => x.SetupJobButtonView = Visibility.Collapsed);
                }
            }
            catch (Exception ex)
            {
                LogHelper.PTL(5, ex.Message);
            }
        }

        private void UpdateSetupJobModelList(List<SetupJobModel> list)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                List<SetupJobModel> temp_Current = new List<SetupJobModel>();

                temp_Current = list.FindAll(x => !x.JobStatus.Equals(JobStatus.Complete));

                SetupJobModel = temp_Current.OrderBy(x => x.CreatedOn).ToList();

                SetupJobModel.ForEach(x => x.DataGridImageClickCommand = new RelayCommand<SetupJobModel>(DataGridImageClickCommandEvent));
                SetupJobModel.ForEach(x => x.NonOpr_DataGridPurgeButtonClickCommand = new RelayCommand<SetupJobModel>(NonOpr_DataGridPurgeButtonClickCommandEvent));
                SetupJobModel.ForEach(x => x.NonOpr_DataGridAbortButtonClickCommand = new RelayCommand<SetupJobModel>(NonOpr_DataGridAbortButtonClickCommandEvent));

                UpdateUserAccount(UserAccountModel);
            }));
        }

        private void NonOpr_DataGridAbortButtonClickCommandEvent(SetupJobModel model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (MessageBox.Show($"Are you sure want to abort this Lot ?\nLot Information:\nLot ID = [{model.LotNo}]\nJob ID = [{model.JobID}]", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning).Equals(MessageBoxResult.OK))
                {
                    LoadPort_VM.LoadingLogData($"Lot ID = [{model.LotNo}] - Lot Aborted");
                    LoadPort_VM.UnloadingLogData($"Lot ID = [{model.LotNo}] - Lot Aborted");
                    EF_Service.EF_InsertEventLog("WARNING", "ABORT", model.JobID, model.LotNo, "", "", UserAccountModel.UserName, "Lot Aborted");

                    if (EF_Service.EF_AbortJob(model.JobID, model.LotNo))
                    {
                        MessageBox.Show("Lot Aborted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Lot Fail to Abort", "Abort Fail", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }));
        }

        private void NonOpr_DataGridPurgeButtonClickCommandEvent(SetupJobModel model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (MessageBox.Show($"Are you sure want to purge this Lot ?\nLot Information:\nLot ID = [{model.LotNo}]\nJob ID = [{model.JobID}]", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning).Equals(MessageBoxResult.OK))
                {
                    LoadPort_VM.LoadingLogData($"Lot ID = [{model.LotNo}] - Lot Purged");
                    LoadPort_VM.UnloadingLogData($"Lot ID = [{model.LotNo}] - Lot Purged");
                    EF_Service.EF_InsertEventLog("WARNING", "PURGED", model.JobID, model.LotNo, "", "", UserAccountModel.UserName, "Lot Purged");

                    if (EF_Service.EF_PurgeJob(model.JobID, model.LotNo))
                    {
                        MessageBox.Show("Lot Purged", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Fail to Purge lot.", "Purge Fail", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }));
        }

        private void UpdateSetupJobHistoryModelList(List<SetupJobHistoryModel> list)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SetupJobModelHistory = list;

                SetupJobModelHistory.ForEach(x => x.DataGridImageClickHistoryCommand = new RelayCommand<List<JobDetailsHistory>>(DataGridImageClickHistoryCommandEvent));
            }));
        }

        private void DataGridImageClickCommandEvent(SetupJobModel model)
        {
            Messenger.Default.Send(new CarrierDetails_VM
            {
                SetupJobModel = model,
            });

            carrierDetailsWindow.Show();
        }

        private void DataGridImageClickHistoryCommandEvent(List<JobDetailsHistory> model)
        {
            Messenger.Default.Send(new CarrierDetails_VM
            {
                JobDetailsHistory = model,
            });

            carrierDetailsWindow.Show();
        }

        public void FindJobHistoryCommandEvent()
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
                    UpdateSetupJobHistoryModelList(EF_Service.EF_GetJobHistory(FromDate.Date, ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)));
                }
            }
            else
            {
                MessageBox.Show("\"Date From\" Must Before or Same With \"Date To\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LotStatusTabControlCommandEvent(TabControl sender)
        {
            TabItem tab = sender.SelectedItem as TabItem;
            string header = tab.Header.ToString();

            if (header.Equals("History") && !header.Equals(CurrentHeader))
            {
                CurrentHeader = header;
                FromDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                ToDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);

                UpdateSetupJobHistoryModelList(EF_Service.EF_GetJobHistory(FromDate.Date, ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)));
            }
            else
            {
                CurrentHeader = header;
            }
        }

        private void ResetJobHistoryCommandEvent()
        {
            FromDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
            ToDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);

            UpdateSetupJobHistoryModelList(EF_Service.EF_GetJobHistory(FromDate.Date, ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)));
        }

        private void ResetJobHistory(HMIReset reset)
        {
            if (reset.Equals(HMIReset.ResetJobHistory))
            {
                FromDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                ToDate = DateTime.ParseExact(DateTime.Now.ToString(GlobalAttributesClass.TimeFormat), GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);

                UpdateSetupJobHistoryModelList(EF_Service.EF_GetJobHistory(FromDate.Date, ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)));
            }
        }

        private void GenerateJobCSVCommandEvent()
        {
            try
            {
                if (SetupJobModelHistory.Count > 0)
                {
#if DEBUG
                    string path = @$"D:\HMI_Reports\Jobs\";
#else
                    string path = @$"E:\HMI_Reports\Jobs\"; 
#endif

                    SetupJobModelHistory.OrderBy(x => x.CompletedOn);

                    SetupJobModelHistory.ForEach(x =>
                    {
                        string folderName = $"JobReport-{x.LotNo}_{x.JobID}_({x.CreatedOn.Value:yyyy-MM-dd}).csv";

                        if (File.Exists(path + folderName))
                        {
                            File.Delete(path + folderName);
                        }

                        GlobalClassFunction.CsvWriter(path, folderName, new Dictionary<string, string>
                        {
                            [nameof(x.LotNo)] = x.LotNo.ToString(),
                            [nameof(x.JobID)] = x.JobID.ToString(),
                            ["Loading Time (Start)"] = x.LD_START.ToString(),
                            ["Loading Time (End)"] = x.LD_END.ToString(),
                            ["Cooling Time (Start)"] = x.CT_START.ToString(),
                            ["Cooling Time (End)"] = x.CT_END.ToString(),
                            ["Staging Time (Start)"] = x.ST_START.ToString(),
                            ["Staging Time (End)"] = x.ST_END.ToString(),
                            ["Created On"] = x.CreatedOn.ToString(),
                            ["Completed On"] = x.CompletedOn.ToString(),
                        });

                        int i = 0;

                        x.JobDetailsHistory.ForEach(y =>
                        {
                            GlobalClassFunction.CsvWriter(path, folderName, new Dictionary<string, string>
                            {
                                ["Tray Type"] = y.Type.ToString(),
                                ["Carrier ID"] = y.CarrierID.ToString(),
                                ["Load Port Location"] = y.PortLocation.ToString(),
                                ["Loading Time"] = y.LoadingTime.ToString(),
                                ["Load By"] = y.LoadBy.ToString(),
                                ["Unloading Time"] = y.UnloadingTime.ToString(),
                                ["Unload By"] = y.UnloadBy.ToString(),
                            }, i.Equals(0));

                            i += 1;
                        });
                    });
                    MessageBox.Show($"Export Job History Done.\nFilePath: {path}", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export Failed.\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
