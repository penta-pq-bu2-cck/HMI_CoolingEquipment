using System;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using System.Windows;

namespace HMI_CoolingEquipment.Models
{
    public class SetupJobModel : ViewModelBase
    {
        public ICommand DataGridImageClickCommand { get; set; }
        public ICommand DataGridButtonClickCommand { get; set; }
        public ICommand DataGridPurgeButtonClickCommand { get; set; }
        public ICommand DataGridAbortButtonClickCommand { get; set; }
        public ICommand NonOpr_DataGridPurgeButtonClickCommand { get; set; }
        public ICommand NonOpr_DataGridAbortButtonClickCommand { get; set; }

        private Visibility _setupJobButtonView = Visibility.Collapsed;
        public Visibility SetupJobButtonView
        {
            get { return _setupJobButtonView; }
            set
            {
                if (_setupJobButtonView != value)
                {
                    _setupJobButtonView = value;
                    RaisePropertyChanged(nameof(SetupJobButtonView));
                }
            }
        }

        private string _jobID = string.Empty;
        public string JobID
        {
            get { return _jobID; }
            set
            {
                if(_jobID != value)
                {
                    _jobID = value;
                    RaisePropertyChanged(nameof(JobID));
                }
            }
        }

        private string _lotNo = string.Empty;
        public string LotNo
        {
            get { return _lotNo; }
            set
            {
                if(_lotNo != value)
                {
                    _lotNo = value;
                    RaisePropertyChanged(nameof(LotNo));
                }
            }
        }
        
        private string _coolingTime = string.Empty;
        public string CoolingTime
        {
            get { return _coolingTime; }
            set
            {
                if(_coolingTime != value)
                {
                    _coolingTime = value;
                    RaisePropertyChanged(nameof(CoolingTime));
                }
            }
        }
        
        private string _stagingTime = string.Empty;
        public string StagingTime
        {
            get { return _stagingTime; }
            set
            {
                if(_stagingTime != value)
                {
                    _stagingTime = value;
                    RaisePropertyChanged(nameof(StagingTime));
                }
            }
        }
        
        private DateTime? ld_START = null;
        public DateTime? LD_START
        {
            get { return ld_START; }
            set
            {
                if(ld_START != value)
                {
                    ld_START = value;
                    RaisePropertyChanged(nameof(LD_START));
                }
            }
        }
        
        private DateTime? ld_EXPECTED_END = null;
        public DateTime? LD_EXPECTED_END
        {
            get { return ld_EXPECTED_END; }
            set
            {
                if(ld_EXPECTED_END != value)
                {
                    ld_EXPECTED_END = value;
                    RaisePropertyChanged(nameof(LD_EXPECTED_END));
                }
            }
        }
        
        private DateTime? ld_END = null;
        public DateTime? LD_END
        {
            get { return ld_END; }
            set
            {
                if(ld_END != value)
                {
                    ld_END = value;
                    RaisePropertyChanged(nameof(LD_END));
                }
            }
        }
        
        private DateTime? ct_START = null;
        public DateTime? CT_START
        {
            get { return ct_START; }
            set
            {
                if(ct_START != value)
                {
                    ct_START = value;
                    RaisePropertyChanged(nameof(CT_START));
                }
            }
        }
        
        private DateTime? ct_END = null;
        public DateTime? CT_END
        {
            get { return ct_END; }
            set
            {
                if(ct_END != value)
                {
                    ct_END = value;
                    RaisePropertyChanged(nameof(CT_END));
                }
            }
        }
        
        private DateTime? st_START = null;
        public DateTime? ST_START
        {
            get { return st_START; }
            set
            {
                if(st_START != value)
                {
                    st_START = value;
                    RaisePropertyChanged(nameof(ST_START));
                }
            }
        }
        
        private DateTime? st_EXPECTED_END = null;
        public DateTime? ST_EXPECTED_END
        {
            get { return st_EXPECTED_END; }
            set
            {
                if(st_EXPECTED_END != value)
                {
                    st_EXPECTED_END = value;
                    RaisePropertyChanged(nameof(ST_EXPECTED_END));
                }
            }
        }
                
        private DateTime? st_END = null;
        public DateTime? ST_END
        {
            get { return st_END; }
            set
            {
                if(st_END != value)
                {
                    st_END = value;
                    RaisePropertyChanged(nameof(ST_END));
                }
            }
        }
                
        private DateTime? _createdOn = null;
        public DateTime? CreatedOn
        {
            get { return _createdOn; }
            set
            {
                if(_createdOn != value)
                {
                    _createdOn = value;
                    RaisePropertyChanged(nameof(CreatedOn));
                }
            }
        }
        
        private DateTime? _completedOn = null;
        public DateTime? CompletedOn
        {
            get { return _completedOn; }
            set
            {
                if(_completedOn != value)
                {
                    _completedOn = value;
                    RaisePropertyChanged(nameof(CompletedOn));
                }
            }
        }

        private JobStatus? _jobStatus;
        public JobStatus? JobStatus
        {
            get { return _jobStatus; }
            set
            {
                if (_jobStatus != value)
                {
                    _jobStatus = value;
                    RaisePropertyChanged(nameof(JobStatus));
                }
            }
        }
        
        private int _noOfCarrier;
        public int NoOfCarrier
        {
            get { return _noOfCarrier; }
            set
            {
                if (_noOfCarrier != value)
                {
                    _noOfCarrier = value;
                    RaisePropertyChanged(nameof(NoOfCarrier));
                }
            }
        }

        private bool _isJobError = false;
        public bool IsJobError
        {
            get { return _isJobError; }
            set
            {
                if (_isJobError != value)
                {
                    _isJobError = value;
                    RaisePropertyChanged(nameof(IsJobError));
                }
            }
        } 
    }
}
