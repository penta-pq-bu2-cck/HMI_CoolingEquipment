using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Interface;
using HMI_CoolingEquipment.Models;
using HMI_CoolingEquipment.Views;
using HMI_CoolingEquipment.Views.LotStatus;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace HMI_CoolingEquipment.ViewModels
{
    public class Frame_VM : ViewModelBase
    {
        public ICommand ResetButtonCommand { get; set; }

        private LoadPort_VM LoadPort_VM = null;
        private MachineLayout MachineLayout = null;
        private LoadingLoadPort LoadingLoadPort = null;
        private UnloadingLoadPort UnloadingLoadPort = null;
        private SecsGem SecsGem = null;
        private LotStatus LotStatus = null;
        private PTL PTL = null;
        private SettingsView SettingsView = null;
        private OperatorView OperatorView = null;
        private ReportView ReportView = null;
        private Thread CheckingDateTimeThread= null;
        private DateTime dtOpenHMI = DateTime.Now;
        private PTLService ptlService {get;set;}
        private float TimerLimit = 60;
        private UserAccountModel CurrentUser { get; set; } = null;

        #region IdleTimer
        DispatcherTimer tmrIdle = new DispatcherTimer();
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        uint idleTime = 0;
        LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();

        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
        #endregion

        private Frame_InterfaceModel frame_InterfaceModel = new Frame_InterfaceModel();
        public Frame_InterfaceModel Frame_InterfaceModel
        {
            get { return frame_InterfaceModel; }
            set
            {
                if (value != frame_InterfaceModel)
                {
                    frame_InterfaceModel = value;
                    RaisePropertyChanged(nameof(Frame_InterfaceModel));
                }
            }
        }

        private string currentTitle;
        public string CurrentTitle
        {
            get { return currentTitle; }
            set
            {
                if (currentTitle != value)
                {
                    currentTitle = value;
                    RaisePropertyChanged(nameof(CurrentTitle));
                }
            }
        }

        public Frame_VM()
        {
            try
            {
                LogManager.Configuration.Variables["basedirCustom"] = Path.GetPathRoot(Environment.CurrentDirectory);
                string a = Path.GetPathRoot(Environment.CurrentDirectory);
                LogHelper.General(3, "Initialize HMI");
                InitializeMessenger();
                InitializeMenuContent();
                InitializeRelayCommand();
                InitializeAsyn();
                InitializeCustomThread();
                InitializeTittleBar();
                InitializeIdleTimer();
            }
            catch (Exception ex)
            {
                LogHelper.General(6, ex.Message);
                throw ex;
            }
        }

        private void InitializeMessenger()
        {
            Messenger.Default.Register<UserAccountModel>(this, UpdateMenuContent);
            Messenger.Default.Register<List<UserGroupAccessModel>>(this, UpdateUserGroupAccess);
            Messenger.Default.Register<PTLService>(this, UpdatePTLService);
            Messenger.Default.Register<List<AlarmCurrentModel>>(this, UpdateAlarmCurrentMessage);
            Messenger.Default.Register<OperatorModel>(this, OperatorMenuSelection);
            Messenger.Default.Register<List<SettingsModel>>(this, UpdateSettingsModel);
            Messenger.Default.Register<UserAccountModel>(this, UpdateUserAccountModel);
        }

        private void InitializeMenuContent()
        {

            try
            {
                LoadPort_VM = new LoadPort_VM();
                MachineLayout = new MachineLayout(new MachineLayout_VM(LoadPort_VM));
                LoadingLoadPort = new LoadingLoadPort(new LoadingLoadPort_VM(LoadPort_VM));
                UnloadingLoadPort = new UnloadingLoadPort(new UnloadingLoadPort_VM(LoadPort_VM));
                SecsGem = new SecsGem(new SecsGem_VM());
                LotStatus = new LotStatus(new LotStatus_VM());
                PTL = new PTL(new PTL_VM());
                SettingsView = new SettingsView(new Settings_VM());
                OperatorView = new OperatorView(new Operator_VM());
                ReportView = new ReportView(new Report_VM());

            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }
        }

        private void InitializeRelayCommand()
        {
            Frame_InterfaceModel.MenuModel.MenuCommand = new RelayCommand<ToggleButton>(MenuComandEvent);
            Frame_InterfaceModel.PentaIconCommand = new RelayCommand(PentaIconCommandEvent);
            ResetButtonCommand = new RelayCommand(ResetButtonCommandEvent);
        }

        private void InitializeIdleTimer()
        {
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            tmrIdle.Tick += tmrIdle_Tick;
            tmrIdle.Interval = TimeSpan.FromSeconds(5);
            tmrIdle.Start();
        }

        public void InitializeTittleBar()
        {
            try
            {
                string w_file = AppDomain.CurrentDomain.FriendlyName;
                string w_directory = Directory.GetCurrentDirectory();
                DateTime sBuildDateTime = File.GetLastWriteTime(Path.Combine(w_directory, w_file));
                CurrentTitle = $"Infineon Cooling Equipment  v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version} <Build: {sBuildDateTime}> (Open: {dtOpenHMI}) - {w_directory}\\{w_file} " +
                             $" - idle: {idleTime}s";


            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }

        }

        private void InitializeCustomThread()
        {
            if (ConfigurationManager.AppSettings["DBChecking"].Equals("1"))
            {
                EF_Service.isCheckingDateTimeEnable = true;
                CheckingDateTimeThread = new Thread(EF_Service.EF_CompareDateTime);
                CheckingDateTimeThread.Name = nameof(CheckingDateTimeThread);
                CheckingDateTimeThread.Start(); 
            }
        }

        private async void InitializeAsyn()
        {
            await Task.Delay(1000);
            ModBus_Service.ConnectToModBus();
            EF_Service.EF_UpdateMigration();
        }

        private void PentaIconCommandEvent()
        {
            if (Frame_InterfaceModel.UserAccount != null)
            {
                if (!Frame_InterfaceModel.UserAccount.UserGroup.Equals("Operator"))
                {
                    Frame_InterfaceModel.MenuModel.MenuSelectedIndex = 0;
                }
                else
                {
                    Frame_InterfaceModel.ContentControl = OperatorView;
                    Frame_InterfaceModel.ContentHeader = "Menu";
                }
            }
            else
            {
                Frame_InterfaceModel.MenuModel.MenuSelectedIndex = 0;
            }
            EF_Service.EF_UpdateMemory(HMIDBMemory.ALL, nameof(PentaIconCommandEvent));
        }


        private async void ResetButtonCommandEvent()
        {
            try
            {
                Messenger.Default.Send(HMIReset.Reset);
                await Task.Run(() => AlarmService.ClearAlarm()).ConfigureAwait(false);
                await Task.Run(() => ptlService.ResetError()).ConfigureAwait(false);
                //EF_Service.EF_ClearAllError();
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
                MessageBox.Show($"Error:\n{ex.Message}", "Reset Failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                EF_Service.EF_UpdateMemory(HMIDBMemory.ALL, nameof(ResetButtonCommandEvent));
                Messenger.Default.Send(HMIReset.ResetJobHistory);
            }
        }


        private void MenuComandEvent(ToggleButton obj)
        {
            try
            {
                MenuItemModel menuModel = Frame_InterfaceModel.MenuModel.MenuSelectedItem as MenuItemModel;

                if (menuModel != null)
                {
                    Frame_InterfaceModel.ContentControl = menuModel.MenuContent;
                    Frame_InterfaceModel.ContentHeader = menuModel.MenuName;
                    LogHelper.General(3, $"Page Content Changed = [{menuModel.MenuName}]");
                    EF_Service.EF_InsertEventLog("TRACE", "GENERAL", "", "", "", "", Frame_InterfaceModel.UserAccount == null ? "" : Frame_InterfaceModel.UserAccount.UserName, $"Page Content Changed = [{menuModel.MenuName}]");
                    obj.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }
            finally
            {
                MenuItemModel menuModel = Frame_InterfaceModel.MenuModel.MenuSelectedItem as MenuItemModel;

                if (menuModel != null)
                {
                    if (menuModel.MenuName.Equals("Main") || menuModel.MenuName.Equals("PTL"))
                    {
                        EF_Service.EF_UpdateMemory(HMIDBMemory.LOADPORT, nameof(MenuComandEvent));
                    }
                }  
            }
        }
        private void UpdateUserAccountModel(UserAccountModel model)
        {
            CurrentUser = model;
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if (CurrentUser == null)
                {
                    e.Cancel = true;
                    MessageBox.Show("You Are Not Allowed To Close This Program", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (CurrentUser != null)
                {
                    if (CurrentUser.UserGroup.Equals("Operator"))
                    {
                        e.Cancel = true;
                        MessageBox.Show("You Are Not Allowed To Close This Program", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else if (MessageBox.Show("Are You Sure To Close This Program?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                LogHelper.General(3, "Closing HMI");
                SecsGemService.CloseSecsGem();
                ModBus_Service.LedBuzzerTrigger(ModBus_Service.RedAddress, false);
                ModBus_Service.LedBuzzerTrigger(ModBus_Service.YellowAddress, false);
                ModBus_Service.LedBuzzerTrigger(ModBus_Service.GreenAddress, false);
                ModBus_Service.LedBuzzerTrigger(ModBus_Service.BuzzerAddress, false);
                ModBus_Service.DisconnectFromModBus();

                if (ModBus_Service.HeartBeatTimer.Enabled)
                {
                    ModBus_Service.HeartBeatTimer.Stop();
                }

                if (ConfigurationManager.AppSettings["DBChecking"].Equals("1"))
                {
                    EF_Service.isCheckingDateTimeEnable = false;
                    CheckingDateTimeThread.Join();
                    CheckingDateTimeThread.Abort();
                }

                if (ptlService != null)
                {
                    foreach (PTLConnectionInfo x in ptlService.ConnectionInfos)
                    {
                        if (x.GatewayConnectionCode.Equals(7))
                        {
                            ptlService.ClosePTL();
                            break;
                        }
                    }

                }

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                LogHelper.General(6, ex.Message);
            }
        }

        private void tmrIdle_Tick(object sender, EventArgs e)
        {
            try
            {
                uint envTicks = (uint)Environment.TickCount;

                if (GetLastInputInfo(ref lastInputInfo))
                {
                    uint lastInputTick = lastInputInfo.dwTime;
                    if (GlobalAttributesClass.HMINoActiveJob)
                    {
                        idleTime = (envTicks - lastInputTick) / 1000;
                    }
                    else
                    {
                        idleTime = 0;
                    }
                }

                InitializeTittleBar();

                if (TimerLimit != 0)
                {
                    if (idleTime > TimerLimit && Frame_InterfaceModel.UserAccount != null && GlobalAttributesClass.HMINoActiveJob)
                    {
                        LogHelper.General(3, $"Trigger Auto Sign-out since user been idle for {TimerLimit}s. User name = [{Frame_InterfaceModel.UserAccount.UserName}] ");
                        EF_Service.EF_InsertEventLog("TRACE", "GENERAL", "", "", "", "", Frame_InterfaceModel.UserAccount.UserName, "Auto Sign-Out Triggered");
                        Frame_InterfaceModel.UserAccount = null;
                        Messenger.Default.Send(Frame_InterfaceModel.UserAccount);
                    } 
                }
               
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }

        }

        private void UpdateMenuContent(UserAccountModel model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    bool debugMode = ConfigurationManager.AppSettings["BypassLogin"].Equals("1") ? true : false;
                    bool LotStatusPageAccess = false;
                    bool LoadingPageAccess = false;
                    bool UnloadingPageAccess = false;
                    bool SecsGemPageAccess = false;
                    bool SettingPageAccess = false;
                    bool PTLPageAccess = false;

                    Frame_InterfaceModel.UserAccount = model;

                    if (model != null)
                    {
                        LotStatusPageAccess = Frame_InterfaceModel.UserGroupAccess.Find(x => x.UserGroup.Equals(model.UserGroup)).LotStatusPageAccess;
                        LoadingPageAccess = Frame_InterfaceModel.UserGroupAccess.Find(x => x.UserGroup.Equals(model.UserGroup)).LoadingPageAccess;
                        UnloadingPageAccess = Frame_InterfaceModel.UserGroupAccess.Find(x => x.UserGroup.Equals(model.UserGroup)).UnloadingPageAccess;
                        SecsGemPageAccess = Frame_InterfaceModel.UserGroupAccess.Find(x => x.UserGroup.Equals(model.UserGroup)).SecsGemPageAccess;
                        SettingPageAccess = Frame_InterfaceModel.UserGroupAccess.Find(x => x.UserGroup.Equals(model.UserGroup)).SettingPageAccess;
                        PTLPageAccess = Frame_InterfaceModel.UserGroupAccess.Find(x => x.UserGroup.Equals(model.UserGroup)).PTLPageAccess;

                    }

                    Frame_InterfaceModel.MenuModel.MenuModels.Clear();

                    AddMenuModel(MachineLayout, "pack://application:,,,/HMI_CoolingEquipment;component/Images/Icons/home.png", "Main");
                    AddMenuModel(LotStatus, "pack://application:,,,/HMI_CoolingEquipment;component/Images/Icons/StatsHistogram.png", "Lot Status");
                    //AddMenuModel(ReportView, "pack://application:,,,/HMI_CoolingEquipment;component/Images/Icons/Graph.png", "Report");
                    if (debugMode || LoadingPageAccess) { AddMenuModel(LoadingLoadPort, "pack://application:,,,/HMI_CoolingEquipment;component/Images/Icons/arrowup.png", "Loading"); }
                    if (debugMode || UnloadingPageAccess) { AddMenuModel(UnloadingLoadPort, "pack://application:,,,/HMI_CoolingEquipment;component/Images/Icons/arrowdown.png", "Unloading"); }
                    if (debugMode || SecsGemPageAccess) { AddMenuModel(SecsGem, "pack://application:,,,/HMI_CoolingEquipment;component/Images/Icons/world.png", "SecsGem"); }
                    if (debugMode || PTLPageAccess) { AddMenuModel(PTL, "pack://application:,,,/HMI_CoolingEquipment;component/Images/Icons/world.png", "PTL"); }
                    if (debugMode || SettingPageAccess) { AddMenuModel(SettingsView, "pack://application:,,,/HMI_CoolingEquipment;component/Images/Icons/gear.png", "Settings"); }

                    Frame_InterfaceModel.ContentControl = MachineLayout;
                    Frame_InterfaceModel.ContentHeader = "Main";
                    Frame_InterfaceModel.MenuModel.MenuSelectedIndex = 0;

                    if (model != null)
                    {
                        if (model.UserGroup.Equals("Operator"))
                        {
                            Frame_InterfaceModel.ContentControl = OperatorView;
                            Frame_InterfaceModel.ContentHeader = "Menu";
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.General(5, ex.ToString());
                }

            }));
        }

        private void UpdateUserGroupAccess(List<UserGroupAccessModel> groupAccess)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (!Frame_InterfaceModel.UserGroupAccess.Equals(groupAccess))
                {
                    Frame_InterfaceModel.UserGroupAccess = groupAccess;
                    //UpdateMenuContent(Frame_InterfaceModel.UserAccount);
                }
            }));
        }

        private void UpdatePTLService(PTLService service)
        {
            ptlService = service;
        }


        private void UpdateAlarmCurrentMessage(List<AlarmCurrentModel> model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Frame_InterfaceModel.CurrentAlarmMessage.Clear();
                Frame_InterfaceModel.CurrentAlarmMessage = model;
            }));
        }
        
        private void OperatorMenuSelection(OperatorModel model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                switch (model.Name)
                {
                    case "Loading":
                        Frame_InterfaceModel.ContentControl = LoadingLoadPort;
                        Frame_InterfaceModel.ContentHeader = "Loading";
                        break;
                    case "Unloading":
                        Frame_InterfaceModel.ContentControl = UnloadingLoadPort;
                        Frame_InterfaceModel.ContentHeader = "Unloading";
                        break;
                }

            }));
        }

        private void UpdateSettingsModel(List<SettingsModel> model)
        {
            if (model.Exists(x => x.Items.Contains("Auto Sign-out")))
            {
                float TimerLimit_tmp = float.Parse(model.Find(x => x.Items.Contains("Auto Sign-out")).Value)*60;
                if (!TimerLimit.Equals(TimerLimit_tmp))
                {
                    TimerLimit = float.Parse(model.Find(x => x.Items.Contains("Auto Sign-out")).Value)*60;
                    LogHelper.General(3, $"Auto Sign-out idle limit been updated. New idle time = [{TimerLimit}]");
                }
            }
        }
        
        private void AddMenuModel(ContentControl pageContent, string pageImage, string pageName)
        {
            try
            {
                Frame_InterfaceModel.MenuModel.MenuModels.Add(new MenuItemModel
                {
                    MenuContent = pageContent,
                    MenuImage = pageImage,
                    MenuName = pageName
                });
            }
            catch (Exception ex) 
            {
                LogHelper.General(5,ex.Message);
            }
        }
    }

}
