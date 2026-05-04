using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace HMI_CoolingEquipment.ViewModels
{
    public class UnloadJobDetails_VM : ViewModelBase
    {
        public ICommand UnloadingJobDetailsWindowClosing { get; set; }

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


        public UnloadJobDetails_VM()
        {
            Messenger.Default.Register<List<SetupJobModel>>(this, UpdateSetupJobModelList);
            UnloadingJobDetailsWindowClosing = new RelayCommand<EventArgsModel>(UnloadingJobDetailsWindowClosingEvent);
        }

        private void UpdateSetupJobModelList(List<SetupJobModel> list)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                List<SetupJobModel> StagingJob = new List<SetupJobModel>();

                StagingJob = list.FindAll(x => x.JobStatus.Equals(JobStatus.Staging) || x.JobStatus.Equals(JobStatus.StagingTimeExpired));

                SetupJobModel = StagingJob;
                SetupJobModel.ForEach(x => x.DataGridButtonClickCommand = new RelayCommand<UnloadingSetupJobModel>(DataGridButtonClickCommandEvent));
            }));
        }

        private void UnloadingJobDetailsWindowClosingEvent(EventArgsModel eventArgs)
        {
            CancelEventArgs cancelEventArgs = eventArgs.EventArgs as CancelEventArgs;
            cancelEventArgs.Cancel = true;

            UnloadingSetupJobModel model = eventArgs.Parameter as UnloadingSetupJobModel;
            model.UnloadingLotDetailsWindow.Hide();
        }

        private void DataGridButtonClickCommandEvent(UnloadingSetupJobModel model)
        {
            //if (MessageBox.Show($"You have selected:\nLot ID = [{model.setupJobModel.LotNo}]\nJob ID = [{model.setupJobModel.JobID}]\n\nAre sure you want to Unload this Job ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information).Equals(MessageBoxResult.Yes))
            //{
            //    model.UnloadingLotDetailsWindow.Hide();
            //    Messenger.Default.Send(model.setupJobModel);
            //}
            model.UnloadingLotDetailsWindow.Hide();
            Messenger.Default.Send(model.setupJobModel);
        }
    }
}
