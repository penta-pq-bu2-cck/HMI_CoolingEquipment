using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HMI_CoolingEquipment.ViewModels
{
    public class CarrierDetails_VM : ViewModelBase
    {
        public ICommand CarrierDetailsClosingCommand { get; set; }

        private Visibility _isJobDetailsVisible = Visibility.Collapsed;
        public Visibility IsJobDetailsVisible
        {
            get { return _isJobDetailsVisible; }
            set
            {
                if (_isJobDetailsVisible != value)
                {
                    _isJobDetailsVisible = value;
                    RaisePropertyChanged(nameof(IsJobDetailsVisible));
                }
            }
        }

        private Visibility _isHistoryJobDetailsVisible = Visibility.Collapsed;
        public Visibility IsHistoryJobDetailsVisible
        {
            get { return _isHistoryJobDetailsVisible; }
            set
            {
                if (_isHistoryJobDetailsVisible != value)
                {
                    _isHistoryJobDetailsVisible = value;
                    RaisePropertyChanged(nameof(IsHistoryJobDetailsVisible));
                }
            }
        }


        private List<JobDetailsModel> _jobDetails = new List<JobDetailsModel>();
        public List<JobDetailsModel> JobDetails
        {
            get { return _jobDetails; }
            set
            {
                if(_jobDetails != value)
                {
                    _jobDetails = value;
                    RaisePropertyChanged(nameof(JobDetails));
                }
            }
        }

        private List<JobDetailsHistory> _jobDetailsHistory = new List<JobDetailsHistory>();
        public List<JobDetailsHistory> JobDetailsHistory
        {
            get { return _jobDetailsHistory; }
            set
            {
                if (_jobDetailsHistory != value)
                {
                    _jobDetailsHistory = value;
                    RaisePropertyChanged(nameof(JobDetailsHistory));
                }
            }
        }

        private List<JobDetailsModel> _displayJobDetails = new List<JobDetailsModel>();
        public List<JobDetailsModel> DisplayJobDetails
        {
            get { return _displayJobDetails; }
            set
            {
                if(_displayJobDetails != value)
                {
                    _displayJobDetails = value;
                    RaisePropertyChanged(nameof(DisplayJobDetails));
                }
            }
        }

        private List<JobDetailsHistory> _displayJobDetailsHistory = new List<JobDetailsHistory>();
        public List<JobDetailsHistory> DisplayJobDetailsHistory
        {
            get { return _displayJobDetailsHistory; }
            set
            {
                if (_displayJobDetailsHistory != value)
                {
                    _displayJobDetailsHistory = value;
                    RaisePropertyChanged(nameof(DisplayJobDetailsHistory));
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

        public CarrierDetails_VM()
        {
            Messenger.Default.Register<List<JobDetailsModel>>(this, UpdateJobDetailsModel);
            Messenger.Default.Register<CarrierDetails_VM>(this, UpdateDisplayJobDetailsModel);
            CarrierDetailsClosingCommand = new RelayCommand<EventArgsModel>(CarrierDetailsClosingCommandEvent);
        }

        private void UpdateJobDetailsModel(List<JobDetailsModel> jobDetails)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                JobDetails = new List<JobDetailsModel>();
                JobDetails = jobDetails;
            }));
        }
        
        private void UpdateDisplayJobDetailsModel(CarrierDetails_VM model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                DisplayJobDetails.Clear();
                DisplayJobDetailsHistory.Clear();
                IsJobDetailsVisible = Visibility.Collapsed;
                IsHistoryJobDetailsVisible = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(model.SetupJobModel.LotNo))
                {
                    SetupJobModel = model.SetupJobModel;
                    DisplayJobDetails = JobDetails.Where(x => x.JobID.Equals(SetupJobModel.JobID) && x.LotNo.Equals(SetupJobModel.LotNo)).ToList();
                    IsJobDetailsVisible = Visibility.Visible;
                }
                else if(model.JobDetailsHistory.Count > 0)
                {
                    DisplayJobDetailsHistory = model.JobDetailsHistory;
                    IsHistoryJobDetailsVisible = Visibility.Visible;
                }
            }));
        }

        private void CarrierDetailsClosingCommandEvent(EventArgsModel eventArgsModel)
        {
            CancelEventArgs cancelEventArgs = eventArgsModel.EventArgs as CancelEventArgs;
            cancelEventArgs.Cancel = true;

            Window window = eventArgsModel.Parameter as Window;

            window.Hide();
        }
    }
}
