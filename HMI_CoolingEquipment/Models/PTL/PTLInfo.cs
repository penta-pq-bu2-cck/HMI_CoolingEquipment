using GalaSoft.MvvmLight;

namespace HMI_CoolingEquipment.Models
{
    public class PTLInfo : ViewModelBase
    {
        public m_GatewayConfigType[] m_GatewayConfig;
        public struct m_GatewayConfigType
        {
            public int gateway_id;
            public string ip;
            public int port;
            public int statusCode;
        }
        
        private int _gatewayCount = 0;
        public int GatewayCount
        {
            get { return _gatewayCount; }
            set
            {
                if(_gatewayCount != value)
                {
                    _gatewayCount = value;
                    RaisePropertyChanged(nameof(GatewayCount));
                }
            }
        }

        private int[] _diagnoseGatewayTimeOut;
        public int[] DiagnoseGatewayTimeOut
        {
            get { return _diagnoseGatewayTimeOut; }
            set
            {
                if(_diagnoseGatewayTimeOut != value)
                {
                    _diagnoseGatewayTimeOut = value;
                    RaisePropertyChanged(nameof(DiagnoseGatewayTimeOut));
                }
            }
        }
        
        private bool[] _diagnoseGatewayOk;
        public bool[] DiagnoseGatewayOk
        {
            get { return _diagnoseGatewayOk; }
            set
            {
                if(_diagnoseGatewayOk != value)
                {
                    _diagnoseGatewayOk = value;
                    RaisePropertyChanged(nameof(DiagnoseGatewayOk));
                }
            }
        }

        private bool _setBestPoll = false;
        public bool SetBestPoll
        {
            get { return _setBestPoll; }
            set
            {
                if(_setBestPoll != value)
                {
                    _setBestPoll = value;
                    RaisePropertyChanged(nameof(SetBestPoll));
                }
            }
        }
        
        private int _scanTimer = 200;
        public int ScanTimer
        {
            get { return _scanTimer; }
            set
            {
                if(_scanTimer != value)
                {
                    _scanTimer = value;
                    RaisePropertyChanged(nameof(ScanTimer));
                }
            }
        }

        private bool _autoScan = true;
        public bool AutoScan
        {
            get { return _autoScan; }
            set
            {
                if (_autoScan != value)
                {
                    _autoScan = value;
                    RaisePropertyChanged(nameof(AutoScan));
                }
            }
        }

        private bool _initClearTags = false;
        public bool InitClearTags
        {
            get { return _initClearTags; }
            set
            {
                if (_initClearTags != value)
                {
                    _initClearTags = value;
                    RaisePropertyChanged(nameof(InitClearTags));
                }
            }
        }
    }
}
