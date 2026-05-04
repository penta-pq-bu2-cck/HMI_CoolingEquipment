using GalaSoft.MvvmLight;

namespace HMI_CoolingEquipment.Models
{
    public class LogValue : ViewModelBase
    {
        private string _value = string.Empty;
        public string Value
        {
            get { return _value; }
            set
            {
                if (string.IsNullOrEmpty(_value))
                {
                    _value = value;
                    RaisePropertyChanged(nameof(Value));
                }
            }
        }
        
        private HMIPage _page = HMIPage.MainPage;
        public HMIPage Page
        {
            get { return _page; }
            set
            {
                if (value != _page)
                {
                    _page = value;
                    RaisePropertyChanged(nameof(Page));
                }
            }
        }
    }
}
