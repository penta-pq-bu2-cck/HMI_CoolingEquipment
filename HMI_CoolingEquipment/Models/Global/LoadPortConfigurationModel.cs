using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace HMI_CoolingEquipment.Models
{
    public class LoadPortConfigurationModel : ViewModelBase
    {
        private string _loadPortStatusKey = string.Empty;
        public string LoadPortStatusKey
        {
            get { return _loadPortStatusKey; }
            set
            {
                if(_loadPortStatusKey != value)
                {
                    _loadPortStatusKey = value;
                    RaisePropertyChanged(nameof(LoadPortStatusKey));
                }
            }
        }

        private LoadPortState _loadPortStatusState;
        public LoadPortState LoadPortStatusState
        {
            get { return _loadPortStatusState; }
            set
            {
                if(_loadPortStatusState != value)
                {
                    _loadPortStatusState = value;
                    RaisePropertyChanged(nameof(LoadPortStatusState));
                }
            }
        }

        private LEDColour _loadPortLEDColour;
        public LEDColour LoadPortLEDColour
        {
            get { return _loadPortLEDColour; }
            set
            {
                if( _loadPortLEDColour != value)
                {
                    _loadPortLEDColour = value;
                    RaisePropertyChanged(nameof(LoadPortLEDColour));
                }
            }
        }

        private LEDState _loadPortLEDState;
        public LEDState LoadPortLEDState
        {
            get { return _loadPortLEDState; }
            set
            {
                if(_loadPortLEDState != value)
                {
                    _loadPortLEDState = value;
                    RaisePropertyChanged(nameof(LoadPortLEDState));
                }
            }
        }

        private List<string> _comboBoxValues = new List<string>();
        public List<string> ComboBoxValues
        {
            get { return _comboBoxValues; }
            set
            {
                if (value != _comboBoxValues)
                {
                    _comboBoxValues = value;
                    RaisePropertyChanged(nameof(ComboBoxValues));
                }
            }
        }
    }
}
