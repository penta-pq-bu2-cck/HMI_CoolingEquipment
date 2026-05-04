using GalaSoft.MvvmLight;
using System.Windows.Input;

namespace HMI_CoolingEquipment.Models
{
    public class LoadPortModel : ViewModelBase
    {
        public ICommand LoadPortIsEnableCommand { get; set; }

        private string _loadPortID = string.Empty;
        public string LoadPortID
        {
            get { return _loadPortID; }
            set
            {
                if(_loadPortID != value)
                {
                    _loadPortID = value;
                    RaisePropertyChanged(nameof(LoadPortID));
                }
            }
        }
        
        private string _loadPortRack = string.Empty;
        public string LoadPortRack
        {
            get { return _loadPortRack; }
            set
            {
                if(_loadPortRack != value)
                {
                    _loadPortRack = value;
                    RaisePropertyChanged(nameof(LoadPortRack));
                }
            }
        }
        
        private string _loadPortColumn = string.Empty;
        public string LoadPortColumn
        {
            get { return _loadPortColumn; }
            set
            {
                if(_loadPortColumn != value)
                {
                    _loadPortColumn = value;
                    RaisePropertyChanged(nameof(LoadPortColumn));
                }
            }
        }
        
        private string _loadPortRow = string.Empty;
        public string LoadPortRow
        {
            get { return _loadPortRow; }
            set
            {
                if(_loadPortRow != value)
                {
                    _loadPortRow = value;
                    RaisePropertyChanged(nameof(LoadPortRow));
                }
            }
        }
        
        private string _loadPortPriority = string.Empty;
        public string LoadPortPriority
        {
            get { return _loadPortPriority; }
            set
            {
                if(_loadPortPriority != value)
                {
                    _loadPortPriority = value;
                    RaisePropertyChanged(nameof(LoadPortPriority));
                }
            }
        }
        
        private string _loadPortType = string.Empty;
        public string LoadPortType
        {
            get { return _loadPortType; }
            set
            {
                if(_loadPortType != value)
                {
                    _loadPortType = value;
                    RaisePropertyChanged(nameof(LoadPortType));
                }
            }
        }
        
        private string? _loadPortCarrierLoad = string.Empty;
        public string? LoadPortCarrierLoad
        {
            get { return _loadPortCarrierLoad; }
            set
            {
                if(_loadPortCarrierLoad != value)
                {
                    _loadPortCarrierLoad = value;
                    RaisePropertyChanged(nameof(LoadPortCarrierLoad));
                }
            }
        }

        private string? _loadPortIPAddress = string.Empty;
        public string? LoadPortIPAddress
        {
            get { return _loadPortIPAddress; }
            set
            {
                if(_loadPortIPAddress != value)
                {
                    _loadPortIPAddress = value;
                    RaisePropertyChanged(nameof(LoadPortIPAddress));
                }
            }
        }
        
        private int? _loadPortNodeAddress = 0;
        public int? LoadPortNodeAddress
        {
            get { return _loadPortNodeAddress; }
            set
            {
                if(_loadPortNodeAddress != value)
                {
                    _loadPortNodeAddress = value;
                    RaisePropertyChanged(nameof(LoadPortNodeAddress));
                }
            }
        }
        
        private bool _loadPortNodeConnectionStatus = false;
        public bool LoadPortNodeConnectionStatus
        {
            get { return _loadPortNodeConnectionStatus; }
            set
            {
                if(_loadPortNodeConnectionStatus != value)
                {
                    _loadPortNodeConnectionStatus = value;
                    RaisePropertyChanged(nameof(LoadPortNodeConnectionStatus));
                }
            }
        }

        private bool _loadPortError = false;
        public bool LoadPortError
        {
            get { return _loadPortError; }
            set
            {
                if (_loadPortError != value)
                {
                    _loadPortError = value;
                    RaisePropertyChanged(nameof(LoadPortError));
                }
            }
        }

        private LoadPortState _loadPortState = LoadPortState.Empty;
        public LoadPortState LoadPortState
        {
            get { return _loadPortState; }
            set
            {
                if (_loadPortState != value)
                {
                    _loadPortState = value;
                    RaisePropertyChanged(nameof(LoadPortState));
                }
            }
        }

        private string? _preAssignLoad = string.Empty;
        public string? PreAssignLoad
        {
            get { return _preAssignLoad; }
            set
            {
                if (_preAssignLoad != value)
                {
                    _preAssignLoad = value;
                    RaisePropertyChanged(nameof(PreAssignLoad));
                }
            }
        }

        private bool _loadPortIsEnable = true;
        public bool LoadPortIsEnable
        {
            get { return _loadPortIsEnable; }
            set
            {
                if (_loadPortIsEnable != value)
                {
                    _loadPortIsEnable = value;
                    RaisePropertyChanged(nameof(LoadPortIsEnable));
                }
            }
        }

    }
}
