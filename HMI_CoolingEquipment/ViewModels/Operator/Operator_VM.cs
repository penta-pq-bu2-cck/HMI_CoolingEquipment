using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HMI_CoolingEquipment.ViewModels
{
    public class Operator_VM : ViewModelBase
    {
        public ICommand LoadingButtonCommand { get; set; }
        public ICommand UnloadingButtonCommand { get; set; }

        private UserAccountModel UserAccountModel = null;

        private List<SetupJobModel> _setupJobModel = new List<SetupJobModel>();
        public List<SetupJobModel> SetupJobModel
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

        public Operator_VM()
        {
            Messenger.Default.Register<List<SetupJobModel>>(this, UpdateSetupJobModelList);
            Messenger.Default.Register<UserAccountModel>(this, UpdateUserAccount);
            LoadingButtonCommand = new RelayCommand(LoadingButtonCommandEvent);
            UnloadingButtonCommand = new RelayCommand(UnloadingButtonCommandEvent);
        }

        private void UpdateUserAccount(UserAccountModel model)
        {
            try
            {
                UserAccountModel = model;
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
                SetupJobModel.ForEach(x => x.DataGridPurgeButtonClickCommand = new RelayCommand<SetupJobModel>(DataGridPurgeButtonClickCommandEvent));
            }));
        }

        private void LoadingButtonCommandEvent()
        {
            Messenger.Default.Send(new OperatorModel
            {
                Name = "Loading"
            });
        }

        private void UnloadingButtonCommandEvent()
        {
            Messenger.Default.Send(new OperatorModel
            {
                Name = "Unloading"
            });
        }

        private void DataGridPurgeButtonClickCommandEvent(SetupJobModel model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (MessageBox.Show($"Are you sure want to purge this Lot ?\nLot Information:\nLot ID = [{model.LotNo}]\nJob ID = [{model.JobID}]", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning).Equals(MessageBoxResult.OK))
                {
                    LoadPort_VM.LoadingLogData($"Lot ID = [{model.LotNo}] - Lot Purged");
                    LoadPort_VM.UnloadingLogData($"Lot ID = [{model.LotNo}] - Lot Purged");
                    EF_Service.EF_InsertEventLog("WARNING", "ABORT", model.JobID, model.LotNo, "", "", UserAccountModel.UserName, "Lot Purged");

                    if (EF_Service.EF_PurgeJob(model.JobID, model.LotNo))
                    {
                        MessageBox.Show("Lot Purged\nPlease contact admin to abort this Lot", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Fail to Purge lot.\nnPlease contact admin.", "Purge Fail", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }));
        }

    }
}
