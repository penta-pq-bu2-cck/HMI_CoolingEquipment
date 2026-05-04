using GalaSoft.MvvmLight;

namespace HMI_CoolingEquipment.Models
{
    public class UserAccountModel : ViewModelBase
    {
        private string _userName = string.Empty;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if(_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged(nameof(UserName));
                }
            }
        }
        
        private string _password = string.Empty;
        public string Password
        {
            get { return _password; }
            set
            {
                if(_password != value)
                {
                    _password = value;
                    RaisePropertyChanged(nameof(Password));
                }
            }
        }
        
        private string _firstName = string.Empty;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if(_firstName != value)
                {
                    _firstName = value;
                    RaisePropertyChanged(nameof(FirstName));
                }
            }
        }
        
        private string _lastName = string.Empty;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if(_lastName != value)
                {
                    _lastName = value;
                    RaisePropertyChanged(nameof(LastName));
                }
            }
        }
        
        private string _userGroup = string.Empty;
        public string UserGroup
        {
            get { return _userGroup; }
            set
            {
                if(_userGroup != value)
                {
                    _userGroup = value;
                    RaisePropertyChanged(nameof(UserGroup));
                }
            }
        }
    }
}
