using GalaSoft.MvvmLight;
using System;

namespace HMI_CoolingEquipment
{
    public class AlarmCurrentModel : ViewModelBase
    {
        private DateTime _alarmDateTime;
        public DateTime AlarmDateTime
        {
            get { return _alarmDateTime; }
            set
            {
                if (_alarmDateTime != value)
                {
                    _alarmDateTime = value;
                    RaisePropertyChanged(nameof(AlarmDateTime));
                }
            }
        }

        private AlarmType _alarmType = AlarmType.Warning;
        public AlarmType AlarmType
        {
            get { return _alarmType; }
            set
            {
                if (_alarmType != value)
                {
                    _alarmType = value;
                    RaisePropertyChanged(nameof(AlarmType));
                }
            }
        }

        private int _alarmCode = 0;
        public int AlarmCode
        {
            get { return _alarmCode; }
            set
            {
                if (_alarmCode != value)
                {
                    _alarmCode = value;
                    RaisePropertyChanged(nameof(AlarmCode));
                }
            }
        }
        
        private string _alarmModule = "Spare";
        public string AlarmModule
        {
            get { return _alarmModule; }
            set
            {
                if (_alarmModule != value)
                {
                    _alarmModule = value;
                    RaisePropertyChanged(nameof(AlarmModule));
                }
            }
        }
        
        private string _alarmMessage = "Spare";
        public string AlarmMessage
        {
            get { return _alarmMessage; }
            set
            {
                if (_alarmMessage != value)
                {
                    _alarmMessage = value;
                    RaisePropertyChanged(nameof(AlarmMessage));
                }
            }
        }
        
        private string _alarmAction = "Please contact administrator";
        public string AlarmAction
        {
            get { return _alarmAction; }
            set
            {
                if (_alarmAction != value)
                {
                    _alarmAction = value;
                    RaisePropertyChanged(nameof(AlarmAction));
                }
            }
        }

    }
}
