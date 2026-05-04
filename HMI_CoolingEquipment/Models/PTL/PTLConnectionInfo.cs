using GalaSoft.MvvmLight;
using System.Windows.Input;

namespace HMI_CoolingEquipment.Models
{
    public class PTLConnectionInfo : ViewModelBase
    {
        public ICommand ConnectPTLCommand { get; set; }
        public ICommand DisconnectPTLCommand { get; set; }

        private int _gatewayID = 0;
        public int GatewayID
        {
            get { return _gatewayID; }
            set
            {
                if (_gatewayID != value)
                {
                    _gatewayID = value;
                    RaisePropertyChanged(nameof(GatewayID));
                }
            }
        }

        private int _gatewayPort = 0;
        public int GatewayPort
        {
            get { return _gatewayPort; }
            set
            {
                if (_gatewayPort != value)
                {
                    _gatewayPort = value;
                    RaisePropertyChanged(nameof(GatewayPort));
                }
            }
        }

        private string _gatewayIP = string.Empty;
        public string GatewayIP
        {
            get { return _gatewayIP; }
            set
            {
                if (_gatewayIP != value)
                {
                    _gatewayIP = value;
                    RaisePropertyChanged(nameof(GatewayIP));
                }
            }
        }

        private int _gatewayConnectionCode = 0;
        public int GatewayConnectionCode
        {
            get { return _gatewayConnectionCode; }
            set
            {
                if (_gatewayConnectionCode != value)
                {
                    _gatewayConnectionCode = value;
                    RaisePropertyChanged(nameof(GatewayConnectionCode));
                }
            }
        }
        private string _gatewayConnectionValue = string.Empty;
        public string GatewayConnectionValue
        {
            get { return _gatewayConnectionValue; }
            set
            {
                if (_gatewayConnectionValue != value)
                {
                    _gatewayConnectionValue = value;
                    RaisePropertyChanged(nameof(GatewayConnectionValue));
                }
            }
        }
    }
}
