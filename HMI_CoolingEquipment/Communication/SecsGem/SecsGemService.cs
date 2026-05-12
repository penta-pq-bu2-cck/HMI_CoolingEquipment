using GalaSoft.MvvmLight.Messaging;
using GEMPro;
using GEMPro.Database.DbController;
using GEMPro.Database.Dto;
using GEMPro.GEMProObject;
using HMI_CoolingEquipment.Models;
using QynixUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HMI_CoolingEquipment.Communication
{
    public static class SecsGemService
    {
        public static GEMProServer mGEMPro;
        private static string varVal = string.Empty;

        public static string StartJobName = string.Empty;

        #region PublicMethod
        public static void InitializeSecsGem()
        {
            try
            {
                LogHelper.SecsGem(1, $"Initialize SecsGem");
                if (mGEMPro != null)
                {
                    throw new Exception($"Already Initialize !");
                }
                else
                {
                    mGEMPro = new GEMProServer();
                    mGEMPro.Initialize();
                    if (!mGEMPro.StartGEMTool())
                    {
                        throw new Exception($"Failed to Start GEM Tool !");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }

            LogHelper.SecsGem(1, $"Done Initialize SecsGem");
        }

        public static void ConnectSecsGem()
        {
            try
            {
                LogHelper.SecsGem(1, $"Connect SecsGem");
                if (mGEMPro != null)
                {
                    if (!mGEMPro.SECSProControl.IsPortOpened)
                    {
                        InitializeSecsGemSetting();
                    }
                    mGEMPro.SECSProControl.OpenPort();
                    mGEMPro.EstablishComm();
                    LogHelper.SecsGem(1, $"LocalIPPort: {mGEMPro.SECSProControl.LocalIPPort}");
                    LogHelper.SecsGem(1, $"LocalIPAddress: {mGEMPro.SECSProControl.LocalIPAddress}");
                    LogHelper.SecsGem(1, $"RemoteIPPort: {mGEMPro.SECSProControl.RemoteIPPort}");
                    LogHelper.SecsGem(1, $"RemoteIPAddress: {mGEMPro.SECSProControl.RemoteIPAddress}");
                }
                else
                {
                    throw new Exception($"GEMPro is NULL !");
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }

            LogHelper.SecsGem(1, $"Done Connect SecsGem");
        }

        public static void CancelConnectionSecsGem()
        {
            try
            {
                LogHelper.SecsGem(1, $"Cancel Connection SecsGem");

                DisposeSecsGemSetting();
                mGEMPro.SECSProControl.Dispose();
                mGEMPro.Dispose();
                mGEMPro = null;
                mGEMPro = new GEMProServer();
                mGEMPro.Initialize();
                if (!mGEMPro.StartGEMTool())
                {
                    throw new Exception($"Failed to Start GEM Tool !");
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }

            LogHelper.SecsGem(1, $"Done Connection SecsGem");
        }

        public static void DisconnectSecsGem()
        {
            try
            {
                LogHelper.SecsGem(1, $"Disconnect SecsGem");
                if (mGEMPro != null)
                {
                    DisposeSecsGemSetting();
                    mGEMPro.SECSProControl.ClosePort();
                }
                else
                {
                    throw new Exception($"GEMPro is NULL !");
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }

            if (!mGEMPro.SECSProControl.IsPortOpened)
            {
                Messenger.Default.Send(new SecsGemConnectionValue()
                {
                    ConnectionStatus = "Disconnected",
                    IsConnected = false,
                });
            }

            LogHelper.SecsGem(1, $"Done Disconnect SecsGem");
        }

        public static void CloseSecsGem()
        {
            try
            {
                LogHelper.SecsGem(1, $"Closing SecsGem");
                if (mGEMPro.SECSProControl.IsPortOpened)
                {
                    DisconnectSecsGem();
                    mGEMPro.SECSProControl.Dispose();
                    mGEMPro.StopGEMTool();
                    mGEMPro.Dispose();

                }
                else
                {
                    mGEMPro.StopGEMTool();
                    mGEMPro.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }

            LogHelper.SecsGem(1, $"Done Closing SecsGem");
        }

        public static void OpenSettingsSecsGem()
        {
            LogHelper.SecsGem(1, $"Open SecsGem Setting");
            mGEMPro.ShowGEMTool(true);
        }

        public static async Task SendEventSecsGemAsync(int varID, string varValue, int CEID)
        {
            try
            {
                if (mGEMPro.SECSProControl.IsPortOpened)
                {
                    if (!varVal.Equals(varValue))
                    {
                        LogHelper.SecsGem(1, $"{nameof(SendEventSecsGemAsync)} varID: {varID} | varValue : {varValue} | CEID : {CEID}");
                        varVal = varValue;

                        await Task.Run(() =>
                        {
                            VariableInfo<SECSCustomVarDto>.SetVariable(varID, varValue, false);
                            mGEMPro.EventReport().SendEventReportByCEID(CEID);
                        }).ConfigureAwait(false);
                    }
                }
                else
                {
                    throw new Exception("SecsGem not connect");
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }
        }

        public static async Task SendEventSecsGemAsync(int CEID)
        {
            try
            {
                if (mGEMPro.SECSProControl.IsPortOpened)
                {
                    LogHelper.SecsGem(1, $"{nameof(SendEventSecsGemAsync)} CEID : {CEID}");

                    await Task.Run(() =>
                    {
                        mGEMPro.EventReport().SendEventReportByCEID(CEID);
                    }).ConfigureAwait(false);
                }
                else
                {
                    throw new Exception("SecsGem not connect");
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }
        }

        public static void UploadAlarmToDatabase(List<ALARMDEFINITION> alarmDefine)
        {
            try
            {
                List<SECSAlarmDto> oldAlarm = AlarmInfo.Instance.GetAllAlarmList();

                if (oldAlarm.Count > 0)
                {
                    oldAlarm.ForEach(x =>
                    {
                        AlarmInfo.Instance.DeleteAlarmById(Convert.ToInt32(x.AlarmId.ToString()));
                    });
                }

                alarmDefine.ForEach(x =>
                {
                    SECSAlarmDto newAlarm = new SECSAlarmDto();

                    newAlarm.AlarmId = x.AlarmCode;
                    newAlarm.AlarmName = x.AlarmModule;
                    newAlarm.AlarmText = x.AlarmMessage;
                    newAlarm.AlarmSet = false;
                    newAlarm.AlarmClearCeid = 0;
                    newAlarm.AlarmEnable = true;
                    newAlarm.AlarmGroup = x.AlarmType.ToString();
                    newAlarm.AlarmSetCeid = 0;

                    AlarmInfo.Instance.SaveAlarm(newAlarm);
                });
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
                throw ex;
            }
        }

        public static async Task SendAlarmToServer(int int_AlarmID, bool bol_Alarm)
        {
            try
            {
                if (mGEMPro.SECSProControl.IsPortOpened)
                {
                    LogHelper.SecsGem(1, $"{nameof(SendAlarmToServer)} AlarmCode: {int_AlarmID} | AlarmBool : {bol_Alarm}");
                    await Task.Run(() =>
                    {
                        mGEMPro.Alarm().SetAlarmByALID(int_AlarmID, bol_Alarm);
                    }).ConfigureAwait(false);
                }
                else
                {
                    throw new Exception("SecsGem not connect");
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }
        }
        #endregion

        #region PrivateMethod
        private static void InitializeSecsGemSetting()
        {
            try
            {
                if (!mGEMPro.EnableS1F3QueryNotification) mGEMPro.EnableS1F3QueryNotification = true;
                if (mGEMPro.EnableTerminalDisplay) mGEMPro.EnableTerminalDisplay = false;
                if (!mGEMPro.EnableS7F3Notification) mGEMPro.EnableS7F3Notification = true;
                if (!mGEMPro.EnableS6F15QueryNotification) mGEMPro.EnableS6F15QueryNotification = true;
                if (!mGEMPro.EnableS6F19QueryNotification) mGEMPro.EnableS6F19QueryNotification = true;
                //Disable due to direct use GEMPro features.
                if (mGEMPro.EnableS7F17Notification) mGEMPro.EnableS7F17Notification = false;

                mGEMPro.EventReportQueryNotification += GemProServer_EventReportQueryNotification;
                mGEMPro.ReportQueryNotification += GemProServer_ReportQueryNotification;
                mGEMPro.ErrorNotification += GEMPro_ErrorNotification;
                mGEMPro.MethodNotification += GEMPro_MethodNotification;
                mGEMPro.EventNotification += GEMPro_EventNotification;
                mGEMPro.CommandNotification += GEMPro_CommandNotification;
                mGEMPro.QueryNotification += GEMPro_QueryNotification;
                mGEMPro.ECQueryNotification += GEMPro_ECQueryNotification;
                mGEMPro.FormattedRecipeNotification += GEMPro_FormattedRecipeNotification;
                mGEMPro.UnformattedRecipeNotification += GEMPro_UnformattedRecipeNotification;
                mGEMPro.SECSProControl.Error += GEMPro_SECSProErrorEventHandler;
                mGEMPro.SECSProControl.Disconnect += GEMPro_SECSProDisconnectEventHandler;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void DisposeSecsGemSetting()
        {
            try
            {
                mGEMPro.EventReportQueryNotification -= GemProServer_EventReportQueryNotification;
                mGEMPro.ReportQueryNotification -= GemProServer_ReportQueryNotification;
                mGEMPro.ErrorNotification -= GEMPro_ErrorNotification;
                mGEMPro.MethodNotification -= GEMPro_MethodNotification;
                mGEMPro.EventNotification -= GEMPro_EventNotification;
                mGEMPro.CommandNotification -= GEMPro_CommandNotification;
                mGEMPro.QueryNotification -= GEMPro_QueryNotification;
                mGEMPro.ECQueryNotification -= GEMPro_ECQueryNotification;
                mGEMPro.FormattedRecipeNotification -= GEMPro_FormattedRecipeNotification;
                mGEMPro.UnformattedRecipeNotification -= GEMPro_UnformattedRecipeNotification;
                mGEMPro.SECSProControl.Error -= GEMPro_SECSProErrorEventHandler;
                mGEMPro.SECSProControl.Disconnect -= GEMPro_SECSProDisconnectEventHandler;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Notification
        public static void GemProServer_EventReportQueryNotification(GEMProEvtRptQuery gemEvtRptQuery)
        {
            try
            {
                LogHelper.SecsGem(1, $"{nameof(GemProServer_EventReportQueryNotification)} CEID : {gemEvtRptQuery.CEID}");
                mGEMPro.ReplyEvtRptQuery(gemEvtRptQuery);
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, $"{nameof(GemProServer_EventReportQueryNotification)} Error : {ex.Message}");
            }
        }

        public static void GemProServer_ReportQueryNotification(GEMProReportQuery gemReportQuery)
        {
            try
            {
                LogHelper.SecsGem(1, $"{nameof(GemProServer_ReportQueryNotification)} CEID : {gemReportQuery.ReportID}");
                mGEMPro.ReplyReportQuery(gemReportQuery);
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, $"{nameof(GemProServer_ReportQueryNotification)} Error : {ex.Message}");
            }
        }

        public static void GEMPro_ErrorNotification(object sender, EventArgs e)
        {
            try
            {
                string strErrorMessage;
                GEMProErrNotificationEventArgs arg = (GEMProErrNotificationEventArgs)e;

                strErrorMessage = string.Format("{0} - Line: {1} [{2}] {3}", arg.GEMProError.ErrSource, arg.GEMProError.ErrLine, arg.GEMProError.ErrCode, arg.GEMProError.ErrText);

                LogHelper.SecsGem(5, $"{nameof(GEMPro_ErrorNotification)} ErrorMessage : {strErrorMessage}");
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, $"{nameof(GEMPro_ErrorNotification)} Error : {ex.Message}");
            }
        }

        public static void GEMPro_MethodNotification(object sender, EventArgs e)
        {
            try
            {
                string strMethodMessage;
                MethodNotificationEventArgs arg = (MethodNotificationEventArgs)e;

                strMethodMessage = string.Format("[{0}] {1}", arg.GEMProMethod.ServiceName, arg.GEMProMethod.MethodName);

                // for logging purpose start
                // This show how to get the input list from host
                if (arg.GEMProMethod.InputList != null)
                {
                    foreach (GEMProItem item in arg.GEMProMethod.InputList)
                    {
                        strMethodMessage += string.Format(" {{INPUT:{0}={1}}}", item.ItemName, item.ItemValue == null ? string.Empty : item.ItemValue.ToString());
                    }
                }

                // This show how to get the output list from host
                if (arg.GEMProMethod.OutputList != null)
                {
                    foreach (GEMProItem item in arg.GEMProMethod.OutputList)
                    {
                        strMethodMessage += string.Format(" {{OUTPUT:{0}={1}}}", item.ItemName, item.ItemValue == null ? string.Empty : item.ItemValue.ToString());
                    }
                }
                if (arg.GEMProMethod.ServiceName == "GEMGetAttrReq" && arg.GEMProMethod.MethodName == "GetStdAttrReq")
                {
                    if (arg.GEMProMethod.OutputList != null)
                    {
                        var objID = arg.GEMProMethod.InputList.Where(x => x.ItemName == "OBJID").FirstOrDefault().ItemValue.ToString();

                        var attrIDList = arg.GEMProMethod.InputList.Where(x => x.ItemName == "ATTRID").FirstOrDefault().ItemValue.ToString();

                        var objAck = arg.GEMProMethod.OutputList.Where(x => x.ItemName == "OBJACK").FirstOrDefault().ItemValue.ToString();

                        var attrDataOutputList = arg.GEMProMethod.OutputList.Where(x => x.ItemName == "ATTRDATA").FirstOrDefault();

                        if (objAck == "0")
                        {
                            var attrDataRequestToHostArray = attrIDList.Split(',');
                            var attrDataReplyFromHosttArray = attrDataOutputList.ItemValue.ToString().Split(',');

                            foreach (var attrDataRequestToHost in attrDataRequestToHostArray)
                            {
                                string attributeDataRequestToHost = attrDataRequestToHost + "=";

                                foreach (var attrDataReplyFromHost in attrDataReplyFromHosttArray)
                                {
                                    string attrDataReply = String.Empty;

                                    if (!string.IsNullOrEmpty(attrDataReplyFromHost))
                                    {
                                        attrDataReply = attrDataReplyFromHost.ToString();
                                    }

                                    // Validate attribute MAPDATA exist in host reply
                                    if (attrDataReply.Contains("OBJID"))
                                    {
                                        // Add code if need to check OBJID
                                    }
                                    else if (attrDataReply.Contains(attributeDataRequestToHost))
                                    {
                                        // Add code for MAPDATA
                                    }
                                    else
                                    {
                                        strMethodMessage += "S14F2 replied from host failed - mis-matched attribute ID";
                                    }
                                }
                            }
                        }
                        else
                        {
                            strMethodMessage += String.Format("S14F2 replied from host failed with reject code - OBJACK = {0}.", objAck);
                        }
                    }
                }

                LogHelper.SecsGem(1, $"{nameof(GEMPro_MethodNotification)} MethodMessage : {strMethodMessage}");
                CLogger.Instance.Info(strMethodMessage);
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, $"{nameof(GEMPro_MethodNotification)} Error : {ex.Message}");
            }
        }

        public static void GEMPro_EventNotification(object sender, System.EventArgs e)
        {
            try
            {
                var gemEvent = (e as GEMProEventNotificationEventArgs).GEMProEvent;

                var infoText = $"[{gemEvent.ServiceName}] {gemEvent.EventName}";

                IEnumerable<string> combineNameValue = null;

                if (gemEvent.Outputs != null && gemEvent.Outputs.Count() > 0)
                {
                    combineNameValue =
                        from output in gemEvent.Outputs
                        select $" {output.ItemName}={output.ItemValue.ToString()}";

                    infoText += combineNameValue.Aggregate((x, y) => x + y);
                }

                LogHelper.SecsGem(1, $"{nameof(GEMPro_EventNotification)} Event : {infoText}");

                switch (gemEvent.ServiceName)
                {
                    case GEMProConstant.SRVName.GEMPROTOOL:
                        HandleSRVGEMProTool(gemEvent);
                        break;

                    case GEMProConstant.SRVName.SECSPROTOCOL:
                        HandleSRVSECSProtocol(gemEvent);
                        break;

                    case GEMProConstant.SRVName.CONTROL:
                        HandleSRVControl(gemEvent);
                        break;

                    case GEMProConstant.SRVName.COMM:
                        HandleSRVCommunicationState(gemEvent);
                        break;

                    case GEMProConstant.SRVName.TERMINAL:
                        HandleSRVTerminalDisplay(gemEvent);
                        break;
                }

            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, $"{nameof(GEMPro_EventNotification)} Error : {ex.Message}");
            }
        }

        public static void GEMPro_CommandNotification(object sender, CommandNotificationEventArgs e)
        {
            //HCACK = 0 - Acknowledge, command has been performed
            //HCACK = 1 - Command() does Not exist
            //HCACK = 2 - Cannot perform now
            //HCACK = 3 - At least one parameter Is invalid
            //HCACK = 4 - Acknowledge, command will be performed later
            //HCACK = 5 - Rejected, Not In remote mode / already In desired condition
            //HCACK = 6 - Invalid Object

            //CPACK = 1 - Parameter Name(CPNAME) does Not exist
            //CPACK = 2 - Illegal Value specified For CPVAL
            //CPACK = 3 - Illegal Format specified For CPVAL

            string strCommandMessage;
            string sDataType;
            CommandNotificationEventArgs arg = e;

            try
            {
                #region Check SecsGem Status
                if (VariableInfo<SECSGEMVarDto>.Instance.ControlState != GEMProEnum.GEMControlState.ONLINE_REMOTE)
                {
                    LogHelper.SecsGem(2, $"{nameof(GEMPro_CommandNotification)} | Cannot perform now ! Command Received = [{arg.GEMProCommand.RCMD}]");
                    arg.GEMProCommand.HCACK = 2;
                    //mGEMPro.ReplyCommand(arg.GEMProCommand);
                    return;
                }
                #endregion

                #region Checking Command if its not exist
                if (arg.GEMProCommand.RCMD != "SETUPJOB" && arg.GEMProCommand.RCMD != "STARTJOB" && arg.GEMProCommand.RCMD != "PREPARE_UNLOAD" && arg.GEMProCommand.RCMD != "EQP_Tooling")
                {
                    LogHelper.SecsGem(5, $"{nameof(GEMPro_CommandNotification)} | Command Not Exist ! Command Received = [{arg.GEMProCommand.RCMD}]");
                    arg.GEMProCommand.HCACK = 1;
                    //mGEMPro.ReplyCommand(arg.GEMProCommand);
                    return;
                }
                #endregion

                #region Command Handler
                strCommandMessage = string.Format("RCMD={0}", arg.GEMProCommand.RCMD);

                if (arg.GEMProCommand.IsEnhanced)
                {
                    switch (arg.GEMProCommand.RCMD)
                    {
                        case "SETUPJOB":

                            SETUPJOB setupjob = new SETUPJOB();
                            List<JOBDETAILS> jobs = new List<JOBDETAILS>();

                            strCommandMessage += string.Format(" OBJSPEC={0}", arg.GEMProCommand.OBJSPEC);
                            foreach (GEMProEnCommandItem enItem in arg.GEMProCommand.InputList)
                            {
                                sDataType = GEMProConstant.Instance.SECSItemFormatDict[(int)enItem.CPFORMAT];
                                strCommandMessage += string.Format(" {{INPUT:{0}/{1}={2}}}", enItem.CPNAME, sDataType, enItem.CEPVAL.ToString());

                                if (enItem.CPNAME.Contains("JobParameters"))
                                {
                                    if (enItem.CPNAME.Contains("JobName"))
                                    {
                                        setupjob.JobID = enItem.CEPVAL.ToString();
                                    }
                                    
                                    if (enItem.CPNAME.Contains("LotNumber"))
                                    {
                                        setupjob.LotNo = enItem.CEPVAL.ToString();
                                    }
                                }

                                if (enItem.CPNAME.Contains("RecipeParameter"))
                                {
                                    if (enItem.CPNAME.Contains("CT"))
                                    {
                                        setupjob.CoolingTime = enItem.CEPVAL.ToString();
                                    }

                                    if (enItem.CPNAME.Contains("ST"))
                                    {
                                        setupjob.StagingTime = enItem.CEPVAL.ToString();
                                    }
                                }

                                if (enItem.CPNAME.Contains("Carriers"))
                                {
                                    string type = string.Empty;

                                    if (enItem.CEPVAL.ToString().Substring(0, 4).Equals("CRID"))
                                    {
                                        type = "Tube";
                                    }
                                    else if(enItem.CEPVAL.ToString().Substring(0, 4).Equals("TRID"))
                                    {
                                        type = "Tray";
                                    }
                                    else
                                    {
                                        arg.GEMProCommand.HCACK = 3;
                                        LogHelper.SecsGem(5, $"{nameof(GEMPro_CommandNotification)} ErrorMessage : Invalid Carrier ID / Unknown Carrier Type");
                                        return;
                                    }

                                    jobs.Add(new JOBDETAILS()
                                    {
                                        
                                        JobID = setupjob.JobID,
                                        LotNo = setupjob.LotNo,
                                        CarrierID = enItem.CEPVAL.ToString(),
                                        Type = type,
                                        CarrierStatus = JobStatus.Initialize,
                                    });
                                    LogHelper.General(2, $"Send setup job command, JobID={setupjob.JobID}, LotNo={setupjob.LotNo}, CarrierID= {enItem.CEPVAL.ToString()}");
                                }
                            }

                            if (arg.GEMProCommand.HCACK == 0)
                            {
                                if (string.IsNullOrEmpty(setupjob.CoolingTime) || string.IsNullOrEmpty(setupjob.StagingTime) || !float.TryParse(setupjob.CoolingTime, out float CT) || !float.TryParse(setupjob.StagingTime, out float ST))
                                {
                                    arg.GEMProCommand.HCACK = 3;
                                    LogHelper.SecsGem(5, $"{nameof(GEMPro_CommandNotification)} ErrorMessage : CT/ST is empty or not numeric");
                                }
                                else if (float.Parse(setupjob.CoolingTime) <= 0 || float.Parse(setupjob.StagingTime) <= 0)
                                {
                                    arg.GEMProCommand.HCACK = 3;
                                    LogHelper.SecsGem(5, $"{nameof(GEMPro_CommandNotification)} ErrorMessage : CT/ST is 0");
                                }
                                else if (jobs.Count <= 0)
                                {
                                    arg.GEMProCommand.HCACK = 3;
                                    LogHelper.SecsGem(5, $"{nameof(GEMPro_CommandNotification)} ErrorMessage : Carrier List is empty");
                                }
                                else if (EF_Service.EF_SetupJobChecking(setupjob.JobID, setupjob.LotNo))
                                {
                                    arg.GEMProCommand.HCACK = 3;
                                    LogHelper.SecsGem(5, $"{nameof(GEMPro_CommandNotification)} ErrorMessage : {setupjob.JobID} Duplicated Job ID | {setupjob.LotNo} Duplicated Lot ID");
                                }
                                else
                                {
                                    setupjob.NoOfCarrier = jobs.Count();
                                    LogHelper.General(2, $"Send setup job command, JobID={setupjob.JobID}, LotNo={setupjob.LotNo}");
                                    // Carrier rows must land in JOBDETAILS *before* the SETUPJOB memory broadcast,
                                    // otherwise the LotStatus "Carriers List" converter (live DB query) renders empty.
                                    EF_Service.EF_InsertJobDetails(jobs);
                                    EF_Service.EF_InsertSetupJob(setupjob);
                                    EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", setupjob.JobID, setupjob.LotNo, "", "", "", "Received S2F49-SETUPJOB Command");
                                    arg.GEMProCommand.HCACK = 0;
                                }
                            }
                            
                            break;
                        case "EQP_Tooling":
                            strCommandMessage += string.Format(" OBJSPEC={0}", arg.GEMProCommand.OBJSPEC);
                            foreach (GEMProEnCommandItem enItem in arg.GEMProCommand.InputList)
                            {
                                sDataType = GEMProConstant.Instance.SECSItemFormatDict[(int)enItem.CPFORMAT];
                                strCommandMessage += string.Format(" {{INPUT:{0}/{1}={2}}}", enItem.CPNAME, sDataType, enItem.CEPVAL.ToString());

                                Console.WriteLine(enItem.CPNAME);
                            }
                            break;
                    }
                }
                else
                {
                    #region Loop for command with command parameter
                    if (arg.GEMProCommand.InputList.Count > 0)
                    {
                        switch (arg.GEMProCommand.RCMD)
                        {
                            case "PREPARE_UNLOAD":
                                foreach (GEMProCommandItem enItem in arg.GEMProCommand.InputList)
                                {
                                    if (enItem.CPNAME.Contains("CarrierId"))
                                    {
                                        Debug.WriteLine(enItem.CPVAL.ToString());
                                    }

                                    if (enItem.CPNAME.Contains("PortId"))
                                    {
                                        Debug.WriteLine(enItem.CPVAL.ToString());
                                    }
                                }
                                break;

                            case "STARTJOB":
                                foreach (GEMProCommandItem enItem in arg.GEMProCommand.InputList)
                                {
                                    if (enItem.CPNAME.Contains("JobName"))
                                    {
                                        StartJobName = enItem.CPVAL.ToString();
                                        EF_Service.EF_InsertEventLog("TRACE", "SECSGEM", StartJobName, "", "", "", "", "Received S2F41-STARTJOB Command");
                                        arg.GEMProCommand.HCACK = 0;
                                    }
                                }
                                break;

                            default:
                                break;
                        }

                        foreach (GEMProCommandItem item in arg.GEMProCommand.InputList)
                        {
                            sDataType = GEMProConstant.Instance.SECSItemFormatDict[(int)item.CPFORMAT];
                            strCommandMessage += string.Format(" {{INPUT:{0}/{1}={2}}}", item.CPNAME, sDataType, item.CPVAL);
                        }
                    }
                    #endregion
                }
                #endregion

                LogHelper.General(3, $"{strCommandMessage}");
                LogHelper.SecsGem(1, $"{nameof(GEMPro_CommandNotification)} {strCommandMessage}");
                CLogger.Instance.Info(strCommandMessage);
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, $"{nameof(GEMPro_CommandNotification)} ErrorMessage : {ex.Message}");
                arg.GEMProCommand.HCACK = 6;
            }
            finally
            {
                // To reply to HCACK and CPACK for host for pass all check
                mGEMPro.ReplyCommand(arg.GEMProCommand);
            }
        }

        public static void GEMPro_QueryNotification(GEMProQuery gemQuery)
        {
            try
            {
                string varList = string.Empty;
                bool checkGEMVar = true;

                foreach (var vid in gemQuery.VarList)
                {
                    if((uint)vid.VarId.Value > 100)
                    {
                        checkGEMVar = false;
                        break;
                    }
                }

                if (checkGEMVar)
                {
                    mGEMPro.ReplyQuery(gemQuery);
                }
                else
                {
                    // Only example. It can direct update when change in your own program.
                    //foreach (var vid in gemQuery.VarList)
                    //{
                    //    if((uint)vid.VarId.Value > 100 && vid.VarType.Equals("SV"))
                    //    {
                    //        varList += "[" + vid.VarId.ToString() + "]";
                    //        switch (vid.VarId.ToString())
                    //        {
                    //            //case "1":
                    //            //    VariableInfo<SECSGEMVarDto>.SetVariable(1, "1", true);
                    //            //    break;
                    //            //case "2":
                    //            //    VariableInfo<SECSGEMVarDto>.SetVariable(2, "2", true);
                    //            //    break;
                    //            //case "3":
                    //            //    VariableInfo<SECSGEMVarDto>.SetVariable(3, "30s", true);
                    //            //    break;
                    //            //case "4":
                    //            //    VariableInfo<SECSGEMVarDto>.SetVariable(4, "30s", true);
                    //            //    break;
                    //            //case "5":
                    //            //    VariableInfo<SECSGEMVarDto>.SetVariable(5, "30s", true);
                    //            //    break;
                    //            //case "43":
                    //            //    //DV 43 Process State is GEM variable, therefore need use SECSGEMVarDto and equal true
                    //            //    VariableInfo<SECSGEMVarDto>.SetVariable(43, "1", true);
                    //            //    break;
                    //            //case "101":
                    //            //    VariableInfo<SECSCustomVarDto>.SetVariable(103, "Unit1,Unit2,Unit3,Unit4,Unit5,Unit6,Unit7,Unit8,Unit9,Unit10,Unit11,Unit12,Unit13,Unit14,Unit15,Unit16,Unit17,Unit18,Unit19,Unit20,Unit21,Unit22,Unit23,Unit24,Unit25,Unit26,Unit27,Unit28,Unit29,Unit30,Unit31,Unit32,Unit33,Unit34,Unit35,Unit36,Unit37,Unit38,Unit39,Unit40,Unit41,Unit42,Unit43,Unit44,Unit45,Unit46,Unit47,Unit48,Unit49,Unit50", false);
                    //            //    break;
                    //            //default:
                    //            //    break;
                    //        }
                    //    }
                    //}
                }

                LogHelper.SecsGem(1, $"{nameof(GEMPro_QueryNotification)} VarList : {varList}");
                mGEMPro.ReplyQuery(gemQuery);
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, $"{nameof(GEMPro_QueryNotification)} ErrorMessage : {ex.Message}");
            }
        }

        public static void GEMPro_ECQueryNotification(GEMProECQuery gemEcQuery)
        {
            try
            {
                string varList = string.Empty;

                foreach (var vid in gemEcQuery.VarList)
                {
                    varList += "[" + vid.VarId.ToString() + "]";
                }

                LogHelper.SecsGem(1, $"{nameof(GEMPro_ECQueryNotification)} VarList : {varList}");
                mGEMPro.ReplyECQuery(gemEcQuery);
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, $"{nameof(GEMPro_ECQueryNotification)} ErrorMessage : {ex.Message}");
            }
        }

        public static void GEMPro_FormattedRecipeNotification(object sender, RecipeNotificationEventArgs e)
        {
            string strRecipeData;
            RecipeNotificationEventArgs arg = (RecipeNotificationEventArgs)e;
            try
            {

                // ACKC7
                // 0 - Accepted
                // 1 - Permission Not granted
                // 2 - length error
                // 3 - matrix overflow
                // 4 - PPID Not found
                // 5 - unsupported mode
                // 6 - other error

                // Check if the command is S7F23
                if (arg.GEMProFormattedRecipe.IsS7F23)
                {
                    // Retrieve and store the recipe content
                    strRecipeData = arg.GEMProFormattedRecipe.CCODENPPARM;

                    LogHelper.SecsGem(1, $"{nameof(GEMPro_FormattedRecipeNotification)} RecipeData : {strRecipeData}");

                    // Perform action based on the recipe received from the host

                    // Reply acknowledgement code back to host
                    arg.GEMProFormattedRecipe.ACKC7 = 0;
                }
                else // Check if the command is S7F25
                {
                    // All recipe in tool, please retrieve by your own logic. Hardcoded in this sample.
                    string strAllRecipeName = "AOI_Strip_11";

                    // Get Recipe Directory
                    string strRecipeDirectory = VariableInfo<SECSGEMVarDto>.GetVariable("PPDirectory", true).VarVal;

                    // Get PPID
                    string strPpid = arg.GEMProFormattedRecipe.PPID.Replace(strRecipeDirectory + "\\", "");

                    LogHelper.SecsGem(1, $"{nameof(GEMPro_FormattedRecipeNotification)} RecipeName : {strAllRecipeName} | RecipeDirectory : {strRecipeDirectory} | PPID : {strPpid}");

                    // Recipe Validation
                    if (strAllRecipeName.Contains(strPpid))
                    {
                        // Build the recipe into S7F26 and return to host

                        arg.GEMProFormattedRecipe.CCODENPPARM = "T1,TEMPSET:25,MIN:20,MAX:35,DURATION:10;T2,TEMPSET:50,MIN:20,MAX:55,DURATION:10";
                        // or
                        // arg.GEMProFormattedRecipe.CCODENPPARM = String.Format("CMD001,{0}", xmlData);   //xml String
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, $"{nameof(GEMPro_FormattedRecipeNotification)} ErrorMessage : {ex.Message}");
                arg.GEMProFormattedRecipe.ACKC7 = 6;
            }
            finally
            {
                // Reply to the host
                mGEMPro.ReplyFormattedRecipe(arg.GEMProFormattedRecipe);
            }
        }

        public static void GEMPro_UnformattedRecipeNotification(object sender, UnformattedRecipeNotificationEventArgs e)
        {
            var strRecipeBody = string.Empty;
            var byteRecipeBody = new byte[] { };
            UnformattedRecipeNotificationEventArgs arg = (UnformattedRecipeNotificationEventArgs)e;

            // ACKC7
            // 0 - Accepted
            // 1 - Permission Not granted
            // 2 - length error
            // 3 - matrix overflow
            // 4 - PPID Not found
            // 5 - unsupported mode
            // 6 - other error

            try
            {
                if (arg.GEMProUnformattedRecipe.IsS7F3)
                {
                    bool bCompareWrong = false;

                    // Retrieve and store the recipe content
                    if (arg.GEMProUnformattedRecipe.IsBinary)
                    {
                        byteRecipeBody = arg.GEMProUnformattedRecipe.PPBODYINBYTES;
                    }
                    else
                    {
                        strRecipeBody = arg.GEMProUnformattedRecipe.PPBODY;
                    }

                    // arg.GEMProUnformattedRecipe.PPID included the folder location
                    string recipePath = arg.GEMProUnformattedRecipe.PPID;

                    // Get Recipe Directory
                    string strRecipeDirectory = VariableInfo<SECSGEMVarDto>.GetVariable("PPDirectory", true).VarVal;

                    // Get PPID
                    string strRecipeName = recipePath.ToString().Replace(strRecipeDirectory + "\\", "");

                    LogHelper.SecsGem(1, $"{nameof(GEMPro_FormattedRecipeNotification)} RecipeName : {strRecipeName} | RecipeDirectory : {strRecipeDirectory} | PPID : {recipePath}");

                    // Dummy content from host for sample only - start
                    string content = "AA";

                    if (content == "AA")
                    {
                        // Perform action based on the recipe received from the host
                    }
                    else
                    {
                        bCompareWrong = true;
                    }

                    if (bCompareWrong)
                    {
                        arg.GEMProUnformattedRecipe.ACKC7 = 2;
                        mGEMPro.ReplyUnformattedRecipe(arg.GEMProUnformattedRecipe);

                        //InvalidRecipeBodyDownloaded
                        VariableInfo<SECSCustomVarDto>.SetVariable(107, strRecipeName, false);
                        mGEMPro.EventReport().SendEventReportByCEID(130);
                        return;
                    }
                    // Dummy content from host for sample only - end

                    // Reply acknowledgement code back to host
                    arg.GEMProUnformattedRecipe.ACKC7 = 0;
                }
                else
                {
                    // GEMPro will auto send the recipe body based on the recipe file in recipe directory.
                    // If need check then need enable mGEMPro.EnableS7F5Notification = true; in start.
                    // Without enable the notification wont reach here.
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, $"{nameof(GEMPro_UnformattedRecipeNotification)} ErrorMessage : {ex.Message}");
                arg.GEMProUnformattedRecipe.ACKC7 = 6;
            }
            finally
            {
                // Reply to the host
                mGEMPro.ReplyUnformattedRecipe(arg.GEMProUnformattedRecipe);
            }
        }

        public static void GEMPro_SECSProErrorEventHandler(long ErrorCode, string ErrorText, object Param)
        {
            LogHelper.SecsGem(5, $"{nameof(GEMPro_SECSProErrorEventHandler)} | ErrorCode : {ErrorCode} | ErrorMessage : {ErrorText} | Parameter : {Param}");
        }
        
        public static void GEMPro_SECSProDisconnectEventHandler(long ErrorCode, string ErrorText)
        {
            if (!string.IsNullOrEmpty(ErrorText))
            {
                LogHelper.SecsGem(5, $"{nameof(GEMPro_SECSProDisconnectEventHandler)} | ErrorCode : {ErrorCode} | ErrorMessage : {ErrorText}"); 
            }
        }
        #endregion

        #region Handler
        public static void HandleSRVGEMProTool(GEMProEvent gemEvent)
        {
            if (gemEvent != null)
            {
                LogHelper.SecsGem(1, $"{nameof(HandleSRVGEMProTool)} | GEMProTool : {gemEvent.EventName}");
            }

            switch (gemEvent.EventName)
            {
                case "Load":
                    //Enter your code
                    return;

                case "Unload":
                    //Enter your code
                    return;
            }
        }

        public static void HandleSRVSECSProtocol(GEMProEvent gemEvent)
        {
            if (gemEvent != null)
            {
                LogHelper.SecsGem(1, $"{nameof(HandleSRVSECSProtocol)} | SECSProtocol : {gemEvent.EventName}");
            }

            switch (gemEvent.EventName)
            {
                case "Connect":
                    Messenger.Default.Send(new SecsGemConnectionValue()
                    {
                        ConnectionStatus = "Connected",
                        IsConnected = true,
                    });
                    break;

                case "Disconnect":
                    Messenger.Default.Send(new SecsGemConnectionValue()
                    {
                        ConnectionStatus = "Disconnected",
                        IsConnected = false,
                    });
                    break;

                default:
                    //Enter your code
                    break;
            }
        }

        public static void HandleSRVControl(GEMProEvent gemEvent)
        {
            if (gemEvent.EventName == "StateChanged")
            {

                var stateItem =
                    (from a in gemEvent.Outputs
                     where a.ItemName == "State"
                     select a).FirstOrDefault();

                if (stateItem != null)
                {
                    LogHelper.SecsGem(1, $"{nameof(HandleSRVControl)} | StateChanged : {stateItem.ItemValue}");
                }

                if (stateItem != null)
                {
                    switch (stateItem.ItemValue)
                    {
                        case "OnlineRemote":
                            //Enter your code
                            break;

                        case "OnlineLocal":
                            //Enter your code
                            break;

                        default:
                            //Enter your code
                            break;
                    }
                }
            }
        }

        public static void HandleSRVCommunicationState(GEMProEvent gemEvent)
        {
            if (gemEvent.EventName == GEMProConstant.EventName.STATECHANGED)
            {
                var stateItem =
                    (from a in gemEvent.Outputs
                     where a.ItemName == GEMProConstant.ItemName.STATE
                     select a).FirstOrDefault();

                if (stateItem != null)
                {
                    LogHelper.SecsGem(1, $"{nameof(HandleSRVCommunicationState)} | StateChanged : {stateItem.ItemValue}");
                }

                if (stateItem != null)
                {
                    switch (stateItem.ItemValue)
                    {
                        case "EnabledCommunicating":
                            //Enter your code
                            break;

                        case "EnabledNotCommunicating":
                            //Enter your code
                            break;

                        case "Disabled":
                            //Enter your code
                            break;

                        default:
                            //Enter your code
                            break;
                    }
                }
            }
        }

        public static async void HandleSRVTerminalDisplay(GEMProEvent gemEvent)
        {
            if (gemEvent.EventName == GEMProConstant.EventName.MESSAGERECEIVED)
            {
                var textItem =
                    (from a in gemEvent.Outputs
                     where a.ItemName == GEMProConstant.ItemName.TEXT
                     select a).FirstOrDefault();

                var text = textItem.ItemValue;
                text = text.ToString().Replace("\n", " ");
                // TODO : log down the text

                if (text != null)
                {
                    LogHelper.SecsGem(1, $"{nameof(HandleSRVTerminalDisplay)} | Message : {text}");
                }

                try
                {
                    if (MessageBox.Show(text.ToString(), "Host", MessageBoxButton.OK, MessageBoxImage.Information).Equals(MessageBoxResult.OK))
                    {
                        await Task.Run(() =>
                        {
                            mGEMPro.EventReport().SendEventReportByCEID(10);
                        }).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion

        #region NewLogicForSecsGem
        public static async Task SendJobTracker(string jobID, string lotID, string jobStatus, string carrierID)
        {
            try
            {
                if (mGEMPro.SECSProControl.IsPortOpened)
                {
                    await Task.Run(() =>
                    {
                        VariableInfo<SECSCustomVarDto>.SetVariable(101, jobID, false);
                        VariableInfo<SECSCustomVarDto>.SetVariable(102, carrierID, false);
                        VariableInfo<SECSCustomVarDto>.SetVariable(105, lotID, false);
                        VariableInfo<SECSCustomVarDto>.SetVariable(104, jobStatus, false);
                        mGEMPro.EventReport().SendEventReportByCEID(104);
                    });
                }
                else
                {
                    throw new Exception("SecsGem not connect");
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }
        }

        public static async Task SendLoadPortReadyToLoad(string loadportID)
        {
            try
            {
                if (mGEMPro.SECSProControl.IsPortOpened)
                {
                    await Task.Run(() =>
                    {
                        VariableInfo<SECSCustomVarDto>.SetVariable(103, loadportID, false);
                        mGEMPro.EventReport().SendEventReportByCEID(101);
                    });
                }
                else
                {
                    throw new Exception("SecsGem not connect");
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }
        }

        public static async Task SendLoadPortToBlocked(string lotID, string carrierID, string loadportID)
        {
            try
            {
                if (mGEMPro.SECSProControl.IsPortOpened)
                {
                    await Task.Run(() =>
                    {
                        VariableInfo<SECSCustomVarDto>.SetVariable(105, lotID, false);
                        VariableInfo<SECSCustomVarDto>.SetVariable(102, carrierID, false);
                        VariableInfo<SECSCustomVarDto>.SetVariable(103, loadportID, false);
                        mGEMPro.EventReport().SendEventReportByCEID(102);
                    });
                }
                else
                {
                    throw new Exception("SecsGem not connect");
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }
        }

        public static async Task SendLoadPortReadyToUnload(string lotID, string carrierID, string loadportID)
        {
            try
            {
                if (mGEMPro.SECSProControl.IsPortOpened)
                {
                    await Task.Run(() =>
                    {
                        VariableInfo<SECSCustomVarDto>.SetVariable(105, lotID, false);
                        VariableInfo<SECSCustomVarDto>.SetVariable(102, carrierID, false);
                        VariableInfo<SECSCustomVarDto>.SetVariable(103, loadportID, false);
                        mGEMPro.EventReport().SendEventReportByCEID(103);
                    });
                }
                else
                {
                    throw new Exception("SecsGem not connect");
                }
            }
            catch (Exception ex)
            {
                LogHelper.SecsGem(5, ex.Message);
            }
        }
        #endregion

    }
}
