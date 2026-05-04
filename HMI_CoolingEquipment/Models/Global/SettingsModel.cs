using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace HMI_CoolingEquipment.Models
{
    public class SettingsModel : ViewModelBase
    {
        private string _items = string.Empty;
        public string Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    RaisePropertyChanged(nameof(Items));
                }
            }
        }

        private string _value = string.Empty;
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged(nameof(Value));
                }
            }
        }

        private List<string> _comboBoxValues = new List<string>();
        public List<string> ComboBoxValues
        {
            get { return _comboBoxValues; }
            set
            {
                if (_comboBoxValues != value)
                {
                    _comboBoxValues = value;
                    RaisePropertyChanged(nameof(ComboBoxValues));
                }
            }
        }

        private SettingsType _type;
        public SettingsType Type
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

        private HMIPage _page;
        public HMIPage Page
        {
            get { return _page; }
            set
            {
                if (_page != value)
                {
                    _page = value;
                    RaisePropertyChanged(nameof(Page));
                }
            }
        }
    }
}
