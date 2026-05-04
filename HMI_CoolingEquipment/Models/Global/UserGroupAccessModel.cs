using GalaSoft.MvvmLight;

namespace HMI_CoolingEquipment.Models
{
    public class UserGroupAccessModel : ViewModelBase
    {
        private string _userGroup = string.Empty;
        public string UserGroup
        {
            get { return _userGroup; }
            set
            {
                if (_userGroup != value)
                {
                    _userGroup = value;
                    RaisePropertyChanged(nameof(UserGroup));
                }
            }
        }

        private bool _lotStatusPageAccess = true;
        public bool LotStatusPageAccess
        {
            get { return _lotStatusPageAccess; }
            set
            {
                if (_lotStatusPageAccess != value)
                {
                    _lotStatusPageAccess = value;
                    RaisePropertyChanged(nameof(LotStatusPageAccess));
                }
            }
        }

        private bool _loadingPageAccess = true;
        public bool LoadingPageAccess
        {
            get { return _loadingPageAccess; }
            set
            {
                if (_loadingPageAccess != value)
                {
                    _loadingPageAccess = value;
                    RaisePropertyChanged(nameof(LoadingPageAccess));
                }
            }
        }
        
        private bool _unloadingPageAccess = true;
        public bool UnloadingPageAccess
        {
            get { return _unloadingPageAccess; }
            set
            {
                if (_unloadingPageAccess != value)
                {
                    _unloadingPageAccess = value;
                    RaisePropertyChanged(nameof(UnloadingPageAccess));
                }
            }
        }
        
        private bool _secsGemPageAccess = true;
        public bool SecsGemPageAccess
        {
            get { return _secsGemPageAccess; }
            set
            {
                if (_secsGemPageAccess != value)
                {
                    _secsGemPageAccess = value;
                    RaisePropertyChanged(nameof(SecsGemPageAccess));
                }
            }
        }
        
        private bool _settingPageAccess = true;
        public bool SettingPageAccess
        {
            get { return _settingPageAccess; }
            set
            {
                if (_settingPageAccess != value)
                {
                    _settingPageAccess = value;
                    RaisePropertyChanged(nameof(SettingPageAccess));
                }
            }
        }
        
        private bool _ptlPageAccess = true;
        public bool PTLPageAccess
        {
            get { return _ptlPageAccess; }
            set
            {
                if (_ptlPageAccess != value)
                {
                    _ptlPageAccess = value;
                    RaisePropertyChanged(nameof(PTLPageAccess));
                }
            }
        }
        
        private bool _editUserGroupAccess = true;
        public bool EditUserGroupAccess
        {
            get { return _editUserGroupAccess; }
            set
            {
                if (_editUserGroupAccess != value)
                {
                    _editUserGroupAccess = value;
                    RaisePropertyChanged(nameof(EditUserGroupAccess));
                }
            }
        }
        
        private bool _editUserAccountList = true;
        public bool EditUserAccountList
        {
            get { return _editUserAccountList; }
            set
            {
                if (_editUserAccountList != value)
                {
                    _editUserAccountList = value;
                    RaisePropertyChanged(nameof(EditUserAccountList));
                }
            }
        }

    }
}
