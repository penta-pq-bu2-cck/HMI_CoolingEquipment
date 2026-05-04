using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using HMI_CoolingEquipment.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace HMI_CoolingEquipment.ViewModels
{
    public class UserAccount_VM : ViewModelBase
    {
        private readonly UserAccountSignInView userAccountSignInView = new UserAccountSignInView(new UserAccountSignIn_VM());
        private readonly UserGroupAccessEditorView userGroupAccessEditorView = new UserGroupAccessEditorView(new UserGroupAccessEditor_VM());
        private readonly UserAccountEditorView userAccountEditorView = new UserAccountEditorView(new UserAccountEditor_VM());

        public ICommand UserAccountSignInCommand { get; set; }
        public ICommand UserAccountSignOutCommand { get; set; }
        public ICommand EditUserGroupAccessCommand { get; set; }
        public ICommand EditUserAccountListCommand { get; set; }

        private Visibility _loginBtn = Visibility.Collapsed;
        public Visibility LoginBtn
        {
            get { return _loginBtn; }
            set
            {
                if (_loginBtn != value)
                {
                    _loginBtn = value;
                    RaisePropertyChanged(nameof(LoginBtn));
                }
            }
        }
        
        private Visibility _userBtn = Visibility.Collapsed;
        public Visibility UserBtn
        {
            get { return _userBtn; }
            set
            {
                if (_userBtn != value)
                {
                    _userBtn = value;
                    RaisePropertyChanged(nameof(UserBtn));
                }
            }
        }

        private string _fullName = string.Empty;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    RaisePropertyChanged(nameof(FullName));
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
        
        private UserGroupAccessModel _userGroupAccessModel = new UserGroupAccessModel();
        public UserGroupAccessModel UserGroupAccessModel
        {
            get { return _userGroupAccessModel; }
            set
            {
                if (value != _userGroupAccessModel)
                {
                    _userGroupAccessModel = value;
                    RaisePropertyChanged(nameof(UserGroupAccessModel));
                }
            }
        }

        public UserAccount_VM()
        {
            Messenger.Default.Register<UserAccountModel>(this, UpdateUserAccountModel);
            Messenger.Default.Register<List<UserGroupAccessModel>>(this, UpdateUserGroupAccess);

            UserAccountSignInCommand = new RelayCommand(userAccountSignInCommandEvent);
            UserAccountSignOutCommand = new RelayCommand(userAccountSignOutCommandEvent);
            EditUserGroupAccessCommand = new RelayCommand(editUserGroupAccessCommandEvent);
            EditUserAccountListCommand = new RelayCommand(editUserAccountListCommandEvent);

            LoginBtn = Visibility.Visible;
            Messenger.Default.Send(UserAccount);
        }

        private void userAccountSignInCommandEvent()
        {
            userAccountSignInView.Show();
        }

        private void userAccountSignOutCommandEvent()
        {
            if (GlobalAttributesClass.HMINoActiveJob)
            {
                LogHelper.General(3, $"{UserAccount.UserName} Sign-out from the HMI");
                EF_Service.EF_InsertEventLog("TRACE", "GENERAL", "", "", "", "", UserAccount.UserName, "User Sign-Out From HMI");
                UserAccount = null;
                Messenger.Default.Send(UserAccount);
            }
            else
            {
                MessageBox.Show("Sign-Out Event Cancelled.\nPlease finish all pending job before sign-out", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void editUserGroupAccessCommandEvent()
        {
            userGroupAccessEditorView.Show();
        }

        private void editUserAccountListCommandEvent()
        {
            userAccountEditorView.Show();
        }

        private void UpdateUserAccountModel(UserAccountModel model)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    UserAccount = model;

                    if (model != null)
                    {
                        LoginBtn = Visibility.Collapsed;
                        UserBtn = Visibility.Visible;

                        FullName = $"{model.FirstName} {model.LastName}";

                        UserGroupAccessModel = UserGroupAccess.Find(x => x.UserGroup.Equals(model.UserGroup));
                    }
                    else
                    {
                        LoginBtn = Visibility.Visible;
                        UserBtn = Visibility.Collapsed;

                        FullName = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.General(5, ex.ToString());
                }
            });
        }

        private void UpdateUserGroupAccess(List<UserGroupAccessModel> groupAccess)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                UserGroupAccess = groupAccess;
                UpdateUserAccountModel(UserAccount);
            }));
        }
    }
}
