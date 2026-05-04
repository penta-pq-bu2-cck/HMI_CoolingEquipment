using GalaSoft.MvvmLight;
using HMI_CoolingEquipment.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace HMI_CoolingEquipment.Interface
{
    public class Frame_InterfaceModel : ViewModelBase
    {
        public ICommand PentaIconCommand { get; set; }

        private string _contentHeader = string.Empty;
        public string ContentHeader
        {
            get { return _contentHeader; }
            set
            {
                if (_contentHeader != value)
                {
                    _contentHeader = value;
                    RaisePropertyChanged(nameof(ContentHeader));
                }
            }
        }

        private MenuModel _menuModel = new MenuModel();
        public MenuModel MenuModel
        {
            get { return _menuModel; }
            set
            {
                if (value != _menuModel)
                {
                    _menuModel = value;
                    RaisePropertyChanged(nameof(MenuModel));
                }
            }
        }

        private ContentControl _contentControl = null;
        public ContentControl ContentControl
        {
            get { return _contentControl; }
            set
            {
                if (_contentControl != value)
                {
                    _contentControl = value;
                    RaisePropertyChanged(nameof(ContentControl));
                }
            }
        }

        private UserAccountModel _userAccount = null;
        public UserAccountModel UserAccount
        {
            get { return _userAccount; }
            set
            {
                if (value != _userAccount)
                {
                    _userAccount = value;
                    RaisePropertyChanged(nameof(UserAccount));
                }
            }
        }

        private List<UserGroupAccessModel> _userGroupAccess = new List<UserGroupAccessModel>();
        public List<UserGroupAccessModel> UserGroupAccess
        {
            get { return _userGroupAccess; }
            set
            {
                if (value != _userGroupAccess)
                {
                    _userGroupAccess = value;
                    RaisePropertyChanged(nameof(UserGroupAccess));
                }
            }
        }

        private List<AlarmCurrentModel> _currentAlarmMessage = new List<AlarmCurrentModel>();
        public List<AlarmCurrentModel> CurrentAlarmMessage
        {
            get { return _currentAlarmMessage; }
            set
            {
                if (_currentAlarmMessage != value)
                {
                    _currentAlarmMessage = value;
                    RaisePropertyChanged(nameof(CurrentAlarmMessage));
                }
            }
        }
    }
}
