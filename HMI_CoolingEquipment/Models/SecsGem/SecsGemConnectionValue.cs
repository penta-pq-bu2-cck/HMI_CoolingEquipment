using GalaSoft.MvvmLight;

namespace HMI_CoolingEquipment.Models
{
    public class SecsGemConnectionValue : ViewModelBase
    {
        private string _connectionStatus = "Disconnected";
        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                if(_connectionStatus != value)
                {
                    _connectionStatus = value;
                    RaisePropertyChanged(nameof(ConnectionStatus));
                }
            }
        }

        private bool _isConnected = false;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    RaisePropertyChanged(nameof(IsConnected));
                }
            }
        }
    }
}
