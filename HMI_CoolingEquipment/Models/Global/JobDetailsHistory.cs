using GalaSoft.MvvmLight;
using System;

namespace HMI_CoolingEquipment.Models
{
    public class JobDetailsHistory : ViewModelBase
    {
        private string _carrierID = string.Empty;
        public string CarrierID
        {
            get { return _carrierID; }
            set
            {
                if (_carrierID != value)
                {
                    _carrierID = value;
                    RaisePropertyChanged(nameof(CarrierID));
                }
            }
        }

        private string _portLocation = string.Empty;
        public string PortLocation
        {
            get { return _portLocation; }
            set
            {
                if (_portLocation != value)
                {
                    _portLocation = value;
                    RaisePropertyChanged(nameof(PortLocation));
                }
            }
        }

        private DateTime? _loadingTime = null;
        public DateTime? LoadingTime
        {
            get { return _loadingTime; }
            set
            {
                if (_loadingTime != value)
                {
                    _loadingTime = value;
                    RaisePropertyChanged(nameof(LoadingTime));
                }
            }
        }

        private string _loadBy = string.Empty;
        public string LoadBy
        {
            get { return _loadBy; }
            set
            {
                if (_loadBy != value)
                {
                    _loadBy = value;
                    RaisePropertyChanged(nameof(LoadBy));
                }
            }
        }

        private DateTime? _unloadingTime = null;
        public DateTime? UnloadingTime
        {
            get { return _unloadingTime; }
            set
            {
                if (_unloadingTime != value)
                {
                    _unloadingTime = value;
                    RaisePropertyChanged(nameof(UnloadingTime));
                }
            }
        }

        private string _unloadBy = string.Empty;
        public string UnloadBy
        {
            get { return _unloadBy; }
            set
            {
                if (_unloadBy != value)
                {
                    _unloadBy = value;
                    RaisePropertyChanged(nameof(UnloadBy));
                }
            }
        }

        private string _type = string.Empty;
        public string Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    RaisePropertyChanged(nameof(Type));
                }
            }
        }
    }
}
