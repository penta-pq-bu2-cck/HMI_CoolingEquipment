using GalaSoft.MvvmLight;
using System.Windows.Input;

namespace HMI_CoolingEquipment.Models
{
    public class SecsGemCusEvent : ViewModelBase
    {
        public ICommand SelectEventCommand { get; set; }

        private string _eventID = string.Empty;
        public string EventID
        {
            get { return _eventID; }
            set
            {
                if(value != _eventID)
                {
                    _eventID = value;
                    RaisePropertyChanged(nameof(EventID));
                }
            }
        }

        private string _eventName = string.Empty;
        public string EventName
        {
            get { return _eventName; }
            set
            {
                if (value != _eventName)
                {
                    _eventName = value;
                    RaisePropertyChanged(nameof(EventName));
                }
            }
        }
        
        private string _eventGroup = string.Empty;
        public string EventGroup
        {
            get { return _eventGroup; }
            set
            {
                if (value != _eventGroup)
                {
                    _eventGroup = value;
                    RaisePropertyChanged(nameof(EventGroup));
                }
            }
        }
        
        private string _eventText = string.Empty;
        public string EventText
        {
            get { return _eventText; }
            set
            {
                if (value != _eventText)
                {
                    _eventText = value;
                    RaisePropertyChanged(nameof(EventText));
                }
            }
        }
        
        private string _eventEnable = string.Empty;
        public string EventEnable
        {
            get { return _eventEnable; }
            set
            {
                if (value != _eventEnable)
                {
                    _eventEnable = value;
                    RaisePropertyChanged(nameof(EventEnable));
                }
            }
        }
    }
}
