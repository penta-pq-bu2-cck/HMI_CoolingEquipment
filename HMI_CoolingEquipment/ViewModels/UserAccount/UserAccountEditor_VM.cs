using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using HMI_CoolingEquipment.Views.Dialog;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

namespace HMI_CoolingEquipment.ViewModels
{
    public class UserAccountEditor_VM : ViewModelBase
    {
        private bool taskChecking = true;
        private bool taskDiaShown = false;
        private TaskRunningDialog taskRunningDia;

        public ICommand AddUserAccountCommand { get; set; }
        public ICommand UserAccountEditorViewClosingCommand { get; set; }
        public ICommand ClearUserAccountEditorFieldCommand { get; set; }
        public ICommand DeleteUserAccountCommand { get; set; }
        public ICommand UpdateUserAccountCommand { get; set; }
        public ICommand SaveUserAccountListCommand { get; set; }

        public UserAccountModel UserAccountName { get; set; }

        private ObservableCollection<UserAccountModel> _userAccountModel = new ObservableCollection<UserAccountModel>();
        public ObservableCollection<UserAccountModel> UserAccountModel
        {
            get { return _userAccountModel; }
            set
            {
                if (value != _userAccountModel)
                {
                    _userAccountModel = value;
                    RaisePropertyChanged(nameof(UserAccountModel));
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

        CancellationTokenSource cts = new CancellationTokenSource();

        public UserAccountEditor_VM()
        {
            Messenger.Default.Register<List<UserAccountModel>>(this, UpdateUserAccount);
            Messenger.Default.Register<List<UserGroupAccessModel>>(this, UpdateUserGroupAccess);

            AddUserAccountCommand = new RelayCommand<UserAccountEditorModel>(async (o) => await addUserAccountCommandEvent(o));
            UserAccountEditorViewClosingCommand = new RelayCommand<EventArgsModel>(userAccountEditorViewClosingCommandEvent);
            ClearUserAccountEditorFieldCommand = new RelayCommand<UserAccountEditorModel>(clearUserAccountEditorFieldCommandEvent);
            DeleteUserAccountCommand = new RelayCommand<UserAccountEditorModel>(async (o) => await deleteUserAccountCommandEvent(o));
            UpdateUserAccountCommand = new RelayCommand<UserAccountEditorModel>(async (o) => await updateUserAccountCommandEvent(o));
            SaveUserAccountListCommand = new RelayCommand(saveUserAccountListCommandEvent);
        }

        private void UpdateUserAccount(List<UserAccountModel> userAccounts)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                UserAccountModel.Clear();
                userAccounts.ForEach(x =>
                {
                    UserAccountModel.Add(x);
                });
            }));
        }

        private void UpdateUserGroupAccess(List<UserGroupAccessModel> groupAccess)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                UserGroupAccess = groupAccess;
            }));
        }
        private async Task addUserAccountCommandEvent(UserAccountEditorModel userAccountEditorModel)
        {
            try
            {
                LogHelper.General(3, $"Start {nameof(addUserAccountCommandEvent)}");
                bool result = false;
                taskChecking = true;
                string userName = userAccountEditorModel.UserAccountUserNameTextBox?.Text;
                string password = userAccountEditorModel.UserAccountPasswordTextBox?.Text;
                string firstName = userAccountEditorModel.UserAccountFirstNameTextBox?.Text;
                string lastName = userAccountEditorModel.UserAccountLastNameTextBox?.Text;
                string userGroup = userAccountEditorModel.UserAccountGroupKeyComboBox?.Text;

                if (PromptMsgBox($"Are you sure you want to add {userName}", "Confirmation",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxResult.Yes))
                {
                    LogHelper.General(3, $"{userName}");
                    LogHelper.General(3, $"{nameof(addUserAccountCommandEvent)}| Start add user task");
                    List<UserAccountModel> userAccModelList = UserAccountModel.ToList();
                    cts = new CancellationTokenSource();
                    LogHelper.General(3, $"Testing 5 - userAccModelList: {userAccModelList.Count}");
                    var userExistRes = await CheckUserExist(userAccModelList, userName);
                    try
                    {
                        if (!userExistRes.Item1)
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                UserAccountModel.Add(new UserAccountModel
                                {
                                    UserName = userName,
                                    Password = GlobalClassFunction.Encrypt_Password(password),
                                    FirstName = firstName,
                                    LastName = lastName,
                                    UserGroup = userGroup
                                });
                                LogHelper.General(3, $"{nameof(addUserAccountCommandEvent)}| user = {userName} added");
                            });
                            result = true;
                        }
                        LogHelper.General(3, $"{nameof(addUserAccountCommandEvent)}| user {userName} exist: {userExistRes.Item1}");
                    }
                    catch (OperationCanceledException)
                    {
                        LogHelper.General(3, "User cancelled the Add User task.");
                        return; // exit gracefully
                    }
                    catch (Exception ex)
                    {
                        LogHelper.General(5, $"{nameof(addUserAccountCommandEvent)}| Error adding user {userName}: {ex}");
                        result = false;
                    }
                    finally
                    {
                        TaskComplete();
                }
                    #region OldCode
                    //var userExistRes = await CheckUserExist(userAccModelList, userName);
                    //if (userExistRes.Item1)
                    //{
                    //    result = false;
                    //}
                    //else
                    //{
                    //    //add new user
                    //    UserAccountModel.Add(new UserAccountModel
                    //    {
                    //        UserName = userName,
                    //        Password = GlobalClassFunction.Encrypt_Password(password),
                    //        FirstName = firstName,
                    //        LastName = lastName,
                    //        UserGroup = userGroup
                    //    });                            
                    //    result = true;
                    //}
                    //LogHelper.General(3, $"{nameof(addUserAccountCommandEvent)}| user {UserAccountName.UserName} exist: {userExistRes.Item1}");
                    //TaskComplete();
                    #endregion
                }
                else
                {
                    LogHelper.General(3, $"{nameof(addUserAccountCommandEvent)}| User cancel, skip add user task");
                    return;
                }
                if (!result)
                {
                    PromptMsgBox($"{userAccountEditorModel.UserAccountUserNameTextBox.Text} already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxResult.OK);
                    LogHelper.General(3, $"{userName} already exists");
                    //MessageBox.Show($"{userAccountEditorModel.UserAccountUserNameTextBox.Text} Exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    PromptMsgBox($"{userAccountEditorModel.UserAccountUserNameTextBox.Text} added successfully.", "Valid", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxResult.OK);
                    //MessageBox.Show($"{userAccountEditorModel.UserAccountUserNameTextBox.Text} added successfully.", "Valid", MessageBoxButton.OK, MessageBoxImage.Information);

                    userAccountEditorModel.UserAccountListView.SelectedItem = UserAccountModel.First(x => x.UserName.Equals(userName));
                    userAccountEditorModel.UserAccountListView.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                PromptMsgBox($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxResult.OK);
                //MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LogHelper.General(3, $"End of {nameof(addUserAccountCommandEvent)}");
            }
        }
        private void userAccountEditorViewClosingCommandEvent(EventArgsModel eventArgsModel)
        {
            CancelEventArgs cancelEventArgs = eventArgsModel.EventArgs as CancelEventArgs;
            cancelEventArgs.Cancel = true;

            Window window = eventArgsModel.Parameter as Window;
            window.Hide();
            EF_Service.EF_UpdateMemory(HMIDBMemory.USERACCOUNT, nameof(userAccountEditorViewClosingCommandEvent));
        }

        private void clearUserAccountEditorFieldCommandEvent(UserAccountEditorModel userAccountEditorModel)
        {
            userAccountEditorModel.UserAccountListView.SelectedIndex = -1;
        }
        private async Task deleteUserAccountCommandEvent(UserAccountEditorModel userAccountEditorModel)
        {
            try
            {
                LogHelper.General(3, $"Start {nameof(deleteUserAccountCommandEvent)}");
                bool result = false;
                string errorMsg = "";
                taskChecking = true;
                string userName = userAccountEditorModel.UserAccountUserNameTextBox.Text;

                if (MessageBox.Show($"Are you sure you want to delete {userName}", "Confirmation",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    List<UserAccountModel> userAccModelList = UserAccountModel.ToList();
                    cts = new CancellationTokenSource();
                    var userExistRes = await CheckUserExist(userAccModelList, userName);
                    try
                    {
                        if (userExistRes.Item1 && userExistRes.Item2!= null )
                        {
                            if (userExistRes.Item2.UserName.Equals(userName))
                            {
                                //delete user
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    UserAccountModel.Remove(userExistRes.Item2);
                                });
                                LogHelper.General(3, $"Username : {userName}  deleted");
                                result = true;
                            }
                            else
                            {
                                errorMsg = $"{userExistRes.Item2.UserName} & {userName} Not Tally";
                                LogHelper.General(5, $"{errorMsg} ");
                                result = false;
                            }
                        }
                        else
                        {
                            errorMsg = $"{userName} Not Exists";
                            LogHelper.General(5, $"{errorMsg} ");
                            result = false;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        LogHelper.General(3, $"{nameof(deleteUserAccountCommandEvent)}| Task was cancelled by user.");
                    }
                    catch ( Exception ex )
                    {
                        LogHelper.General(5, $"Error deleting user {userName}: {ex}");
                        result = false;
                    }
                    finally
                    {
                        TaskComplete();
                    }
                    #region OldCode
                    //var userExistRes = await CheckUserExist(userAccModelList, userName);
                    //if (userExistRes.Item1)
                    //{
                    //    if (userExistRes.Item2.UserName.Equals(userName))
                    //    {
                    //        //delete user
                    //        UserAccountModel.Remove(userExistRes.Item2);
                    //    }
                    //    else
                    //    {
                    //        result = false;
                    //        errorMsg = $"{userExistRes.Item2.UserName} & {userName} Not Tally";
                    //    }
                    //    result = true;
                    //}
                    //else
                    //{
                    //    result = false;
                    //    errorMsg = $"{userName} Not Exists";
                    //}
                    //TaskComplete();
#endregion
                }
                if (!result)
                {
                    MessageBox.Show(errorMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    userAccountEditorModel.UserAccountListView.SelectedIndex = 0;
                    userAccountEditorModel.UserAccountListView.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show($"{userAccountEditorModel.UserAccountUserNameTextBox.Text} deleted", "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    userAccountEditorModel.UserAccountListView.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LogHelper.General(3, $"End of {nameof(deleteUserAccountCommandEvent)}");
            }
        }
        private async Task updateUserAccountCommandEvent(UserAccountEditorModel userAccountEditorModel)
        {
            try
            {
                LogHelper.General(3, $"Start {nameof(updateUserAccountCommandEvent)}");
                bool result = false;
                taskChecking = true;
                string userName = userAccountEditorModel.UserAccountUserNameTextBox.Text;
                string password = userAccountEditorModel.UserAccountPasswordTextBox.Text;
                string firstName = userAccountEditorModel.UserAccountFirstNameTextBox.Text;
                string lastName = userAccountEditorModel.UserAccountLastNameTextBox.Text;
                string userGroup = userAccountEditorModel.UserAccountGroupKeyComboBox.Text;

                if (MessageBox.Show($"Are you sure you want to update {userName}", "Confirmation",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    List<UserAccountModel> userAccModelList = UserAccountModel.ToList();
                    cts = new CancellationTokenSource();
                    var userExistRes = await CheckUserExist(userAccModelList, userName);
                    try
                    {
                        if (userExistRes.Item1 && userExistRes.Item2 != null )
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                //update data
                                UpdateUserAccountData(userExistRes.Item2, userName, firstName, lastName, userGroup, password);
                                LogHelper.General(3, $"{userName} modified");
                            });
                            result= true;
                        }
                        LogHelper.General(3, $"{nameof(updateUserAccountCommandEvent)}| user {userName} exist: {userExistRes.Item1}");
                    }
                    catch (OperationCanceledException)
                    {
                        LogHelper.General(3, $"{nameof(updateUserAccountCommandEvent)}| Task was cancelled by user.");
                    }
                    catch (Exception ex)
                    {
                        LogHelper.General(5, $"{nameof(updateUserAccountCommandEvent)}| Error updating user {userName} :{ex}");
                        result = false;
                    }
                    finally
                    {
                        TaskComplete();
                    }

                    if (!result)
                    {
                        MessageBox.Show($"{userName} not exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        LogHelper.General(3, $"{userName} not exists");
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            userAccountEditorModel.UserAccountListView.SelectedIndex = 0;
                            userAccountEditorModel.UserAccountListView.SelectedIndex = -1;
                        });
                    }
                    else
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            userAccountEditorModel.UserAccountListView.SelectedIndex = -1;
                            LogHelper.General(3, $"{userName} updated successfully");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LogHelper.General(3, $"End of {nameof(updateUserAccountCommandEvent)}");
            }
        }
        private void UpdateUserAccountData(UserAccountModel oriData, string userName, string firstName, string lastName, string userGroup, string password = "")
        {
            oriData.UserName = userName;
            oriData.FirstName = firstName;
            oriData.LastName = lastName;
            oriData.UserGroup = userGroup;
            if (!string.IsNullOrWhiteSpace(password))
            {
                if (!string.IsNullOrWhiteSpace(password))
                {
                    oriData.Password = GlobalClassFunction.Encrypt_Password(password);
                }
            }
        }

        private void saveUserAccountListCommandEvent()
        {
            EF_Service.EF_UpdateUserAccount(GlobalClassFunction.UserAccountMap(UserAccountModel.ToList()));
            LogHelper.General(3, "User Account Updated and Saved");
        }
        private async Task<Tuple<bool, UserAccountModel>> CheckUserExist(List<UserAccountModel> userAccountList, string userNameToCheck)
        {
            if (userAccountList == null)
            {
                return Tuple.Create(false, (UserAccountModel)null);
            }
            foreach (var user in userAccountList)
            {
                if (cts!= null && cts.Token.IsCancellationRequested)
                {
                    //Throw, so caller knows it was cancelled
                    LogHelper.General(3, "CheckUserExist cancelled by user");
                    cts.Token.ThrowIfCancellationRequested();
                }
                LogHelper.General(3, $"Testing 1 - Loop UserName: {user.UserName}");
                if (user.UserName == userNameToCheck)
                {
                    LogHelper.General(3, $"username match");
                    return Tuple.Create(true, user);
                }
                if (cts != null)
                    await Task.Delay(100, cts.Token);
                else
                    await Task.Delay(100);
            }
            LogHelper.General(3, $"Username not found: {userNameToCheck}");
            return Tuple.Create(false, (UserAccountModel)null);
        }
        private void TaskRunning(string taskName)
        {
            if (!taskChecking) return;
            taskRunningDia = new TaskRunningDialog(taskName);
            taskDiaShown = true;
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Show the dialog non-blocking
                taskRunningDia.Show();
                // Monitor for cancellation via dialog button
                taskRunningDia.Closed += (s, e) =>
                {
                    if (taskRunningDia.DialogResult == false)
                    {
                        taskChecking = false;

                        // Cancel the running task
                        cts?.Cancel();
                        LogHelper.General(3, $"{taskName} task cancelled by user");
                    }
                };
            });
        }

        private void TaskComplete()
        {
            taskChecking = false;

            if (taskDiaShown)
            {
                try
                {
                    taskRunningDia?.CloseDialog();
                }
                catch
                {
                   LogHelper.General(5, "Error closing task running dialog");
                }
                taskRunningDia = null;
                taskDiaShown = false;
            }
        }
        private bool PromptMsgBox(string content, string caption, MessageBoxButton msgButton, MessageBoxImage msgImage, MessageBoxResult defaultResult, MessageBoxResult expectedResult)
        {
            bool result = false;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (MessageBox.Show(content, caption, msgButton, msgImage, defaultResult) == expectedResult)
                {
                    result = true;
                }
            }));

            return result;
        }
    }
}
