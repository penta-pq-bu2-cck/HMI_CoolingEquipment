using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace HMI_CoolingEquipment.Models
{
    public class RackLoadPortList : ViewModelBase
    {
        private string _rackName = string.Empty;
        public string RackName
        {
            get { return _rackName; }
            set
            {
                if (_rackName != value)
                {
                    _rackName = value;
                    RaisePropertyChanged(nameof(RackName));
                }
            }
        }

        private List<RackLoadPort> _rackLoadPorts = new List<RackLoadPort>();
        public List<RackLoadPort> RackLoadPorts
        {
            get { return _rackLoadPorts; }
            set
            {
                if (_rackLoadPorts != value)
                {
                    _rackLoadPorts = value;
                    RaisePropertyChanged(nameof(RackLoadPorts));
                }
            }
        }
    }
}
