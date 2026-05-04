using GalaSoft.MvvmLight.Messaging;
using GEMPro300.Model;
using HMI_CoolingEquipment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace HMI_CoolingEquipment.Communication
{
    public class EF_Service
    {
        public static bool isCheckingDateTimeEnable = false;
        private static bool isInitDone = false;

        public static void EF_UpdateMigration()
        {
            try
            {
                using (var context = new DB_Context())
                {
                    var existingMigrations = context.Database.GetAppliedMigrations();
                    var allMigrations = context.Database.GetMigrations();
                    var newMigrations = allMigrations.Except(existingMigrations);

                    if (newMigrations.Any())
                    {
                        context.Database.Migrate();
                    }
                }

                EF_InitializeDBContent();

                isInitDone = true;
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.ALL, nameof(EF_UpdateMigration));
            }
        }

        private static void EF_InitializeDBContent()
        {
            try
            {
                string dt_Now = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);

                using (var context = new DB_Context())
                {
                    #region UserAccount
                    List<USERACCOUNT> useraccounts = context.USERACCOUNT.ToList();

                    if (useraccounts.Count <= 0)
                    {
                        List<USERACCOUNT> init_UserAccount = new List<USERACCOUNT>()
                        {
                            new USERACCOUNT()
                            {
                                UserName = "penta",
                                Password = GlobalClassFunction.Encrypt_Password("#penta123"),
                                FirstName = "Pentamaster",
                                LastName = "Corporation Berhad",
                                UserGroup = "Penta",
                                CreatedOn = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider)
                            }
                        };

                        context.USERACCOUNT.AddRange(init_UserAccount);
                        context.SaveChanges();
                    }
                    #endregion

                    #region UserGroupAccess
                    List<USERGROUPACCESS> usergroupaccess = context.USERGROUPACCESS.ToList();

                    if (usergroupaccess.Count <= 0)
                    {
                        List<USERGROUPACCESS> init_Usergroupaccess = new List<USERGROUPACCESS>()
                        {
                            new USERGROUPACCESS()
                            {
                                UserGroup = "Penta",
                                LotStatusPageAccess = true,
                                LoadingPageAccess = true,
                                UnloadingPageAccess = true,
                                SecsGemPageAccess = true,
                                SettingPageAccess = true,
                                PTLPageAccess = true,
                                EditUserGroupAccess = true,
                                EditUserAccountList = true,
                            }
                        };

                        context.USERGROUPACCESS.AddRange(init_Usergroupaccess);
                        context.SaveChanges();
                    }
                    #endregion

                    #region LoadPortConfiguration
                    List<LOADPORTCONFIGURATION> loadportconfiguration = context.LOADPORTCONFIGURATION.ToList();

                    if (loadportconfiguration.Count <= 0)
                    {
                        List<LOADPORTCONFIGURATION> init_ = new List<LOADPORTCONFIGURATION>
                        {
                            new LOADPORTCONFIGURATION()
                            {
                                LoadPortStatusKey = "Disable",
                                LoadPortStatusState = LoadPortState.Disabled,
                                LoadPortLEDColour = LEDColour.AMBER,
                                LoadPortLEDState = LEDState.NoBlinking,
                            },
                            new LOADPORTCONFIGURATION()
                            {
                                LoadPortStatusKey = "Empty",
                                LoadPortStatusState = LoadPortState.Empty,
                                LoadPortLEDColour = LEDColour.NoColour,
                                LoadPortLEDState = LEDState.NoColour,
                            },
                            new LOADPORTCONFIGURATION()
                            {
                                LoadPortStatusKey = "Loading Process",
                                LoadPortStatusState = LoadPortState.LoadProcess,
                                LoadPortLEDColour = LEDColour.GREEN,
                                LoadPortLEDState = LEDState.Blinking,
                            },
                            new LOADPORTCONFIGURATION()
                            {
                                LoadPortStatusKey = "Unloading Process",
                                LoadPortStatusState = LoadPortState.UnloadProcess,
                                LoadPortLEDColour = LEDColour.GREEN,
                                LoadPortLEDState = LEDState.Blinking,
                            },
                            new LOADPORTCONFIGURATION()
                            {
                                LoadPortStatusKey = "Loaded/Occupied",
                                LoadPortStatusState = LoadPortState.Loaded,
                                LoadPortLEDColour = LEDColour.GREEN,
                                LoadPortLEDState = LEDState.NoBlinking,
                            },
                            new LOADPORTCONFIGURATION()
                            {
                                LoadPortStatusKey = "Cooling Completed",
                                LoadPortStatusState = LoadPortState.CoolingCompleted,
                                LoadPortLEDColour = LEDColour.BLUE,
                                LoadPortLEDState = LEDState.NoBlinking,
                            },
                            new LOADPORTCONFIGURATION()
                            {
                                LoadPortStatusKey = "Error",
                                LoadPortStatusState = LoadPortState.Error,
                                LoadPortLEDColour = LEDColour.RED,
                                LoadPortLEDState = LEDState.Blinking,
                            },
                            new LOADPORTCONFIGURATION()
                            {
                                LoadPortStatusKey = "Warning",
                                LoadPortStatusState = LoadPortState.Warning,
                                LoadPortLEDColour = LEDColour.RED,
                                LoadPortLEDState = LEDState.Blinking,
                            },
                            new LOADPORTCONFIGURATION()
                            {
                                LoadPortStatusKey = "Abort",
                                LoadPortStatusState = LoadPortState.Abort,
                                LoadPortLEDColour = LEDColour.CYAN,
                                LoadPortLEDState = LEDState.Blinking,
                            },
                        };

                        context.LOADPORTCONFIGURATION.AddRange(init_);
                        context.SaveChanges();
                    } 
                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
        }

        #region InsertDB

        public static bool EF_SetupJobChecking(string JobID, string LotID)
        {
            bool isExist = false;
            try
            {
                using (var context = new DB_Context())
                {
                    isExist = context.SETUPJOB.Any(x => x.JobID.Equals(JobID) || x.LotNo.Equals(LotID));
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return isExist;
        }

        public static void EF_InsertSetupJob(SETUPJOB setupjob)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    context.SETUPJOB.Add(setupjob);
                    context.SaveChanges();
                }

                EF_SetDateTimeSetupJob(setupjob.JobID, setupjob.LotNo, JobStatus.Initialize);
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
                throw ex;
            }
        }
        
        public static void EF_InsertEventLog(string EventType, string EventPage, string JobID, string LotID, string CarrierID, string LoadPortID, string UserID, string EventMessage)
        {
            try
            {
                string dt_Now = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);

                using (var context = new DB_Context())
                {
                    context.EVENTLOG.Add(new EVENTLOG
                    {
                        DateTime = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider),
                        EventType = EventType,
                        EventPage = EventPage,
                        JobID = JobID,
                        LotID = LotID,
                        CarrierID = CarrierID,
                        LoadPortID = LoadPortID,
                        UserID = UserID,
                        EventMessage = EventMessage
                    });

                    context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
        }

        public static void EF_InsertSetupJobHistory(string jobID)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    SETUPJOB job = context.SETUPJOB.ToList().Find(x => x.JobID.Equals(jobID));
                    List<JOBDETAILS> jobDetails = context.JOBDETAILS.ToList().Where(x => x.JobID.Equals(jobID)).ToList();

                    SETUPJOBHISTORY history = GlobalClassFunction.SetupJobHistoryMap(job);
                    history.JobDetails = GlobalClassFunction.JobDetailsHistoryMap(jobDetails);

                    context.SETUPJOBHISTORY.Add(history);
                    context.SaveChanges();

                    context.SETUPJOB.Remove(job);
                    context.SaveChanges();

                    context.JOBDETAILS.RemoveRange(jobDetails);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
                throw ex;
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.SETUPJOB, nameof(EF_InsertSetupJobHistory));
                EF_UpdateMemory(HMIDBMemory.SETUPJOBHISTORY, nameof(EF_InsertSetupJobHistory));
            }
        }

        public static void EF_InsertJobDetailsHistory(string jobID)
        {
            using(var context = new DB_Context())
            {
                List<JOBDETAILS> jobDetails = context.JOBDETAILS.ToList().Where(x => x.JobID.Equals(jobID)).ToList();
                context.JOBDETAILSHISTORY.AddRange(GlobalClassFunction.JobDetailsHistoryMap(jobDetails));
                context.SaveChanges();

                context.JOBDETAILS.RemoveRange(jobDetails);
                context.SaveChanges();
            }
        }

        public static void EF_InsertJobDetails(List<JOBDETAILS> jobdetails)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    context.JOBDETAILS.AddRange(jobdetails);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.JOBDETAILS, nameof(EF_InsertJobDetails));
            }
        }

        public static async void EF_InsertAlarmToAlarmCurrentTableAsync(int alarmCode)
        {
            ALARMDEFINITION alarmDef = new ALARMDEFINITION();
            try
            {
                string dt_Now = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);

                using (var context = new DB_Context())
                {
                    alarmDef = context.ALARMDEFINITION.ToList().Find(x => x.AlarmCode.Equals(alarmCode));
                    
                    if (alarmDef != null)
                    {
                        if (context.ALARMCURRENT.Count() == 0)
                        {
                            await SecsGemService.SendEventSecsGemAsync(105);
                        }

                        try
                        {
                            if (!context.ALARMCURRENT.ToList().Exists(x => x.AlarmCode.Equals(alarmCode)))
                            {
                                ALARMCURRENT alarmCurrent = new ALARMCURRENT
                                {
                                    AlarmDateTime = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider),
                                    AlarmCode = alarmDef.AlarmCode,
                                    AlarmType = alarmDef.AlarmType,
                                    AlarmModule = alarmDef.AlarmModule,
                                    AlarmMessage = alarmDef.AlarmMessage,
                                    AlarmAction = alarmDef.AlarmAction,
                                };

                                context.ALARMCURRENT.Add(alarmCurrent);
                                context.SaveChanges();

                                LogHelper.Alarm(3, $"{alarmCurrent.AlarmDateTime},{alarmDef.AlarmType},{alarmDef.AlarmCode},{alarmDef.AlarmModule},{alarmDef.AlarmMessage},{alarmDef.AlarmAction}");

                                await SecsGemService.SendAlarmToServer(alarmCode, true);

                                EF_UpdateMemory(HMIDBMemory.ALARMCURRENT, nameof(EF_InsertAlarmToAlarmCurrentTableAsync));
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.EF(5, ex.Message.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
        }

        public static bool EF_AbortJob(string jobID, string lotID)
        {
            using(var context = new DB_Context())
            {
                try
                {
                    if (context.SETUPJOB.Any(x => x.JobID.Equals(jobID) && x.LotNo.Equals(lotID)) && context.JOBDETAILS.Any(x => x.JobID.Equals(jobID) && x.LotNo.Equals(lotID)))
                    {
                        context.JOBDETAILS.Where(x => x.JobID.Equals(jobID) && x.LotNo.Equals(lotID)).ToList().ForEach(x =>
                        {
                            LOADPORT port = new LOADPORT();
                            if (context.LOADPORT.Any(y => y.PreAssignLoad.Equals(x.CarrierID)))
                            {
                                port = context.LOADPORT.ToList().Find(y => y.PreAssignLoad != null && y.PreAssignLoad.Equals(x.CarrierID));
                                port.LoadPortState = LoadPortState.Empty;
                                port.PreAssignLoad = null;
                                context.LOADPORT.Update(port);
                            }
                            else if (context.LOADPORT.Any(y => y.LoadPortCarrierLoad.Equals(x.CarrierID)))
                            {
                                port = context.LOADPORT.ToList().Find(y => y.LoadPortCarrierLoad != null && y.LoadPortCarrierLoad.Equals(x.CarrierID));
                                port.LoadPortState = LoadPortState.Abort;
                                context.LOADPORT.Update(port);
                            }
                            else if (context.LOADPORT.Any(y => y.LoadPortID.Equals(x.PortLocation)))
                            {
                                port = context.LOADPORT.ToList().Find(y => y.LoadPortID.Equals(x.PortLocation));
                                port.LoadPortState = LoadPortState.Abort;
                                context.LOADPORT.Update(port);
                            }

                            LogHelper.General(3, $"Abort Job for Job ID = [{jobID}], Lot ID = [{lotID}], [{x.CarrierID}]");

                            context.JOBDETAILS.Remove(x);
                        });

                        context.SETUPJOB.RemoveRange(context.SETUPJOB.Where(x => x.JobID.Equals(jobID) && x.LotNo.Equals(lotID)));

                        context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.General(5, ex.ToString());
                }
            }

            EF_UpdateMemory(HMIDBMemory.LOADPORT, nameof(EF_AbortJob));
            EF_UpdateMemory(HMIDBMemory.JOBDETAILS, nameof(EF_AbortJob));
            EF_UpdateMemory(HMIDBMemory.SETUPJOB, nameof(EF_AbortJob));
            return true;
        }

        public static bool EF_CheckJobPurged(string jobID)
        {
            bool isJobPurged = false;
            using (var context = new DB_Context())
            {
                try
                {
                    if (context.SETUPJOB.ToList().Exists(x => x.JobID.Equals(jobID)))
                    {
                        if (context.SETUPJOB.ToList().Find(x => x.JobID.Equals(jobID)).JobStatus == JobStatus.Purge)
                        {
                            isJobPurged = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.General(5, ex.ToString());
                }
            }
            return isJobPurged;
        }

        public static bool EF_PurgeJob(string jobID, string lotID)
        {
            GlobalAttributesClass.HMINoActiveJob = true;
            using (var context = new DB_Context())
            {
                if (context.SETUPJOB.Any(x => x.JobID.Equals(jobID) && x.LotNo.Equals(lotID)) && context.JOBDETAILS.Any(x => x.JobID.Equals(jobID) && x.LotNo.Equals(lotID)))
                {
                    context.SETUPJOB.ToList().Find(x => x.JobID.Equals(jobID) && x.LotNo.Equals(lotID)).JobStatus = JobStatus.Purge;
                    context.SETUPJOB.ToList().Find(x => x.JobID.Equals(jobID) && x.LotNo.Equals(lotID)).IsJobError = false;

                    context.SaveChanges();
                }
                else
                {
                    return false;
                }

                context.JOBDETAILS.Where(x => x.JobID.Equals(jobID) && x.LotNo.Equals(lotID)).ToList().ForEach(x =>
                {
                    LOADPORT port = new LOADPORT();
                    if (context.LOADPORT.Any(y => y.PreAssignLoad.Equals(x.CarrierID)))
                    {
                        port = context.LOADPORT.ToList().Find(y => y.PreAssignLoad != null && y.PreAssignLoad.Equals(x.CarrierID));
                        port.LoadPortState = LoadPortState.Empty;
                        context.LOADPORT.Update(port);
                    }
                    else if (context.LOADPORT.Any(y => y.LoadPortCarrierLoad.Equals(x.CarrierID)))
                    {
                        port = context.LOADPORT.ToList().Find(y => y.LoadPortCarrierLoad != null && y.LoadPortCarrierLoad.Equals(x.CarrierID));
                        if (port.LoadPortError)
                        {
                            int errorCode = (port.LoadPortRack.Equals("AA") || port.LoadPortRack.Equals("AB") || port.LoadPortRack.Equals("AC")) ? 300 + port.LoadPortNodeAddress.Value : 600 + port.LoadPortNodeAddress.Value;
                            AlarmService.ClearAlarm(errorCode);
                            port.LoadPortError = false;
                            port.LoadPortState = LoadPortState.Loaded;
                            port.LoadPortCarrierLoad = null;
                            context.LOADPORT.Update(port);
                        }
                    }
                });

                context.SaveChanges();
            }

            EF_UpdateMemory(HMIDBMemory.SETUPJOB, nameof(EF_PurgeJob));
            EF_UpdateMemory(HMIDBMemory.LOADPORT, nameof(EF_PurgeJob));
            return true;
        }

        #endregion

        #region UpdateDB

        public static void EF_UpdateMemory(HMIDBMemory HMIMemory, string caller)
        {
            try
            {
                LogHelper.General(3, $"{nameof(EF_UpdateMemory)}| Caller: {caller}");
                List<SETUPJOB> job = new List<SETUPJOB>();
                List<JOBDETAILS> jobdetails = new List<JOBDETAILS>();
                List<USERACCOUNT> useraccounts = new List<USERACCOUNT>();
                List<USERGROUPACCESS> usergroupaccess = new List<USERGROUPACCESS>();
                List<LOADPORT> loadport = new List<LOADPORT>();
                List<ALARMCURRENT> alarmCurrent = new List<ALARMCURRENT>();
                List<ALARMHISTORY> alarmHistory = new List<ALARMHISTORY>();
                List<SETTINGS> settings = new List<SETTINGS>();
                List<SETUPJOBHISTORY> setupjobhistory = new List<SETUPJOBHISTORY>();
                List<LOADPORTCONFIGURATION> loadportconfig = new List<LOADPORTCONFIGURATION>();

                using (var context = new DB_Context())
                {
                    switch (HMIMemory)
                    {
                        case HMIDBMemory.SETUPJOB:
                            job = context.SETUPJOB.ToList();
                            if (job.Count > 0)
                            {
                                List<SetupJobModel> jobModel = GlobalClassFunction.SetupJobMap(job);
                                Messenger.Default.Send(jobModel);
                            }
                            else
                            {
                                Messenger.Default.Send(new List<SetupJobModel>());
                            }
                            break;
                        case HMIDBMemory.JOBDETAILS:
                            jobdetails = context.JOBDETAILS.ToList();
                            if (jobdetails.Count > 0)
                            {
                                List<JobDetailsModel> jobdetailsModel = GlobalClassFunction.JobDetailsMap(jobdetails);
                                Messenger.Default.Send(jobdetailsModel);
                            }
                            else
                            {
                                Messenger.Default.Send(new List<JobDetailsModel>());
                            }
                            break;
                        case HMIDBMemory.USERACCOUNT:
                            useraccounts = context.USERACCOUNT.ToList();
                            if (useraccounts.Count > 0)
                            {
                                List<UserAccountModel> useraccountsModel = GlobalClassFunction.UserAccountMap(useraccounts);
                                Messenger.Default.Send(useraccountsModel);
                            }
                            break;
                        case HMIDBMemory.USERGROUPACCESS:
                            usergroupaccess = context.USERGROUPACCESS.ToList();
                            if (usergroupaccess.Count > 0)
                            {
                                List<UserGroupAccessModel> usergroupaccessModel = GlobalClassFunction.UserGroupAccessMap(usergroupaccess);
                                Messenger.Default.Send(usergroupaccessModel);
                            }
                            break;
                        case HMIDBMemory.LOADPORT:
                            loadport = context.LOADPORT.ToList();
                            if (loadport.Count > 0)
                            {
                                List<LoadPortModel> LoadPortModel = GlobalClassFunction.LoadPortMap(loadport);
                                Messenger.Default.Send(LoadPortModel);
                            }
                            break;
                        case HMIDBMemory.LOADPORTCONFIGURATION:
                            loadportconfig = context.LOADPORTCONFIGURATION.ToList();
                            if (loadportconfig.Count > 0)
                            {
                                List<LoadPortConfigurationModel> tmp_Model = GlobalClassFunction.LoadPortConfigurationMap(loadportconfig);
                                Messenger.Default.Send(tmp_Model);
                            }
                            break;
                        case HMIDBMemory.ALARMCURRENT:
                            alarmCurrent = context.ALARMCURRENT.ToList();
                            if (alarmCurrent.Count > 0)
                            {
                                List<AlarmCurrentModel> AlarmCurrentModel = GlobalClassFunction.AlarmDefinitionMap(alarmCurrent);
                                Messenger.Default.Send(AlarmCurrentModel);
                            }
                            else
                            {
                                List<AlarmCurrentModel> AlarmCurrentModel = new List<AlarmCurrentModel>();
                                Messenger.Default.Send(AlarmCurrentModel);
                            }
                            break;
                        case HMIDBMemory.ALARMHISTORY:
                            alarmHistory = context.ALARMHISTORY.ToList().Where(x => x.AlarmDateTime.Equals(DateTime.Now.ToShortDateString())).ToList();
                            if (alarmHistory.Count > 0)
                            {
                                List<AlarmHistoryModel> AlarmHistoryModel = GlobalClassFunction.AlarmDefinitionMap(alarmHistory);
                                Messenger.Default.Send(AlarmHistoryModel);
                            }
                            else
                            {
                                List<AlarmHistoryModel> AlarmHistoryModel = new List<AlarmHistoryModel>();
                                Messenger.Default.Send(AlarmHistoryModel);
                            }
                            break;
                        case HMIDBMemory.SETTINGS:
                            settings = context.SETTINGS.ToList();
                            if (settings.Count > 0)
                            {
                                List<SettingsModel> SettingsModel = GlobalClassFunction.SettingsMap(settings);
                                Messenger.Default.Send(SettingsModel);
                            }
                            break;
                        case HMIDBMemory.SETUPJOBHISTORY:
                            setupjobhistory = context.SETUPJOBHISTORY.Include(x => x.JobDetails).ToList();
                            if (setupjobhistory.Count > 0)
                            {
                                List<SetupJobHistoryModel> SetupJobHistoryModel = GlobalClassFunction.SetupJobHistoryMap(setupjobhistory);
                                Messenger.Default.Send(SetupJobHistoryModel);
                            }
                            break;
                        case HMIDBMemory.ALL:
                            job = context.SETUPJOB.ToList();
                            if (job.Count > 0)
                            {
                                List<SetupJobModel> jobModel = GlobalClassFunction.SetupJobMap(job);
                                Messenger.Default.Send(jobModel);
                            }
                            else
                            {
                                Messenger.Default.Send(new List<SetupJobModel>());
                            }
                            jobdetails = context.JOBDETAILS.ToList();
                            if (jobdetails.Count > 0)
                            {
                                List<JobDetailsModel> jobdetailsModel = GlobalClassFunction.JobDetailsMap(jobdetails);
                                Messenger.Default.Send(jobdetailsModel);
                            }
                            else
                            {
                                Messenger.Default.Send(new List<JobDetailsModel>());
                            }
                            useraccounts = context.USERACCOUNT.ToList();
                            if (useraccounts.Count > 0)
                            {
                                List<UserAccountModel> useraccountsModel = GlobalClassFunction.UserAccountMap(useraccounts);
                                Messenger.Default.Send(useraccountsModel);
                            }
                            usergroupaccess = context.USERGROUPACCESS.ToList();
                            if (usergroupaccess.Count > 0)
                            {
                                List<UserGroupAccessModel> usergroupaccessModel = GlobalClassFunction.UserGroupAccessMap(usergroupaccess);
                                Messenger.Default.Send(usergroupaccessModel);
                            }
                            loadport = context.LOADPORT.ToList();
                            if (loadport.Count > 0)
                            {
                                List<LoadPortModel> LoadPortModel = GlobalClassFunction.LoadPortMap(loadport);
                                Messenger.Default.Send(LoadPortModel);
                            }
                            alarmCurrent = context.ALARMCURRENT.ToList();
                            if (alarmCurrent.Count > 0)
                            {
                                List<AlarmCurrentModel> AlarmCurrentModel = GlobalClassFunction.AlarmDefinitionMap(alarmCurrent);
                                Messenger.Default.Send(AlarmCurrentModel);
                            }
                            else
                            {
                                List<AlarmCurrentModel> AlarmCurrentModel = new List<AlarmCurrentModel>();
                                Messenger.Default.Send(AlarmCurrentModel);
                            }
                            alarmHistory = context.ALARMHISTORY.ToList().Where(x => x.AlarmDateTime.Equals(DateTime.Now.ToShortDateString())).ToList();
                            if (alarmHistory.Count > 0)
                            {
                                List<AlarmHistoryModel> AlarmHistoryModel = GlobalClassFunction.AlarmDefinitionMap(alarmHistory);
                                Messenger.Default.Send(AlarmHistoryModel);
                            }
                            else
                            {
                                List<AlarmHistoryModel> AlarmHistoryModel = new List<AlarmHistoryModel>();
                                Messenger.Default.Send(AlarmHistoryModel);
                            }
                            settings = context.SETTINGS.ToList();
                            if (settings.Count > 0)
                            {
                                List<SettingsModel> SettingsModel = GlobalClassFunction.SettingsMap(settings);
                                Messenger.Default.Send(SettingsModel);
                            }
                            setupjobhistory = context.SETUPJOBHISTORY.Include(x => x.JobDetails).ToList();
                            if (setupjobhistory.Count > 0)
                            {
                                List<SetupJobHistoryModel> SetupJobHistoryModel = GlobalClassFunction.SetupJobHistoryMap(setupjobhistory);
                                Messenger.Default.Send(SetupJobHistoryModel);
                            }
                            loadportconfig = context.LOADPORTCONFIGURATION.ToList();
                            if (loadportconfig.Count > 0)
                            {
                                List<LoadPortConfigurationModel> tmp_Model = GlobalClassFunction.LoadPortConfigurationMap(loadportconfig);
                                Messenger.Default.Send(tmp_Model);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(6, ex.Message.ToString());
            }
        }

        public static void EF_UpdateLoadPort(LOADPORT loadport)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    context.LOADPORT.Update(loadport);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
                throw ex;
            }
        }

        public static void EF_UpdateLoadPortConfig(List<LOADPORTCONFIGURATION> loadportconfig)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    context.LOADPORTCONFIGURATION.UpdateRange(loadportconfig);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
                throw ex;
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.LOADPORTCONFIGURATION, nameof(EF_UpdateLoadPortConfig));
            }
        }

        public static void EF_UpdateUserGroupAccess(List<USERGROUPACCESS> usergroupaccess)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    List<USERGROUPACCESS> usergroupaccesslist = context.USERGROUPACCESS.ToList();

                    usergroupaccess.ForEach(x =>
                    {
                        if (usergroupaccesslist.Exists(y => y.UserGroup.Equals(x.UserGroup)))
                        {
                            USERGROUPACCESS model = usergroupaccesslist.Find(y => y.UserGroup.Equals(x.UserGroup));

                            model.LotStatusPageAccess = x.LotStatusPageAccess;
                            model.LoadingPageAccess = x.LoadingPageAccess;
                            model.UnloadingPageAccess = x.UnloadingPageAccess;
                            model.SecsGemPageAccess = x.SecsGemPageAccess;
                            model.SettingPageAccess = x.SettingPageAccess;
                            model.PTLPageAccess = x.PTLPageAccess;
                            model.EditUserGroupAccess = x.EditUserGroupAccess;
                            model.EditUserAccountList = x.EditUserAccountList;

                            context.USERGROUPACCESS.Update(model);
                        }
                        else
                        {
                            context.USERGROUPACCESS.Add(x);
                        }
                    });

                    usergroupaccesslist.ForEach(x =>
                    {
                        if (!usergroupaccess.Exists(y => y.UserGroup.Equals(x.UserGroup)))
                        {
                            context.USERGROUPACCESS.Remove(x);
                        }
                    });

                    context.SaveChanges();
                }
                MessageBox.Show("Saved", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.USERGROUPACCESS, nameof(EF_UpdateUserGroupAccess));
            }
        }

        public static void EF_UpdateUserAccount(List<USERACCOUNT> useraccounts)
        {
            string dt_Now = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);

            try
            {
                using (var context = new DB_Context())
                {
                    List<USERACCOUNT> useraccountlist = context.USERACCOUNT.ToList();

                    useraccounts.ForEach(x =>
                    {
                        if (useraccountlist.Exists(y => y.UserName.Equals(x.UserName)))
                        {
                            USERACCOUNT useraccount = useraccountlist.Find(y => y.UserName.Equals(x.UserName));

                            useraccount.UserName = x.UserName;
                            useraccount.FirstName = x.FirstName;
                            useraccount.LastName = x.LastName;
                            useraccount.UserGroup = x.UserGroup;
                            useraccount.Password = x.Password;

                            context.USERACCOUNT.Update(useraccount);
                        }
                        else
                        {
                            x.CreatedOn = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                            context.USERACCOUNT.Add(x);
                        }
                    });

                    useraccountlist.ForEach(x =>
                    {
                        if (!useraccounts.Exists(y => y.UserName.Equals(x.UserName)))
                        {
                            context.USERACCOUNT.Remove(x);
                        }
                    });

                    context.SaveChanges();
                }
                MessageBox.Show("Saved", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.USERACCOUNT, nameof(EF_UpdateUserAccount));
            }
        }

        public static void EF_UpdateLoadPort(List<LOADPORT> loadports)
        {

            try
            {
                using (var context = new DB_Context())
                {
                    List<LOADPORT> LoadPortList = context.LOADPORT.ToList();

                    loadports.ForEach(x =>
                    {
                        if (LoadPortList.Exists(y => y.LoadPortID.Equals(x.LoadPortID)))
                        {
                            LOADPORT LoadPort = LoadPortList.Find(y => y.LoadPortID.Equals(x.LoadPortID));

                            LoadPort.LoadPortID = x.LoadPortID;
                            LoadPort.LoadPortRack = x.LoadPortRack;
                            LoadPort.LoadPortColumn = x.LoadPortColumn;
                            LoadPort.LoadPortRow = x.LoadPortRow;
                            LoadPort.LoadPortPriority = x.LoadPortPriority;
                            LoadPort.LoadPortType = x.LoadPortType;
                            LoadPort.LoadPortCarrierLoad = x.LoadPortCarrierLoad;
                            LoadPort.LoadPortIPAddress = x.LoadPortIPAddress;
                            LoadPort.LoadPortNodeAddress = x.LoadPortNodeAddress;
                            LoadPort.LoadPortNodeConnectionStatus = x.LoadPortNodeConnectionStatus;
                            LoadPort.LoadPortError = x.LoadPortError;
                            LoadPort.LoadPortState = x.LoadPortState;
                            LoadPort.PreAssignLoad = x.PreAssignLoad;
                            LoadPort.LoadPortIsEnable = x.LoadPortIsEnable;

                            context.LOADPORT.Update(LoadPort);
                        }
                    });

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.LOADPORT, $"{nameof(EF_UpdateLoadPort)}-List");
            }
        }

        public static void EF_UpdateLoadPort(LoadPortModel loadport)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    List<LOADPORT> LoadPortList = context.LOADPORT.ToList();

                    if (LoadPortList.Exists(y => y.LoadPortID.Equals(loadport.LoadPortID)))
                    {
                        LOADPORT LoadPort = LoadPortList.Find(y => y.LoadPortID.Equals(loadport.LoadPortID));

                        LoadPort.LoadPortID = loadport.LoadPortID;
                        LoadPort.LoadPortRack = loadport.LoadPortRack;
                        LoadPort.LoadPortColumn = loadport.LoadPortColumn;
                        LoadPort.LoadPortRow = loadport.LoadPortRow;
                        LoadPort.LoadPortPriority = loadport.LoadPortPriority;
                        LoadPort.LoadPortType = loadport.LoadPortType;
                        LoadPort.LoadPortCarrierLoad = loadport.LoadPortCarrierLoad;
                        LoadPort.LoadPortIPAddress = loadport.LoadPortIPAddress;
                        LoadPort.LoadPortNodeAddress = loadport.LoadPortNodeAddress;
                        LoadPort.LoadPortNodeConnectionStatus = loadport.LoadPortNodeConnectionStatus;
                        LoadPort.LoadPortError = loadport.LoadPortError;
                        LoadPort.LoadPortState = loadport.LoadPortState;
                        LoadPort.PreAssignLoad = loadport.PreAssignLoad;
                        LoadPort.LoadPortIsEnable = loadport.LoadPortIsEnable;

                        context.LOADPORT.Update(LoadPort);
                    }

                    context.SaveChanges();
                }
                LogHelper.EF(1, $"Loadport {loadport.LoadPortID} updated. LoadPortState: {loadport.LoadPortState.ToString()}. LoadPortIsEnable: {loadport.LoadPortIsEnable.ToString()}");
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
        }

        public static void EF_UpdateDateTimeLoginHistory(string username)
        {
            try
            {
                string dt_Now = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);

                using (var context = new DB_Context())
                {
                    USERACCOUNT useraccount = context.USERACCOUNT.ToList().Find(x => x.UserName.Equals(username));

                    if (useraccount != null)
                    {
                        useraccount.LastLoginTime = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                        context.USERACCOUNT.Update(useraccount);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
        }

        public static void EF_UpdateAlarmDefinition(List<ALARMDEFINITION> model)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    List<ALARMDEFINITION> originalAlarmDefinition = context.ALARMDEFINITION.ToList();

                    if (originalAlarmDefinition.Count() > 0)
                    {
                        context.ALARMDEFINITION.RemoveRange(originalAlarmDefinition);
                        context.ALARMDEFINITION.AddRange(model);
                        context.SaveChanges();
                    }
                    else
                    {
                        context.ALARMDEFINITION.AddRange(model);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
                throw ex;
            }
        }

        public static void EF_UpdateSettings(List<SETTINGS> model)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    List<SETTINGS> originalSettings = context.SETTINGS.ToList();

                    if (originalSettings.Count() > 0)
                    {
                        context.SETTINGS.RemoveRange(originalSettings);
                        context.SETTINGS.AddRange(model);
                        context.SaveChanges();
                    }
                    else
                    {
                        context.SETTINGS.AddRange(model);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
                throw ex;
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.SETTINGS, nameof(EF_UpdateSettings));
            }
        }
        
        public static void EF_UpdateSettingsValue(List<SETTINGS> model)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    context.SETTINGS.UpdateRange(model);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
                throw ex;
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.SETTINGS, nameof(EF_UpdateSettingsValue));
            }
        }
        
        public static void EF_UpdateLoadPortConfiguration(List<LOADPORT> model)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    List<LOADPORT> originalSettings = context.LOADPORT.ToList();

                    if (!originalSettings.Any(x => !x.LoadPortState.Equals(LoadPortState.Empty)))
                    {
                        if (originalSettings.Count() > 0)
                        {
                            context.LOADPORT.RemoveRange(originalSettings);
                            context.LOADPORT.AddRange(model);
                            context.SaveChanges();
                        }
                        else
                        {
                            context.LOADPORT.AddRange(model);
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        throw new Exception("LoadPort not empty. Please clear all LoadPort before update LoadPort Configuration");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
                throw ex;
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.LOADPORT, nameof(EF_UpdateLoadPortConfiguration));
            }
        }

        public static async void EF_UpdateCarrierStatusAsync(string carrierID, string portLocation, JobStatus status, string userName)
        {
            try
            {
                string dt_Now = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);
                JOBDETAILS carrier;

                using (var context = new DB_Context())
                {
                    try
                    {
                        if (!status.Equals(JobStatus.Loading))
                        {
                            //carrier = context.JOBDETAILS.ToList().Find(x => x.CarrierID.Equals(carrierID) || x.PortLocation.Equals(portLocation));
                            carrier = context.JOBDETAILS.ToList().Find(x => x.CarrierID.Equals(carrierID));
                        }
                        else
                        {
                            carrier = context.JOBDETAILS.ToList().Find(x => x.CarrierID.Equals(carrierID));
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.EF(5, ex.Message);
                        carrier = context.JOBDETAILS.ToList().Find(x => x.CarrierID.Equals(carrierID));
                    }

                    if (carrier != null)
                    {
                        switch (status)
                        {
                            case JobStatus.Loading: //Third FAT
                                carrier.CarrierStatus = status;
                                context.SaveChanges();
                                break;
                            case JobStatus.Cooling:
                                carrier.CarrierStatus = status;
                                carrier.PortLocation = portLocation;
                                carrier.LoadingTime = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                                carrier.LoadBy = userName;
                                context.JOBDETAILS.Update(carrier);
                                context.SaveChanges();
                                GlobalAttributesClass.HMINoActiveJob = true;

                                EF_InsertEventLog("TRACE", "LOADING", carrier.JobID, carrier.LotNo, carrierID, portLocation, userName, "LoadPort Loaded With Carrier");

                                //Sent LoadPortToBlocked
                                //await SecsGemService.SendEventSecsGemAsync(1050, $"{portLocation},{carrierID}", 102);
                                SecsGemService.SendLoadPortToBlocked(carrier.LotNo, carrierID, portLocation).Wait(200);

                                EF_InsertEventLog("TRACE", "SECSGEM", carrier.JobID, carrier.LotNo, carrierID, portLocation, userName, "Sent Event 102-LoadPortToBlocked");

                                break;
                            case JobStatus.Complete:
                                carrier.CarrierStatus = status;
                                carrier.UnloadingTime = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                                carrier.UnloadBy = userName;
                                context.JOBDETAILS.Update(carrier);
                                context.SaveChanges();

                                EF_InsertEventLog("TRACE", "UNLOADING", carrier.JobID, carrier.LotNo, carrierID, portLocation, userName, "Carrier Unloaded From LoadPort");

                                //Sent LoadPortEmpty
                                //await SecsGemService.SendEventSecsGemAsync(103, $"{portLocation}", 101);
                                SecsGemService.SendLoadPortReadyToLoad(portLocation).Wait(200);

                                EF_InsertEventLog("TRACE", "SECSGEM", carrier.JobID, carrier.LotNo, carrierID, portLocation, userName, "Sent Event 101-LoadPortEmpty");

                                break;
                        }

                        EF_CarrierCheckFirstLastAsync(carrier.CarrierID, status, userName);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.JOBDETAILS, nameof(EF_UpdateCarrierStatusAsync));
            }
        }

        public static void EF_RevertCarrierStatus(string JobID, string LotID, JobStatus status)
        {
            using (var context = new DB_Context())
            {
                try
                {
                    foreach (var item in context.JOBDETAILS.Where(x => x.LotNo.Equals(LotID) && x.JobID.Equals(JobID)))
                    {
                        item.CarrierStatus = status;
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.EF(5, ex.Message.ToString());
                }
            }
        }

        #endregion

        public static List<SetupJobHistoryModel> EF_GetJobHistory(DateTime DateFrom, DateTime DateTo)
        {
            List<SetupJobHistoryModel> jobHistoryModels = new List<SetupJobHistoryModel>();
            try
            {
                using (var context = new DB_Context())
                {
                    List<SETUPJOBHISTORY> jobHistory = context.SETUPJOBHISTORY.Include(x => x.JobDetails).ToList().Where(x => x.CompletedOn >= DateFrom && x.CompletedOn <= DateTo).ToList();
                    jobHistoryModels = GlobalClassFunction.SetupJobHistoryMap(jobHistory);
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return jobHistoryModels;
        }

        public static List<AlarmHistoryModel> EF_GetAlarmHistory(DateTime DateFrom, DateTime DateTo)
        {
            List<AlarmHistoryModel> alarmHistoryModels = new List<AlarmHistoryModel>();
            try
            {
                using (var context = new DB_Context())
                {
                   List<ALARMHISTORY> alarmHistory = context.ALARMHISTORY.ToList().Where(x => x.AlarmDateTime >= DateFrom && x.AlarmDateTime <= DateTo).ToList();
                   alarmHistoryModels = GlobalClassFunction.AlarmDefinitionMap(alarmHistory);
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return alarmHistoryModels;
        }

        public static string EF_GetLotIDFromCarrierID(string carrierID)
        {
            string LotID = string.Empty;

            try
            {
                using (var context = new DB_Context())
                {
                    JOBDETAILS jobdetails = null;
                    jobdetails = context.JOBDETAILS.ToList().Find(x => x.CarrierID.Equals(carrierID));

                    if (jobdetails != null)
                    {
                        LotID = jobdetails.LotNo;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return LotID;
        }

        public static List<SETUPJOB> EF_GetErrorJobFromCarrierID(JobStatus status)
        {
            List<SETUPJOB> tmp_job = new List<SETUPJOB>();
            try
            {
                using (var context = new DB_Context())
                {
                    tmp_job = context.SETUPJOB.ToList().Where(x => x.JobStatus.Equals(status)).Where(x => x.IsJobError).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return tmp_job;
        }

        public static string EF_GetJobIDFromCarrierID(string carrierID)
        {
            string JobID = string.Empty;

            try
            {
                using (var context = new DB_Context())
                {
                    JOBDETAILS jobdetails = null;
                    jobdetails = context.JOBDETAILS.ToList().Find(x => x.CarrierID.Equals(carrierID));

                    if (jobdetails != null)
                    {
                        JobID = jobdetails.JobID;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return JobID;
        }
        
        public static int EF_GetNumberOfCurrentErrorWarning()
        {
            int errorWarning = 0;
            try
            {
                using (var context = new DB_Context())
                {
                    errorWarning = context.ALARMCURRENT.Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return errorWarning;
        }

        public static List<JOBDETAILS> EF_GetCarrierListFromJobID(string jobID, ref bool isFirstCarrier )
        {
            isFirstCarrier = true;
            List<JOBDETAILS> carrierList = null;

            try
            {
                using (var context = new DB_Context())
                {
                    carrierList = context.JOBDETAILS.ToList().Where(x => x.JobID.Equals(jobID)).ToList();

                    if (carrierList.Count > 0)
                    {
                        foreach(JOBDETAILS jobs in carrierList)
                        {
                            if (!jobs.CarrierStatus.Equals(JobStatus.Initialize))
                            {
                                isFirstCarrier = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return carrierList;
        }

        public static string EF_GetJobIDFromLotID(string lotID)
        {
            string JobID = string.Empty;

            try
            {
                using (var context = new DB_Context())
                {
                    SETUPJOB job = null;
                    job = context.SETUPJOB.ToList().Find(x => x.LotNo.Equals(lotID));

                    if (job != null)
                    {
                        JobID = job.JobID;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return JobID;
        }
        
        public static bool EF_UnloadingStatusCheck(string lotID, string jobID)
        {
            bool isReadyToUnload = false;

            try
            {
                using (var context = new DB_Context())
                {
                    SETUPJOB job = null;
                    job = context.SETUPJOB.ToList().Find(x => x.LotNo.Equals(lotID) && x.JobID.Equals(jobID));
                    if (job.JobStatus.Equals(JobStatus.Staging) || job.JobStatus.Equals(JobStatus.StagingTimeExpired))
                    {
                        isReadyToUnload = true;
                    }
                    LogHelper.General(3, $"{nameof(EF_UnloadingStatusCheck)}| [EF_UnloadingStatusCheck] - LotID: {lotID} / jobID: {jobID} / JobStatus: {job.JobStatus} / ReadyToUnload: {isReadyToUnload}");
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return isReadyToUnload;
        }
        
        public async static void MoveAlarmFromCurrentToHistory(int alarmCode = 0)
        {
            try
            {
                string dt_Now = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);

                using (var context = new DB_Context())
                {
                    List<ALARMCURRENT> alarmCurrent = context.ALARMCURRENT.ToList();

                    if (alarmCurrent.Count > 0)
                    {
                        if (alarmCode.Equals(0))
                        {
                            alarmCurrent.ForEach(async x =>
                            {
                                bool isJobError = false;
                                if (x.AlarmCode.Equals(2))
                                {
                                    isJobError = context.SETUPJOB.ToList().Where(y => y.JobStatus.Equals(JobStatus.LoadingTimeExpired)).Any(y => y.IsJobError);
                                }
                                else if (x.AlarmCode.Equals(2) || x.AlarmCode.Equals(52))
                                {
                                    isJobError = context.SETUPJOB.ToList().Where(y => y.JobStatus.Equals(JobStatus.StagingTimeExpired)).Any(y => y.IsJobError);
                                }

                                if (!isJobError)
                                {
                                    ALARMHISTORY alarmHistory = new ALARMHISTORY
                                    {
                                        AlarmClearedDateTime = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider),
                                        AlarmCode = x.AlarmCode,
                                        AlarmType = x.AlarmType,
                                        AlarmModule = x.AlarmModule,
                                        AlarmMessage = x.AlarmMessage,
                                        AlarmAction = x.AlarmAction,
                                        AlarmDateTime = x.AlarmDateTime,
                                    };

                                    context.ALARMCURRENT.Remove(x);
                                    context.ALARMHISTORY.Add(alarmHistory);

                                    await SecsGemService.SendAlarmToServer(x.AlarmCode, false); 
                                }
                            });
                        }
                        else
                        {
                            if(alarmCurrent.Any(x => x.AlarmCode.Equals(alarmCode)))
                            {
                                ALARMCURRENT tmp_ALARMCURRENT = alarmCurrent.Find(x => x.AlarmCode.Equals(alarmCode));

                                ALARMHISTORY alarmHistory = new ALARMHISTORY
                                {
                                    AlarmClearedDateTime = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider),
                                    AlarmCode = tmp_ALARMCURRENT.AlarmCode,
                                    AlarmType = tmp_ALARMCURRENT.AlarmType,
                                    AlarmModule = tmp_ALARMCURRENT.AlarmModule,
                                    AlarmMessage = tmp_ALARMCURRENT.AlarmMessage,
                                    AlarmAction = tmp_ALARMCURRENT.AlarmAction,
                                    AlarmDateTime = tmp_ALARMCURRENT.AlarmDateTime,
                                };

                                context.ALARMCURRENT.Remove(tmp_ALARMCURRENT);
                                context.ALARMHISTORY.Add(alarmHistory);

                                await SecsGemService.SendAlarmToServer(tmp_ALARMCURRENT.AlarmCode, false);
                            }
                        }
                        context.SaveChanges();

                        if(context.ALARMCURRENT.Count() == 0)
                        {
                            await SecsGemService.SendEventSecsGemAsync(106);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.ALARMHISTORY, nameof(MoveAlarmFromCurrentToHistory));
                EF_UpdateMemory(HMIDBMemory.ALARMCURRENT, nameof(MoveAlarmFromCurrentToHistory));
            }
        }
        
        public static async void EF_CarrierCheckFirstLastAsync(string carrierID, JobStatus status, string userName)
        {
            try
            {
                string dt_Now = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);

                using (var context = new DB_Context())
                {
                    string jobID = context.JOBDETAILS.ToList().Find(x => x.CarrierID.Equals(carrierID)).JobID;
                    string lotID = context.JOBDETAILS.ToList().Find(x => x.CarrierID.Equals(carrierID)).LotNo;
                    List<JOBDETAILS> carrier = context.JOBDETAILS.ToList().Where(x => x.JobID.Equals(jobID) && !x.CarrierStatus.Equals(status)).ToList();

                    if (carrier.Count() <= 0)
                    {
                        switch (status)
                        {
                            case JobStatus.Cooling:

                                EF_SetDateTimeSetupJob(jobID, lotID, status);
                                EF_InsertEventLog("TRACE", "LOADING", jobID, lotID, "", "", userName, "Loading Time Stopped");
                                EF_InsertEventLog("TRACE", "COOLING", jobID, lotID, "", "", userName, "Cooling Time Started");

                                //Sent Job Executing
                                //await SecsGemService.SendEventSecsGemAsync(1060, $"{jobID},Executing,{lotID}", 104);
                                SecsGemService.SendJobTracker(jobID, lotID, $"Executing", carrierID).Wait(200);

                                EF_InsertEventLog("TRACE", "SECSGEM", jobID, lotID, "", "", userName, "Sent Event 104-JobExecuting");

                                Messenger.Default.Send(new LogValue
                                {
                                    Value = $"Cooling Executed - Lot ID = [{lotID}]",
                                    Page = HMIPage.LoadingPage
                                });

                                break;

                            case JobStatus.Complete:

                                //Set JobStatus = Complete for this JOB
                                EF_SetDateTimeSetupJob(jobID, lotID, status);
                                EF_InsertSetupJobHistory(jobID);
                                GlobalAttributesClass.HMINoActiveJob = true;

                                EF_InsertEventLog("TRACE", "STAGING", jobID, lotID, "", "", userName, "Staging Time Stopped");
                                EF_InsertEventLog("TRACE", "COMPLETED", jobID, lotID, "", "", userName, "Job/Lot Completed");

                                Messenger.Default.Send(new LogValue
                                {
                                    Value = $"Lot Completed - Lot ID = [{lotID}]",
                                    Page = HMIPage.UnloadingPage
                                });
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message);
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.JOBDETAILS, nameof(EF_CarrierCheckFirstLastAsync));
            }
        }
        
        public static async void EF_SentLoadPortReadyToUnloadAsync(string jobID)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    List<JOBDETAILS> carrier = context.JOBDETAILS.ToList().Where(x => x.JobID.Equals(jobID)).ToList();

                    if (carrier.Count() > 0)
                    {
                        for(int i = 0; i < carrier.Count(); i++)
                        {
                            //Sent LoadPortReadyToUnload
                            //await SecsGemService.SendEventSecsGemAsync(1070, $"{carrier[i].PortLocation},{carrier[i].CarrierID}", 103);
                            SecsGemService.SendLoadPortReadyToUnload(carrier[i].LotNo, carrier[i].CarrierID, carrier[i].PortLocation).Wait(200);

                            EF_InsertEventLog("TRACE", "SECSGEM", carrier[i].JobID, carrier[i].LotNo, carrier[i].CarrierID, carrier[i].PortLocation, "", "Sent Event 103-LoadPortReadyToUnload");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message);
            }
        }

        public static void EF_ClearAllError()
        {
            try
            {
                using (var context = new DB_Context())
                {
                    List<SETUPJOB> jobs = context.SETUPJOB.ToList().Where(x => x.IsJobError.Equals(true)).ToList();
                    jobs.ForEach(x => x.IsJobError = false);
                    context.SETUPJOB.UpdateRange(jobs);

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message);
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.SETUPJOB, nameof(EF_ClearAllError));
            }
        }

        public static string EF_GetLoadingTimeFromSettings()
        {
            SETTINGS settings = new SETTINGS();
            try
            {
                using (var context = new DB_Context())
                {
                    settings = context.SETTINGS.ToList().Find(x => x.Items.Contains("Loading Time"));
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }

            return settings.Value.ToString();
        }

        public static void EF_SetDateTimeSetupJob(string jobID, string LotID, JobStatus dtType)
        {
            string dt_Now = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);

            using (var context = new DB_Context())
            {
                try
                {
                    SETUPJOB job = context.SETUPJOB.ToList().Find(x => x.JobID.Equals(jobID) && x.LotNo.Equals(LotID));

                    if (job != null)
                    {
                        LogHelper.General(3, $"[EF_SetDateTimeSetupJob] JodID: {jobID} / LotID: {LotID}");
                        LogHelper.General(3, $"{dtType} DateTime updated");
                        LogHelper.General(3, $"JobID: {jobID}, LotID: {LotID}, {dtType} DateTime updated");
                        switch (dtType)
                        {
                            case JobStatus.Initialize:
                                job.CreatedOn = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                                job.JobStatus = JobStatus.Initialize;
                                break;
                            case JobStatus.Loading:
                                job.LD_START = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                                job.LD_EXPECTED_END = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider).AddMinutes(double.Parse(EF_GetLoadingTimeFromSettings()));
                                job.JobStatus = JobStatus.Loading;
                                break;
                            case JobStatus.Cooling:
                                job.LD_END = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                                job.CT_START = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                                job.CT_END = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider).AddMinutes(double.Parse(job.CoolingTime));
                                job.JobStatus = JobStatus.Cooling;

                                if (job.IsJobError)
                                {
                                    job.IsJobError = false;

                                    Messenger.Default.Send(new LogValue
                                    {
                                        Value = $"[CLEARED]-{job.LotNo}",
                                        Page = HMIPage.LoadingPage
                                    });
                                }
                                break;
                            case JobStatus.Staging:
                                job.ST_START = job.CT_END;
                                job.ST_EXPECTED_END = job.CT_END.Value.AddMinutes(double.Parse(job.StagingTime));
                                job.JobStatus = JobStatus.Staging;
                                break;
                            case JobStatus.Complete:
                                job.ST_END = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                                job.CompletedOn = DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider);
                                job.JobStatus = JobStatus.Complete;

                                if (job.IsJobError)
                                {
                                    job.IsJobError = false;

                                    Messenger.Default.Send(new LogValue
                                    {
                                        Value = $"[CLEARED]-{job.LotNo}",
                                        Page = HMIPage.UnloadingPage
                                    });
                                }
                                break;
                        }

                        context.SETUPJOB.Update(job);
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception($"JOB ID = [{jobID}] Not Found In Database");
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.EF(5, ex.Message.ToString());
                }
                finally
                {
                    EF_UpdateMemory(HMIDBMemory.SETUPJOB, nameof(EF_SetDateTimeSetupJob));
                }
            }

            using (var context = new DB_Context())
            {
                if (!context.SETUPJOB.ToList().Where(x => x.JobStatus.Equals(dtType)).ToList().Any(x => x.IsJobError))
                {
                    switch (dtType)
                    {
                        case JobStatus.Cooling:
                            AlarmService.ClearAlarm(2);
                            break;
                        case JobStatus.Complete:
                            AlarmService.ClearAlarm(1056);
                            break;
                    }
                }
            }
        }

        public static void EF_UpdateLoadportCoolingCompleted(string JobID, string LotID)
        {
            try
            {
                using (var context = new DB_Context())
                {
                    List<JOBDETAILS> carrier = context.JOBDETAILS.Where(x => x.JobID.Equals(JobID) && x.LotNo.Equals(LotID)).ToList();
                    List<LOADPORT> loadport = context.LOADPORT.ToList();

                    if (carrier.Count > 0)
                    {
                        carrier.ForEach(x =>
                        {
                            if (!loadport.Find(y => y.LoadPortID.Equals(x.PortLocation)).LoadPortState.Equals(LoadPortState.UnloadProcess))  // 19-5 lok
                            {
                                loadport.Find(y => y.LoadPortID.Equals(x.PortLocation)).LoadPortState = LoadPortState.CoolingCompleted;
                            }
                        });

                        context.LOADPORT.UpdateRange(loadport);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.EF(5, ex.Message.ToString());
            }
            finally
            {
                EF_UpdateMemory(HMIDBMemory.LOADPORT, nameof(EF_UpdateLoadportCoolingCompleted));
            }
        }

        public static void EF_CompareDateTime()
        {
            List<SETUPJOB> jobs;

            while (isCheckingDateTimeEnable)
            {
                try
                {
                    if (isInitDone)
                    {
                        using (var context = new DB_Context())
                        {
                            jobs = context.SETUPJOB.ToList();

                            if (jobs.Count > 0)
                            {
                                if (jobs.Exists(x => !x.JobStatus.Equals(JobStatus.Complete) && !x.JobStatus.Equals(JobStatus.Initialize)))
                                {
                                    jobs = jobs.Where(x => !x.JobStatus.Equals(JobStatus.Complete) && !x.JobStatus.Equals(JobStatus.Initialize)).ToList();

                                    jobs.ForEach(async x =>
                                    {
                                        string dt_Now = DateTime.Now.ToString(GlobalAttributesClass.TimeFormat);
                                       

                                        switch (x.JobStatus)
                                        {
                                            case JobStatus.Loading:
                                                if (DateTime.Compare(DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider), DateTime.Parse(x.LD_EXPECTED_END.ToString())) > 0)
                                                {
                                                    if (!x.IsJobError)
                                                    {
                                                        x.IsJobError = true;
                                                        EF_InsertEventLog("ERROR", "LOADING", x.JobID, x.LotNo, "", "", "", "Loading Time Expired");
                                                        AlarmService.DefineAlarm(2); //Error
                                                        x.JobStatus = JobStatus.LoadingTimeExpired;
                                                        context.SETUPJOB.Update(x);
                                                        context.SaveChanges();
                                                        EF_UpdateMemory(HMIDBMemory.SETUPJOB, nameof(EF_CompareDateTime));

                                                        Messenger.Default.Send(new LogValue
                                                        {
                                                            Value = $"[ERROR] - Loading Time Exceeded For Lot ID = [{x.LotNo}]",
                                                            Page = HMIPage.LoadingPage
                                                        });
                                                    }
                                                }
                                                break;
                                            case JobStatus.Cooling:
                                                if (DateTime.Compare(DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider), DateTime.Parse(x.CT_END.ToString())) > 0)
                                                {
                                                    string carrierID = context.JOBDETAILS.Where(y => y.JobID.Equals(x.JobID) && y.LotNo.Equals(x.LotNo)).First().CarrierID;
                                                    EF_InsertEventLog("TRACE", "COOLING", x.JobID, x.LotNo, "", "", "", "Cooling Time Ended");

                                                    LogHelper.EF(3, $"[Cooling Time END] - CarrierID: {carrierID} / JobStatus: {JobStatus.Staging}");
                                                    EF_SetDateTimeSetupJob(x.JobID, x.LotNo, JobStatus.Staging);
                                                    EF_InsertEventLog("TRACE", "STAGING", x.JobID, x.LotNo, "", "", "", "Staging Time Started");

                                                    //Sent Job Completed
                                                    //await SecsGemService.SendEventSecsGemAsync(1060, $"{x.JobID},Completed,{x.LotNo}", 104);
                                                    SecsGemService.SendJobTracker(x.JobID, x.LotNo, $"Completed", carrierID).Wait(200);

                                                    EF_InsertEventLog("TRACE", "SECSGEM", x.JobID, x.LotNo, "", "", "", "Sent Event 104-JobCompleted");

                                                    //Sent Job Destroyed
                                                    //await SecsGemService.SendEventSecsGemAsync(1060, $"{x.JobID},Destroyed,{x.LotNo}", 104);
                                                    SecsGemService.SendJobTracker(x.JobID, x.LotNo, $"Destroyed", carrierID).Wait(200);

                                                    EF_InsertEventLog("TRACE", "SECSGEM", x.JobID, x.LotNo, "", "", "", "Sent Event 104-JobDestroyed");

                                                    Messenger.Default.Send(new LogValue
                                                    {
                                                        Value = $"Cooling Ended - Lot ID = [{x.LotNo}]",
                                                        Page = HMIPage.UnloadingPage
                                                    });
                                                   
                                                    EF_SentLoadPortReadyToUnloadAsync(x.JobID);
                                                    EF_UpdateLoadportCoolingCompleted(x.JobID, x.LotNo);
                                                    EF_UpdateMemory(HMIDBMemory.SETUPJOB, $"{nameof(EF_CompareDateTime)} - Cooling");

                                                }
                                                break;
                                            case JobStatus.Staging:
                                                if (DateTime.Compare(DateTime.ParseExact(dt_Now, GlobalAttributesClass.TimeFormat, GlobalAttributesClass.Provider), DateTime.Parse(x.ST_EXPECTED_END.ToString())) > 0)
                                                {
                                                    if (!x.IsJobError)
                                                    {
                                                        x.IsJobError = true;
                                                        EF_InsertEventLog("WARNING", "STAGING", x.JobID, x.LotNo, "", "", "", "Staging Time Expired");
                                                        AlarmService.DefineAlarm(1056); //Warning
                                                        x.JobStatus = JobStatus.StagingTimeExpired;
                                                        context.SETUPJOB.Update(x);
                                                        context.SaveChanges();

                                                        EF_UpdateMemory(HMIDBMemory.SETUPJOB, $"{nameof(EF_CompareDateTime)} - Staging");

                                                        Messenger.Default.Send(new LogValue
                                                        {
                                                            Value = $"[WARNING] - Staging Time Exceeded For Lot ID = [{x.LotNo}]",
                                                            Page = HMIPage.UnloadingPage
                                                        });
                                                    }
                                                }
                                                break;
                                        }
                                    }); 
                                }
                            }
                        } 
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.EF(5, ex.Message.ToString());
                }
            }
        }

        #region Archive
        //public static List<SETUPJOB> EF_GetDataSetupJob()
        //{
        //    List<SETUPJOB> job = new List<SETUPJOB>();
        //    try
        //    {
        //        using (var context = new DB_Context())
        //        {
        //            job = context.SETUPJOB.ToList();
        //        }

        //        return job;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.EF(5, ex.Message.ToString());
        //        throw ex;
        //    }
        //}

        //public static void EF_UpdateSetupJob(string jobID, string lotID, SETUPJOB setupjob)
        //{
        //    try
        //    {
        //        using (var context = new DB_Context())
        //        {
        //            SETUPJOB job = context.SETUPJOB.ToList().Find(x => x.JobID.Equals(jobID));

        //            if (job != null)
        //            {
        //                if (!string.IsNullOrEmpty(setupjob.JobID))
        //                {
        //                    job.JobID = setupjob.JobID;
        //                }

        //                if (!string.IsNullOrEmpty(setupjob.CoolingTime))
        //                {
        //                    job.CoolingTime = setupjob.CoolingTime;
        //                }

        //                if (!string.IsNullOrEmpty(setupjob.StagingTime))
        //                {
        //                    job.StagingTime = setupjob.StagingTime;
        //                }

        //                if (!string.IsNullOrEmpty(setupjob.LoadingTime))
        //                {
        //                    job.LoadingTime = setupjob.LoadingTime;
        //                }

        //                if (!string.IsNullOrEmpty(setupjob.LD_START.ToString()))
        //                {
        //                    job.LD_START = setupjob.LD_START;
        //                }

        //                if (!string.IsNullOrEmpty(setupjob.LD_END.ToString()))
        //                {
        //                    job.LD_END = setupjob.LD_END;
        //                }

        //                if (!string.IsNullOrEmpty(setupjob.CT_START.ToString()))
        //                {
        //                    job.CT_START = setupjob.CT_START;
        //                }

        //                if (!string.IsNullOrEmpty(setupjob.CT_END.ToString()))
        //                {
        //                    job.CT_END = setupjob.CT_END;
        //                }

        //                if (!string.IsNullOrEmpty(setupjob.ST_START.ToString()))
        //                {
        //                    job.ST_START = setupjob.ST_START;
        //                }

        //                if (!string.IsNullOrEmpty(setupjob.ST_END.ToString()))
        //                {
        //                    job.ST_END = setupjob.ST_END;
        //                }

        //                if (!string.IsNullOrEmpty(setupjob.JobStatus.ToString()))
        //                {
        //                    job.JobStatus = setupjob.JobStatus;
        //                }

        //                context.SETUPJOB.Update(job);
        //                context.SaveChanges();
        //            }
        //            else
        //            {
        //                throw new Exception($"JOB ID = [{jobID}] LOT ID = [{lotID}] Not Found In Database");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.EF(5, ex.Message.ToString());
        //        throw ex;
        //    }
        //} 
        #endregion
    }
}
