using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HMI_CoolingEquipment.ViewModels
{
    public class UserGroupAccessEditor_VM : ViewModelBase
    {
        public ICommand UserGroupAccessEditorViewClosingCommand { get; set; }
        public ICommand SaveUserGroupAccessCommand { get; set; }
        public ICommand DeleteUserAccountGroupCommand { get; set; }
        public ICommand AddUserAccountGroupCommand { get; set; }

        private ObservableCollection<UserGroupAccessModel> _userGroupAccess = new ObservableCollection<UserGroupAccessModel>();
        public ObservableCollection<UserGroupAccessModel> UserGroupAccess
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
        
        private int _comboBoxSelectedIndex = -1;
        public int ComboBoxSelectedIndex
        {
            get { return _comboBoxSelectedIndex; }
            set
            {
                if (value != _comboBoxSelectedIndex)
                {
                    _comboBoxSelectedIndex = value;
                    RaisePropertyChanged(nameof(ComboBoxSelectedIndex));
                }
            }
        }

        public UserGroupAccessEditor_VM() 
        {
            Messenger.Default.Register<List<UserGroupAccessModel>>(this, UpdateUserGroupAccess);
            UserGroupAccessEditorViewClosingCommand = new RelayCommand<EventArgsModel>(userGroupAccessEditorViewClosingCommand);
            SaveUserGroupAccessCommand = new RelayCommand(saveUserGroupAccessCommandEvent);
            DeleteUserAccountGroupCommand = new RelayCommand<UserGroupAccessModel>(deleteUserAccountGroupCommandEvent);
            AddUserAccountGroupCommand = new RelayCommand<ComboBox>(addUserAccountGroupCommandEvent);
        }

        private void UpdateUserGroupAccess(List<UserGroupAccessModel> groupAccess)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                UserGroupAccess.Clear();
                groupAccess.ForEach(x =>
                {
                    UserGroupAccess.Add(x);
                });
            }));
        }

        private void userGroupAccessEditorViewClosingCommand(EventArgsModel eventArgsModel)
        {
            CancelEventArgs cancelEventArgs = eventArgsModel.EventArgs as CancelEventArgs;
            cancelEventArgs.Cancel = true;

            Window window = eventArgsModel.Parameter as Window;
            window.Hide();
            EF_Service.EF_UpdateMemory(HMIDBMemory.USERGROUPACCESS, nameof(userGroupAccessEditorViewClosingCommand));
        }

        private void saveUserGroupAccessCommandEvent()
        {
            EF_Service.EF_UpdateUserGroupAccess(GlobalClassFunction.UserGroupAccessMap(UserGroupAccess.ToList()));
            LogHelper.General(3, "User Group Access have been modified and saved");
        }

        private void deleteUserAccountGroupCommandEvent(UserGroupAccessModel user)
        {
            LogHelper.General(3, $"Remove user group = [{user.UserGroup}]");
            UserGroupAccess.Remove(user);
            ComboBoxSelectedIndex = -1;
        }

        private void addUserAccountGroupCommandEvent(ComboBox cb)
        {
            LogHelper.General(3, $"New User Group Add = [{cb.Text}]");

            if(UserGroupAccess.SelectMany(x => x.UserGroup).Equals(cb.Text))
            {
                MessageBox.Show("User Group have been registered","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            else
            {
                UserGroupAccessModel userGroupAccessModel = new UserGroupAccessModel()
                {
                    UserGroup = cb.Text,
                    LotStatusPageAccess = true,
                    LoadingPageAccess = true,
                    UnloadingPageAccess = true,
                    SecsGemPageAccess = true,
                    SettingPageAccess = true,
                    PTLPageAccess = true,
                    EditUserGroupAccess = true,
                    EditUserAccountList = true,
                };

                UserGroupAccess.Add(userGroupAccessModel);
                ComboBoxSelectedIndex = -1;
                cb.Text = string.Empty;
            }
            
        }
    }
}
