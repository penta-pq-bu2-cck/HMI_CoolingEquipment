using GalaSoft.MvvmLight;
using HMI_CoolingEquipment.ViewModels;
using System.Collections.Generic;

namespace HMI_CoolingEquipment.Models
{
    public class RackModel : ViewModelBase
    {
        private List<RackLoadPortList> _trayRack = new List<RackLoadPortList>();
        public List<RackLoadPortList> TrayRack
        {
            get { return _trayRack; }
            set
            {
                if (value != _trayRack)
                {
                    _trayRack = value;
                    RaisePropertyChanged(nameof(TrayRack));
                }
            }
        }

        private List<RackLoadPortList> _tubeRack = new List<RackLoadPortList>();
        public List<RackLoadPortList> TubeRack
        {
            get { return _tubeRack; }
            set
            {
                if (value != _tubeRack)
                {
                    _tubeRack = value;
                    RaisePropertyChanged(nameof(TubeRack));
                }
            }
        }
    }
}
