using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace HMI_CoolingEquipment.ViewModels
{
    public class UserAccountSignIn_VM : ViewModelBase
    {
        public ICommand UserAccountSignInCommand { get; set; }
        public ICommand UserAccountSignInViewClosingCommand { get; set; }

        private List<UserAccountModel> _userAccounts = new List<UserAccountModel>();
        public List<UserAccountModel> UserAccounts
        {
            get { return _userAccounts; }
            set
            {
                if (value != _userAccounts)
                {
                    _userAccounts = value;
                    RaisePropertyChanged(nameof(UserAccounts));
                }
            }
        }
        
        private string _userName = string.Empty;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value != _userName)
                {
                    _userName = value;
                    RaisePropertyChanged(nameof(UserName));
                }
            }
        }

        private Visibility _wrongPasswordUsername = Visibility.Collapsed;
        public Visibility WrongPasswordUsername
        {
            get { return _wrongPasswordUsername; }
            set
            {
                if (value != _wrongPasswordUsername)
                {
                    _wrongPasswordUsername = value;
                    RaisePropertyChanged(nameof(WrongPasswordUsername));
                }
            }
        }

        public UserAccountSignIn_VM() 
        {
            Messenger.Default.Register<List<UserAccountModel>>(this, UpdateUserAccountModel);
            UserAccountSignInCommand = new RelayCommand<UserAccountSignInModel>(UserAccountSignInCommandEvent);
            UserAccountSignInViewClosingCommand = new RelayCommand<EventArgsModel>(UserAccountSignInViewClosingCommandEvent);
        }

        private void UserAccountSignInCommandEvent(UserAccountSignInModel userAccountSignInModel)
        {
            if (UserSignIn(userAccountSignInModel.UserAccountUserNameTextBox.Text.Trim(),
                userAccountSignInModel.UserAccountPasswordBox.Password.Trim()))
            {
                userAccountSignInModel.UserAccountSignInWindow.Close();
            }
            else
            {
                LogHelper.General(3, $"Username = [{userAccountSignInModel.UserAccountUserNameTextBox.Text}] not exist");
                userAccountSignInModel.UserAccountSignInMessageTextBlock.Visibility = Visibility.Visible;
                userAccountSignInModel.UserAccountUserNameTextBox.Clear();
                userAccountSignInModel.UserAccountPasswordBox.Clear();
                userAccountSignInModel.UserAccountUserNameTextBox.Focus();
                userAccountSignInModel.UserAccountSignInButton.IsDefault = false;
            }
        }

        private void UserAccountSignInViewClosingCommandEvent(EventArgsModel eventArgsModel)
        {
            CancelEventArgs cancelEventArgs = eventArgsModel.EventArgs as CancelEventArgs;
            cancelEventArgs.Cancel = true;

            UserAccountSignInModel userAccountSignInModel = eventArgsModel.Parameter as UserAccountSignInModel;

            userAccountSignInModel.UserAccountSignInMessageTextBlock.Visibility = Visibility.Collapsed;
            userAccountSignInModel.UserAccountUserNameTextBox.Clear();
            userAccountSignInModel.UserAccountPasswordBox.Clear();
            userAccountSignInModel.UserAccountUserNameTextBox.Focus();
            userAccountSignInModel.UserAccountSignInWindow.Hide();

        }

        private void UpdateUserAccountModel(List<UserAccountModel> model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                UserAccounts = model;
            }));
        }

        public bool UserSignIn(string username, string password)
        {
            bool isLogin = false;

            foreach (UserAccountModel x in UserAccounts)
            {
                if (username.Equals(x.UserName))
                {
                    if (password.Equals(GlobalClassFunction.Decrypt_Password(x.Password)))
                    {
                        isLogin = true;
                        EF_Service.EF_UpdateDateTimeLoginHistory(username);
                        LogHelper.General(3, $"{x.UserName} Sign-in into the HMI");
                        EF_Service.EF_InsertEventLog("TRACE", "GENERAL", "", "", "", "", x.UserName, "User Sign-In Into HMI");
                        Messenger.Default.Send(x);
                        break;
                    }
                }
            }

            return isLogin;
        }

        public bool UserSignIn(string username)
        {
            bool isLogin = false;

            foreach (UserAccountModel x in UserAccounts)
            {
                if (username.Equals(x.UserName))
                {
                    isLogin = true;
                    EF_Service.EF_UpdateDateTimeLoginHistory(username);
                    LogHelper.General(3, $"{x.UserName} Sign-in into the HMI");
                    EF_Service.EF_InsertEventLog("TRACE", "GENERAL", "", "", "", "", x.UserName, "User Sign-In Into HMI");
                    Messenger.Default.Send(x);
                    break;
                }
            }

            return isLogin;
        }
    }
}
