using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace HMI_CoolingEquipment
{
    public static class GlobalClassFunction
    {
        public static bool ConvertBool(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            else if (value.ToLower().Contains("true"))
            {
                return true;
            }
            else if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            return false;
        }

        #region Mapping
        public static SolidColorBrush ColourMap(LEDColour colour)
        {
            SolidColorBrush result = Brushes.Black;

            switch (colour)
            {
                case LEDColour.RED:
                    result = Brushes.Red;
                    break;
                case LEDColour.GREEN:
                    result = Brushes.Green;
                    break;
                case LEDColour.AMBER:
                    result = Brushes.Orange;
                    break;
                case LEDColour.BLUE:
                    result = Brushes.Blue;
                    break;
                case LEDColour.PINK:
                    result = Brushes.Pink;
                    break;
                case LEDColour.CYAN:
                    result = Brushes.Cyan;
                    break;
                case LEDColour.NoColour:
                    result = Brushes.Transparent;
                    break;
            }

            return result;
        }

        public static List<SetupJobModel> SetupJobMap(List<SETUPJOB> list_setupjob)
        {
            List<SetupJobModel> List_SetupJobModel = new List<SetupJobModel>();
            SetupJobModel SetupJobModel = new SetupJobModel();

            list_setupjob.ForEach(x =>
            {
                SetupJobModel = new SetupJobModel();

                SetupJobModel.JobID = x.JobID;
                SetupJobModel.LotNo = x.LotNo;
                SetupJobModel.CoolingTime = x.CoolingTime;
                SetupJobModel.StagingTime = x.StagingTime;
                SetupJobModel.CT_START = x.CT_START;
                SetupJobModel.CT_END = x.CT_END;
                SetupJobModel.ST_START = x.ST_START;
                SetupJobModel.ST_EXPECTED_END = x.ST_EXPECTED_END;
                SetupJobModel.ST_END = x.ST_END;
                SetupJobModel.LD_START = x.LD_START;
                SetupJobModel.LD_EXPECTED_END = x.LD_EXPECTED_END;
                SetupJobModel.LD_END = x.LD_END;
                SetupJobModel.CreatedOn = x.CreatedOn;
                SetupJobModel.CompletedOn = x.CompletedOn;
                SetupJobModel.JobStatus = x.JobStatus;
                SetupJobModel.NoOfCarrier = x.NoOfCarrier;
                SetupJobModel.IsJobError = x.IsJobError;

                List_SetupJobModel.Add(SetupJobModel);
            });

            return List_SetupJobModel;
        }

        public static List<SetupJobHistoryModel> SetupJobHistoryMap(List<SETUPJOBHISTORY> list_setupjob)
        {
            List<SetupJobHistoryModel> List_SetupJobModel = new List<SetupJobHistoryModel>();
            SetupJobHistoryModel SetupJobModel = new SetupJobHistoryModel();

            list_setupjob.ForEach(x =>
            {
                SetupJobModel = new SetupJobHistoryModel();

                SetupJobModel.JobID = x.JobID;
                SetupJobModel.LotNo = x.LotNo;
                SetupJobModel.CT_START = x.CT_START;
                SetupJobModel.CT_END = x.CT_END;
                SetupJobModel.ST_START = x.ST_START;
                SetupJobModel.ST_END = x.ST_END;
                SetupJobModel.ST_EXPECTED_END = x.ST_EXPECTED_END;
                SetupJobModel.LD_START = x.LD_START;
                SetupJobModel.LD_END = x.LD_END;
                SetupJobModel.LD_EXPECTED_END = x.LD_EXPECTED_END;
                SetupJobModel.CreatedOn = x.CreatedOn;
                SetupJobModel.CompletedOn = x.CompletedOn;
                SetupJobModel.NoOfCarrier = x.NoOfCarrier;
                SetupJobModel.JobDetailsHistory = JobDetailsHistoryMap(x.JobDetails);

                List_SetupJobModel.Add(SetupJobModel);
            });

            return List_SetupJobModel;
        }

        public static SETUPJOBHISTORY SetupJobHistoryMap(SETUPJOB model)
        {
            SETUPJOBHISTORY SetupJobModel = new SETUPJOBHISTORY();

            if (model != null)
            {
                SetupJobModel.JobID = model.JobID;
                SetupJobModel.LotNo = model.LotNo;
                SetupJobModel.CT_START = model.CT_START;
                SetupJobModel.CT_END = model.CT_END;
                SetupJobModel.ST_START = model.ST_START;
                SetupJobModel.ST_END = model.ST_END;
                SetupJobModel.ST_EXPECTED_END = model.ST_EXPECTED_END;
                SetupJobModel.LD_START = model.LD_START;
                SetupJobModel.LD_END = model.LD_END;
                SetupJobModel.LD_EXPECTED_END = model.LD_EXPECTED_END;
                SetupJobModel.CreatedOn = model.CreatedOn;
                SetupJobModel.CompletedOn = model.CompletedOn;
                SetupJobModel.NoOfCarrier = model.NoOfCarrier;
            }

            return SetupJobModel;
        }

        public static List<JobDetailsModel> JobDetailsMap(List<JOBDETAILS> list_jobdetails)
        {
            List<JobDetailsModel> List_JobDetailsModel = new List<JobDetailsModel>();
            JobDetailsModel JobDetailsModel = new JobDetailsModel();

            list_jobdetails.ForEach(x =>
            {
                JobDetailsModel = new JobDetailsModel();

                JobDetailsModel.JobID = x.JobID;
                JobDetailsModel.LotNo = x.LotNo;
                JobDetailsModel.CarrierID = x.CarrierID;
                JobDetailsModel.PortLocation = x.PortLocation;
                JobDetailsModel.LoadingTime = x.LoadingTime;
                JobDetailsModel.UnloadingTime = x.UnloadingTime;
                JobDetailsModel.CarrierStatus = x.CarrierStatus;
                JobDetailsModel.Type = x.Type;
                JobDetailsModel.UnloadBy = x.UnloadBy;
                JobDetailsModel.LoadBy = x.LoadBy;

                List_JobDetailsModel.Add(JobDetailsModel);
            });

            return List_JobDetailsModel;
        }
        
        public static List<JOBDETAILSHISTORY> JobDetailsHistoryMap(List<JOBDETAILS> list_jobdetails)
        {
            List<JOBDETAILSHISTORY> List_JobDetailsModel = new List<JOBDETAILSHISTORY>();
            JOBDETAILSHISTORY JobDetailsModel = new JOBDETAILSHISTORY();

            list_jobdetails.ForEach(x =>
            {
                JobDetailsModel = new JOBDETAILSHISTORY();

                JobDetailsModel.CarrierID = x.CarrierID;
                JobDetailsModel.PortLocation = x.PortLocation;
                JobDetailsModel.LoadingTime = x.LoadingTime;
                JobDetailsModel.LoadBy = x.LoadBy;
                JobDetailsModel.UnloadingTime = x.UnloadingTime;
                JobDetailsModel.UnloadBy = x.UnloadBy;
                JobDetailsModel.Type = x.Type;

                List_JobDetailsModel.Add(JobDetailsModel);
            });

            return List_JobDetailsModel;
        }
        
        public static List<JobDetailsHistory> JobDetailsHistoryMap(List<JOBDETAILSHISTORY> list_jobdetails)
        {
            List<JobDetailsHistory> List_JobDetailsModel = new List<JobDetailsHistory>();
            JobDetailsHistory JobDetailsModel = new JobDetailsHistory();

            list_jobdetails.ForEach(x =>
            {
                JobDetailsModel = new JobDetailsHistory();

                JobDetailsModel.CarrierID = x.CarrierID;
                JobDetailsModel.PortLocation = x.PortLocation;
                JobDetailsModel.LoadingTime = x.LoadingTime;
                JobDetailsModel.LoadBy = x.LoadBy;
                JobDetailsModel.UnloadingTime = x.UnloadingTime;
                JobDetailsModel.UnloadBy = x.UnloadBy;
                JobDetailsModel.Type = x.Type;

                List_JobDetailsModel.Add(JobDetailsModel);
            });

            return List_JobDetailsModel;
        }

        public static List<UserAccountModel> UserAccountMap(List<USERACCOUNT> list_useraccount)
        {
            List<UserAccountModel> List_UserAccountModel = new List<UserAccountModel>();
            UserAccountModel UserAccountModel = new UserAccountModel();

            list_useraccount.ForEach(x =>
            {
                UserAccountModel = new UserAccountModel();

                UserAccountModel.UserName = x.UserName;
                UserAccountModel.Password = x.Password;
                UserAccountModel.FirstName = x.FirstName;
                UserAccountModel.LastName = x.LastName;
                UserAccountModel.UserGroup = x.UserGroup;

                List_UserAccountModel.Add(UserAccountModel);
            });

            return List_UserAccountModel;
        }
        
        public static List<USERACCOUNT> UserAccountMap(List<UserAccountModel> list_useraccount)
        {
            List<USERACCOUNT> List_UserAccountModel = new List<USERACCOUNT>();
            USERACCOUNT UserAccountModel = new USERACCOUNT();

            list_useraccount.ForEach(x =>
            {
                UserAccountModel = new USERACCOUNT();

                UserAccountModel.UserName = x.UserName;
                UserAccountModel.Password = x.Password;
                UserAccountModel.FirstName = x.FirstName;
                UserAccountModel.LastName = x.LastName;
                UserAccountModel.UserGroup = x.UserGroup;

                List_UserAccountModel.Add(UserAccountModel);
            });

            return List_UserAccountModel;
        }
        
        public static List<UserGroupAccessModel> UserGroupAccessMap(List<USERGROUPACCESS> list_usergroupaccess)
        {
            List<UserGroupAccessModel> List_UserGroupAccessModel = new List<UserGroupAccessModel>();
            UserGroupAccessModel UserGroupAccessModel = new UserGroupAccessModel();

            list_usergroupaccess.ForEach(x =>
            {
                UserGroupAccessModel = new UserGroupAccessModel();

                UserGroupAccessModel.UserGroup = x.UserGroup;
                UserGroupAccessModel.LotStatusPageAccess = x.LotStatusPageAccess;
                UserGroupAccessModel.LoadingPageAccess = x.LoadingPageAccess;
                UserGroupAccessModel.UnloadingPageAccess = x.UnloadingPageAccess;
                UserGroupAccessModel.SecsGemPageAccess = x.SecsGemPageAccess;
                UserGroupAccessModel.SettingPageAccess = x.SettingPageAccess;
                UserGroupAccessModel.PTLPageAccess = x.PTLPageAccess;
                UserGroupAccessModel.EditUserGroupAccess = x.EditUserGroupAccess;
                UserGroupAccessModel.EditUserAccountList = x.EditUserAccountList;

                List_UserGroupAccessModel.Add(UserGroupAccessModel);
            });
            
            return List_UserGroupAccessModel;
        }
        
        public static List<USERGROUPACCESS> UserGroupAccessMap(List<UserGroupAccessModel> list_usergroupaccess)
        {
            List<USERGROUPACCESS> List_UserGroupAccessModel = new List<USERGROUPACCESS>();
            USERGROUPACCESS UserGroupAccessModel = new USERGROUPACCESS();

            list_usergroupaccess.ForEach(x =>
            {
                UserGroupAccessModel = new USERGROUPACCESS();

                UserGroupAccessModel.UserGroup = x.UserGroup;
                UserGroupAccessModel.LotStatusPageAccess = x.LotStatusPageAccess;
                UserGroupAccessModel.LoadingPageAccess = x.LoadingPageAccess;
                UserGroupAccessModel.UnloadingPageAccess = x.UnloadingPageAccess;
                UserGroupAccessModel.SecsGemPageAccess = x.SecsGemPageAccess;
                UserGroupAccessModel.SettingPageAccess = x.SettingPageAccess;
                UserGroupAccessModel.PTLPageAccess = x.PTLPageAccess;
                UserGroupAccessModel.EditUserGroupAccess = x.EditUserGroupAccess;
                UserGroupAccessModel.EditUserAccountList = x.EditUserAccountList;

                List_UserGroupAccessModel.Add(UserGroupAccessModel);
            });
            
            return List_UserGroupAccessModel;
        }

        public static List<LoadPortModel> LoadPortMap(List<LOADPORT> list_loadport)
        {
            List<LoadPortModel> List_LoadPortModel = new List<LoadPortModel>();
            LoadPortModel LoadPort = new LoadPortModel();

            list_loadport.ForEach(x =>
            {
                LoadPort = new LoadPortModel()
                {
                    LoadPortID = x.LoadPortID,
                    LoadPortRack = x.LoadPortRack,
                    LoadPortColumn = x.LoadPortColumn,
                    LoadPortRow = x.LoadPortRow,
                    LoadPortPriority = x.LoadPortPriority,
                    LoadPortType = x.LoadPortType,
                    LoadPortCarrierLoad = x.LoadPortCarrierLoad,
                    LoadPortIPAddress = x.LoadPortIPAddress,
                    LoadPortNodeAddress = x.LoadPortNodeAddress,
                    LoadPortNodeConnectionStatus = x.LoadPortNodeConnectionStatus,
                    LoadPortError = x.LoadPortError,
                    LoadPortState = x.LoadPortState,
                    PreAssignLoad = x.PreAssignLoad,
                    LoadPortIsEnable = x.LoadPortIsEnable,
                };
                List_LoadPortModel.Add(LoadPort);
            });

            return List_LoadPortModel;
        }

        public static List<LOADPORT> LoadPortMap(List<LoadPortModel> list_loadport)
        {
            List<LOADPORT> List_LoadPortModel = new List<LOADPORT>();
            LOADPORT LoadPortModel = new LOADPORT();

            list_loadport.ForEach(x =>
            {
                LoadPortModel = new LOADPORT()
                {
                    LoadPortID = x.LoadPortID,
                    LoadPortRack = x.LoadPortRack,
                    LoadPortColumn = x.LoadPortColumn,
                    LoadPortRow = x.LoadPortRow,
                    LoadPortPriority = x.LoadPortPriority,
                    LoadPortType = x.LoadPortType,
                    LoadPortCarrierLoad = x.LoadPortCarrierLoad,
                    LoadPortIPAddress = x.LoadPortIPAddress,
                    LoadPortNodeAddress = x.LoadPortNodeAddress,
                    LoadPortNodeConnectionStatus = x.LoadPortNodeConnectionStatus,
                    LoadPortError = x.LoadPortError,
                    LoadPortState = x.LoadPortState,
                    PreAssignLoad = x.PreAssignLoad,
                    LoadPortIsEnable = x.LoadPortIsEnable,
                };
                List_LoadPortModel.Add(LoadPortModel);
            });

            return List_LoadPortModel;
        }

        public static RackModel RackModelMap(List<LoadPortModel> loadportmodels)
        {
            string rackname = string.Empty;
            string type = string.Empty;
            RackModel RackModel = new RackModel();
            List<RackLoadPort> _rackLoadPorts = new List<RackLoadPort>();
            List<RackLoadPort> tmp_rackLoadPorts = new List<RackLoadPort>();
            List<RackLoadPortList> _trayRack = new List<RackLoadPortList>();
            List<RackLoadPortList> _tubeRack = new List<RackLoadPortList>();

            LoadPortModel lastModel = loadportmodels.LastOrDefault();
            List<string> _rackName = loadportmodels.Select(x => x.LoadPortRack).Distinct().ToList();
            List<LOADPORTCONFIGURATION> _loadportConfig = new List<LOADPORTCONFIGURATION>();

            using (var context = new DB_Context())
            {
                _loadportConfig = context.LOADPORTCONFIGURATION.ToList();
            }

            loadportmodels.ForEach(x =>
            {
                if (string.IsNullOrEmpty(rackname))
                {
                    rackname = x.LoadPortRack;
                }

                if (string.IsNullOrEmpty(type))
                {
                    type = x.LoadPortType;
                }

                if (!x.LoadPortRack.Equals(rackname))
                {
                    _rackLoadPorts.Reverse();

                    //if (type.Equals("Tray"))
                    //{
                    //    _trayRack.Add(new RackLoadPortList
                    //    {
                    //        RackName = rackname,
                    //        RackLoadPorts = _rackLoadPorts,
                    //    });
                    //}
                    //else if (type.Equals("Tube"))
                    //{
                    //    _tubeRack.Add(new RackLoadPortList
                    //    {
                    //        RackName = rackname,
                    //        RackLoadPorts = _rackLoadPorts,
                    //    });
                    //}

                    if (rackname.Equals("AA") || rackname.Equals("AB") || rackname.Equals("AC"))
                    {
                        _trayRack.Add(new RackLoadPortList
                        {
                            RackName = rackname,
                            RackLoadPorts = _rackLoadPorts,
                        });
                    }
                    else if (rackname.Equals("BA") || rackname.Equals("BB"))
                    {
                        _tubeRack.Add(new RackLoadPortList
                        {
                            RackName = rackname,
                            RackLoadPorts = _rackLoadPorts,
                        });
                    }

                    _rackLoadPorts = new List<RackLoadPort>();
                    rackname = x.LoadPortRack;
                    type = x.LoadPortType;
                }

                if (x.LoadPortError)
                {
                    Console.WriteLine(x.LoadPortID);
                }

                tmp_rackLoadPorts.Add(new RackLoadPort
                {
                    LoadPortD = x.LoadPortID,
                    LoadPortStateAndConnection = new Tuple<LoadPortState, bool, bool, bool>(x.LoadPortState, x.LoadPortNodeConnectionStatus, (x.LoadPortState.Equals(LoadPortState.SensorTesting) ? false : _loadportConfig.Find(y => y.LoadPortStatusState.Equals(x.LoadPortState)).LoadPortLEDState.Equals(LEDState.Blinking) || x.LoadPortError) ? true : false, x.LoadPortError),
                });

                //int limit = x.LoadPortType.Equals("Tray") ? 5 : 6;
                int limit = (rackname.Equals("AA") || rackname.Equals("AB") || rackname.Equals("AC")) ? 5 : 6;
                if (tmp_rackLoadPorts.Count().Equals(limit))
                {
                    tmp_rackLoadPorts.Reverse();
                    _rackLoadPorts.AddRange(tmp_rackLoadPorts);
                    tmp_rackLoadPorts = new List<RackLoadPort>();
                }

                if (x.LoadPortID.Equals(lastModel.LoadPortID))
                {
                    _rackLoadPorts.Reverse();

                    _tubeRack.Add(new RackLoadPortList
                    {
                        RackName = rackname,
                        RackLoadPorts = _rackLoadPorts,
                    });
                }

            });

            _tubeRack.FindAll(x => x.RackName.Equals("BA") || x.RackName.Equals("BB")).ForEach(x =>
            {
                x.RackLoadPorts.FindAll(y => y.LoadPortD.Contains("-07-") || y.LoadPortD.Contains("-08-")).ForEach(z =>
                {
                    z.LoadPortVisibility = System.Windows.Visibility.Collapsed;
                });
            });

            RackModel.TubeRack = _tubeRack;
            RackModel.TrayRack = _trayRack;

         return RackModel;
        }

        public static List<ALARMCURRENT> AlarmDefinitionMap(List<AlarmCurrentModel> list_alarmcurrent)
        {
            List<ALARMCURRENT> alarmCurrentList = new List<ALARMCURRENT>();
            ALARMCURRENT alarmCurrent = new ALARMCURRENT();

            list_alarmcurrent.ForEach(x =>
            {
                alarmCurrent = new ALARMCURRENT
                {
                    AlarmDateTime = x.AlarmDateTime,
                    AlarmType = x.AlarmType,
                    AlarmCode = x.AlarmCode,
                    AlarmModule = x.AlarmModule,
                    AlarmMessage = x.AlarmMessage,
                    AlarmAction = x.AlarmAction,
                };

                alarmCurrentList.Add(alarmCurrent);
            });

            return alarmCurrentList;
        }

        public static List<AlarmCurrentModel> AlarmDefinitionMap(List<ALARMCURRENT> list_alarmcurrent)
        {
            List<AlarmCurrentModel> alarmCurrentList = new List<AlarmCurrentModel>();
            AlarmCurrentModel alarmCurrent = new AlarmCurrentModel();

            list_alarmcurrent.ForEach(x =>
            {
                alarmCurrent = new AlarmCurrentModel
                {
                    AlarmDateTime = x.AlarmDateTime,
                    AlarmType = x.AlarmType,
                    AlarmCode = x.AlarmCode,
                    AlarmModule = x.AlarmModule,
                    AlarmMessage = x.AlarmMessage,
                    AlarmAction = x.AlarmAction,
                };

                alarmCurrentList.Add(alarmCurrent);
            });

            return alarmCurrentList;
        }
        
        public static List<AlarmHistoryModel> AlarmDefinitionMap(List<ALARMHISTORY> list_alarmhistory)
        {
            List<AlarmHistoryModel> alarmHistoryList = new List<AlarmHistoryModel>();
            AlarmHistoryModel alarmHistory = new AlarmHistoryModel();

            list_alarmhistory.ForEach(x =>
            {
                alarmHistory = new AlarmHistoryModel
                {
                    AlarmClearedDateTime = x.AlarmClearedDateTime,
                    AlarmDateTime = x.AlarmDateTime,
                    AlarmType = x.AlarmType,
                    AlarmCode = x.AlarmCode,
                    AlarmModule = x.AlarmModule,
                    AlarmMessage = x.AlarmMessage,
                    AlarmAction = x.AlarmAction,
                };

                alarmHistoryList.Add(alarmHistory);
            });

            return alarmHistoryList;
        }

        public static List<ALARMHISTORY> AlarmDefinitionMap(List<AlarmHistoryModel> list_alarmhistory)
        {
            List<ALARMHISTORY> alarmHistoryList = new List<ALARMHISTORY>();
            ALARMHISTORY alarmHistory = new ALARMHISTORY();

            list_alarmhistory.ForEach(x =>
            {
                alarmHistory = new ALARMHISTORY
                {
                    AlarmClearedDateTime = x.AlarmClearedDateTime,
                    AlarmDateTime = x.AlarmDateTime,
                    AlarmType = x.AlarmType,
                    AlarmCode = x.AlarmCode,
                    AlarmModule = x.AlarmModule,
                    AlarmMessage = x.AlarmMessage,
                    AlarmAction = x.AlarmAction,
                };

                alarmHistoryList.Add(alarmHistory);
            });

            return alarmHistoryList;
        }
        
        public static List<SettingsModel> SettingsMap(List<SETTINGS> list_settings)
        {
            List<SettingsModel> settingsList = new List<SettingsModel>();
            SettingsModel settings = new SettingsModel();

            list_settings.ForEach(x =>
            {
                settings = new SettingsModel
                {
                    Items = x.Items,
                    Value = x.Value,
                    Type = x.Type,
                    Page = x.Page,
                };

                settingsList.Add(settings);
            });

            return settingsList;
        }

        public static List<SETTINGS> SettingsMap(List<SettingsModel> list_settings)
        {
            List<SETTINGS> settingsList = new List<SETTINGS>();
            SETTINGS settings = new SETTINGS();

            list_settings.ForEach(x =>
            {
                settings = new SETTINGS
                {
                    Items = x.Items,
                    Value = x.Value,
                    Type = x.Type,
                    Page = x.Page,
                };

                settingsList.Add(settings);
            });

            return settingsList;
        }

        public static List<LoadPortConfigurationModel> LoadPortConfigurationMap(List<LOADPORTCONFIGURATION> list_loadportConfig)
        {
            List<LoadPortConfigurationModel> return_Val = new List<LoadPortConfigurationModel>();
            LoadPortConfigurationModel temp_Model = new LoadPortConfigurationModel();

            list_loadportConfig.ForEach(x =>
            {
                temp_Model = new LoadPortConfigurationModel
                {
                    LoadPortStatusKey = x.LoadPortStatusKey,
                    LoadPortStatusState = x.LoadPortStatusState,
                    LoadPortLEDColour = x.LoadPortLEDColour,
                    LoadPortLEDState = x.LoadPortLEDState,
                };

                return_Val.Add(temp_Model);
            });

            return return_Val;
        }

        public static List<LOADPORTCONFIGURATION> LoadPortConfigurationMap(List<LoadPortConfigurationModel> list_loadportConfig)
        {
            List<LOADPORTCONFIGURATION> return_Val = new List<LOADPORTCONFIGURATION>();
            LOADPORTCONFIGURATION temp_Model = new LOADPORTCONFIGURATION();

            list_loadportConfig.ForEach(x =>
            {
                temp_Model = new LOADPORTCONFIGURATION
                {
                    LoadPortStatusKey = x.LoadPortStatusKey,
                    LoadPortStatusState = x.LoadPortStatusState,
                    LoadPortLEDColour = x.LoadPortLEDColour,
                    LoadPortLEDState = x.LoadPortLEDState,
                };

                return_Val.Add(temp_Model);
            });

            return return_Val;
        }
        #endregion

        #region Encrypt/DecryptPassword
        public static string Encrypt_Password(string password)
        {
            string pswstr = string.Empty;
            byte[] psw_encode = new byte[password.Length];
            psw_encode = Encoding.UTF8.GetBytes(password);
            pswstr = Convert.ToBase64String(psw_encode);
            return pswstr;
        }

        public static string Decrypt_Password(string encryptpassword)
        {
            string pswstr = string.Empty;
            UTF8Encoding encode_psw = new UTF8Encoding();
            Decoder Decode = encode_psw.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpassword);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            pswstr = new String(decoded_char);
            return pswstr;
        }
        #endregion

        public static void CsvWriter(string strPath, string FileName, Dictionary<string, string> Dic_Header_Content, bool isAppendTable = false)
        {
            try
            {
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                bool IsNew = false;
                if (!File.Exists(strPath + Path.DirectorySeparatorChar + FileName))
                    IsNew = true;

                using (FileStream stream = new FileStream((strPath + Path.DirectorySeparatorChar + FileName), IsNew ? FileMode.CreateNew : FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    if (IsNew) writer.WriteLine(string.Join(",", Dic_Header_Content.Keys));

                    if (isAppendTable)
                    {
                        writer.WriteLine();
                        writer.WriteLine(string.Join(",", Dic_Header_Content.Keys));
                    }

                    writer.WriteLine(string.Join(",", Dic_Header_Content.Values));
                    writer.Flush();
                    writer.Close();
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
                throw ex;
            }
        }

        public static void ExcelWriter(string strPath, string FileName, Dictionary<string, string> Dic_Header_Content)
        {

        }
    }
}
