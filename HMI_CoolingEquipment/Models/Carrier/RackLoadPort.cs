using GalaSoft.MvvmLight;
using System;
using System.Windows;
using System.Windows.Media;

namespace HMI_CoolingEquipment.Models
{
    public class RackLoadPort : ViewModelBase
    {
        private string _loadPortID = string.Empty;
        public string LoadPortD
        {
            get { return _loadPortID; }
            set
            {
                if (_loadPortID != value)
                {
                    _loadPortID = value;
                    RaisePropertyChanged(nameof(LoadPortD));
                }
            }
        }

        private Tuple<LoadPortState, bool, bool, bool> _loadPortStateAndConnection = new Tuple<LoadPortState, bool, bool, bool>(LoadPortState.Empty, false, false, false);
        public Tuple<LoadPortState, bool, bool, bool> LoadPortStateAndConnection
        {
            get { return _loadPortStateAndConnection; }
            set
            {
                if (_loadPortStateAndConnection != value)
                {
                    _loadPortStateAndConnection = value;
                    RaisePropertyChanged(nameof(LoadPortStateAndConnection));
                }
            }
        }

        private SolidColorBrush _loadPortColour = Brushes.Transparent;
        public SolidColorBrush LoadPortColour
        {
            get { return _loadPortColour; }
            set
            {
                if (_loadPortColour != value)
                {
                    _loadPortColour = value;
                    RaisePropertyChanged(nameof(LoadPortColour));
                }
            }
        }

        private bool _loadPortIsBlink = false;
        public bool LoadPortIsBlink
        {
            get { return _loadPortIsBlink; }
            set
            {
                if (_loadPortIsBlink != value)
                {
                    _loadPortIsBlink = value;
                    RaisePropertyChanged(nameof(LoadPortIsBlink));
                }
            }
        }

        private Visibility _loadPortVisibility = Visibility.Visible;
        public Visibility LoadPortVisibility
        {
            get { return _loadPortVisibility; }
            set
            {
                if (_loadPortVisibility != value)
                {
                    _loadPortVisibility = value;
                    RaisePropertyChanged(nameof(LoadPortVisibility));
                }
            }
        }
    }
}
