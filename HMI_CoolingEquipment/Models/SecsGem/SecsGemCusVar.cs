using GalaSoft.MvvmLight;
using System.Windows.Input;

namespace HMI_CoolingEquipment.Models
{
    public class SecsGemCusVar : ViewModelBase
    {
        public ICommand SelectVarCommand { get; set; }

        private string _varID = string.Empty;
        public string VarID
        {
            get { return _varID; }
            set
            {
                if (value != _varID)
                {
                    _varID = value;
                    RaisePropertyChanged(nameof(VarID));
                }
            }
        }

        private string _varName = string.Empty;
        public string VarName
        {
            get { return _varName; }
            set
            {
                if (value != _varName)
                {
                    _varName = value;
                    RaisePropertyChanged(nameof(VarName));
                }
            }
        }

        private string _varGroup = string.Empty;
        public string VarGroup
        {
            get { return _varGroup; }
            set
            {
                if (value != _varGroup)
                {
                    _varGroup = value;
                    RaisePropertyChanged(nameof(VarGroup));
                }
            }
        }

        private string _varDesc = string.Empty;
        public string VarDesc
        {
            get { return _varDesc; }
            set
            {
                if (value != _varDesc)
                {
                    _varDesc = value;
                    RaisePropertyChanged(nameof(VarDesc));
                }
            }
        }

        private string _varType = string.Empty;
        public string VarType
        {
            get { return _varType; }
            set
            {
                if (value != _varType)
                {
                    _varType = value;
                    RaisePropertyChanged(nameof(VarType));
                }
            }
        }

        private string _varFormat = string.Empty;
        public string VarFormat
        {
            get { return _varFormat; }
            set
            {
                if (value != _varFormat)
                {
                    _varFormat = value;
                    RaisePropertyChanged(nameof(VarFormat));
                }
            }
        }

        private string _varVal = string.Empty;
        public string VarVal
        {
            get { return _varVal; }
            set
            {
                if (value != _varVal)
                {
                    _varVal = value;
                    RaisePropertyChanged(nameof(VarVal));
                }
            }
        }

        private string _varDefaultVal = string.Empty;
        public string VarDefaultVal
        {
            get { return _varDefaultVal; }
            set
            {
                if (value != _varDefaultVal)
                {
                    _varDefaultVal = value;
                    RaisePropertyChanged(nameof(VarDefaultVal));
                }
            }
        }

        private string _varMin = string.Empty;
        public string VarMin
        {
            get { return _varMin; }
            set
            {
                if (value != _varMin)
                {
                    _varMin = value;
                    RaisePropertyChanged(nameof(VarMin));
                }
            }
        }

        private string _varMax = string.Empty;
        public string VarMax
        {
            get { return _varMax; }
            set
            {
                if (value != _varMax)
                {
                    _varMax = value;
                    RaisePropertyChanged(nameof(VarMax));
                }
            }
        }

        private string _varUnit = string.Empty;
        public string VarUnit
        {
            get { return _varUnit; }
            set
            {
                if (value != _varUnit)
                {
                    _varUnit = value;
                    RaisePropertyChanged(nameof(VarUnit));
                }
            }
        }

        private string _varParentKey = string.Empty;
        public string VarParentKey
        {
            get { return _varParentKey; }
            set
            {
                if (value != _varParentKey)
                {
                    _varParentKey = value;
                    RaisePropertyChanged(nameof(VarParentKey));
                }
            }
        }

        private string _varAccessId = string.Empty;
        public string VarAccessId
        {
            get { return _varAccessId; }
            set
            {
                if (value != _varAccessId)
                {
                    _varAccessId = value;
                    RaisePropertyChanged(nameof(VarAccessId));
                }
            }
        }

        private string _varOrderByAccessId = string.Empty;
        public string VarOrderByAccessId
        {
            get { return _varOrderByAccessId; }
            set
            {
                if (value != _varOrderByAccessId)
                {
                    _varOrderByAccessId = value;
                    RaisePropertyChanged(nameof(VarOrderByAccessId));
                }
            }
        }

        private string _varAllowDuplicates = string.Empty;
        public string VarAllowDuplicates
        {
            get { return _varAllowDuplicates; }
            set
            {
                if (value != _varAllowDuplicates)
                {
                    _varAllowDuplicates = value;
                    RaisePropertyChanged(nameof(VarAllowDuplicates));
                }
            }
        }
    }
}
