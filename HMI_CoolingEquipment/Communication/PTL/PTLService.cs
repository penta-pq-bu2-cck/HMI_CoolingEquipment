using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Models;
using HMI_CoolingEquipment.ViewModels;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static HMI_CoolingEquipment.ViewModels.LoadPort_VM;

namespace HMI_CoolingEquipment.Communication
{
    public class PTLService : ViewModelBase
	{

		private Thread GatewayStatusCheckingThread = null;
		private bool PermissionRunning = false;

		private PTLInfo _pTLInfo = new PTLInfo();
		public PTLInfo PTLInfo
		{
			get { return _pTLInfo; }
			set
			{
				if(value != _pTLInfo)
				{
					_pTLInfo = value;
					RaisePropertyChanged(nameof(PTLInfo));
				}
			}
		}

		private List<PTLConnectionInfo> connectionInfos = new List<PTLConnectionInfo>();
		public List<PTLConnectionInfo> ConnectionInfos
		{
			get { return connectionInfos; }
			set
			{
				if(connectionInfos != value)
				{
					connectionInfos = value;
					RaisePropertyChanged(nameof(ConnectionInfos));
				}
			}        
		}

		private List<LoadPortModel> _loadPortModels = new List<LoadPortModel>();
		public List<LoadPortModel> LoadPortModels
		{
			get { return _loadPortModels; }
			set
			{
				if (_loadPortModels != value)
				{
					_loadPortModels = value;
					RaisePropertyChanged(nameof(LoadPortModels));
				}
			}
		}
		
		private List<LoadPortConfigurationModel> _loadPortConfig = new List<LoadPortConfigurationModel>();
		public List<LoadPortConfigurationModel> LoadPortConfig
		{
			get { return _loadPortConfig; }
			set
			{
				if (_loadPortConfig != value)
				{
					_loadPortConfig = value;
					RaisePropertyChanged(nameof(LoadPortConfig));
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

		public PTLService()
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");

				Messenger.Default.Register<List<LoadPortModel>>(this, UpdateLoadPortModels);
				Messenger.Default.Register<List<LoadPortConfigurationModel>>(this, UpdateLoadPortConfig);
				Messenger.Default.Register<UserAccountModel>(this, UpdateUserAccount);
                LoadPort_VM.OnUpdateLoadPortModelData += UpdateLoadPortModels;
                EF_Service.EF_UpdateMemory(HMIDBMemory.LOADPORT, "PTLService Constructor");
				EF_Service.EF_UpdateMemory(HMIDBMemory.LOADPORTCONFIGURATION, "PTLService Constructor");
				OpenPTL();
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

        private void UpdateLoadPortModels(LoadPortModel loadportmodel)
        {
            LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (!LoadPortModels.Find(y => y.LoadPortID.Equals(loadportmodel.LoadPortID)).LoadPortState.Equals(loadportmodel.LoadPortState))
                {
                    int gateway = ConnectionInfos.Find(y => y.GatewayIP.Equals(loadportmodel.LoadPortIPAddress)).GatewayID;
                    SetLedState(gateway, loadportmodel.LoadPortNodeAddress.Value, loadportmodel.LoadPortState, loadportmodel.LoadPortError);
                }

                if (LoadPortModels.Any(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)))
                {
                    LoadPortModels.Find(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)).PreAssignLoad = loadportmodel.PreAssignLoad;
                    LoadPortModels.Find(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)).LoadPortCarrierLoad = loadportmodel.LoadPortCarrierLoad;
                    LoadPortModels.Find(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)).LoadPortState = loadportmodel.LoadPortState;
                    LoadPortModels.Find(x => x.LoadPortID.Equals(loadportmodel.LoadPortID)).LoadPortError = loadportmodel.LoadPortError;
                }
            }));
            LogHelper.PTL(3, $"End Of Process : [{MethodBase.GetCurrentMethod().Name}]");
        }

        private void UpdateUserAccount(UserAccountModel model)
		{
			try
			{
				Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					UserAccount = model;
				}));
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
		}

		private void UpdateLoadPortModels(List<LoadPortModel> loadportmodels)
		{
			try
			{
                LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
                Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					if(LoadPortModels.Count <= 0)
					{
						LoadPortModels = loadportmodels;
					}

					if (loadportmodels.Any(x => x.LoadPortNodeConnectionStatus))
					{
						loadportmodels.Where(x => x.LoadPortNodeConnectionStatus).ToList().ForEach(x =>
						{
							if(!LoadPortModels.Find(y => y.LoadPortID.Equals(x.LoadPortID)).LoadPortState.Equals(x.LoadPortState))
							{
								int gateway = ConnectionInfos.Find(y => y.GatewayIP.Equals(x.LoadPortIPAddress)).GatewayID;
								SetLedState(gateway, x.LoadPortNodeAddress.Value, x.LoadPortState, x.LoadPortError);
							}
						});
					}

					LoadPortModels = loadportmodels;

					//if (LoadPortModels.Any(x => x.LoadPortNodeConnectionStatus))
					//{
					//    LoadPortModels.Where(x => x.LoadPortNodeConnectionStatus).ToList().ForEach(x =>
					//    {
					//        int gateway = ConnectionInfos.Find(y => y.GatewayIP.Equals(x.LoadPortIPAddress)).GatewayID;
					//        SetLedState(gateway, x.LoadPortNodeAddress.Value, x.LoadPortState, x.LoadPortError);
					//    }); 
					//}
				}));
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
                LogHelper.PTL(3, $"End Of Process : [{MethodBase.GetCurrentMethod().Name}]");
            }
		}

		private void UpdateLoadPortConfig(List<LoadPortConfigurationModel> loadportConfigModel)
		{
			try
			{
                LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
                Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					if(LoadPortConfig.Count <= 0)
					{
						LoadPortConfig = loadportConfigModel;
					}
					LoadPortConfig = loadportConfigModel;
					loadportConfigModel.ForEach(x =>
					{
						LoadPortModels.Where(y => y.LoadPortNodeConnectionStatus).ToList().Where(y => y.LoadPortState.Equals(x.LoadPortStatusState)).ToList().ForEach(x =>
						{
							int gateway = ConnectionInfos.Find(y => y.GatewayIP.Equals(x.LoadPortIPAddress)).GatewayID;
							SetLedState(gateway, x.LoadPortNodeAddress.Value, x.LoadPortState, x.LoadPortError);
						});
					});
				}));
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
                LogHelper.PTL(3, $"End Of Process : [{MethodBase.GetCurrentMethod().Name}]");
            }
		}

		public void ConnectToPTL(int gatewayID = 0)
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
				LogHelper.PTL(3, $"GatewayID : [{gatewayID}]");

				if (!gatewayID.Equals(0))
				{
					PTLInfo.InitClearTags = false;
				}

				OpenGateway(gatewayID);

				int num = 0;

				while(ReceiveMessageFromGateway() > 0)
				{
					num += 1;
				}

				LogHelper.PTL(3, $"Message Cleared From Gateway : [{num}]");

				if (GatewayStatusCheckingThread == null)
				{
					PermissionRunning = true;
					GatewayStatusCheckingThread = new Thread(GatewayStatusChecking);
					GatewayStatusCheckingThread.Name = nameof(GatewayStatusCheckingThread);
					GatewayStatusCheckingThread.IsBackground = true;
					GatewayStatusCheckingThread.Start();
				}

			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		public void DisconnectFromPTL(int gatewayID = 0)
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
				LogHelper.PTL(3, $"GatewayID : [{gatewayID}]");

				CloseGateway(gatewayID);

				if(!ConnectionInfos.Any(x => x.GatewayConnectionCode.Equals(7)))
				{
					PermissionRunning = false;

					if (GatewayStatusCheckingThread != null)
					{
						GatewayStatusCheckingThread.Join();
						GatewayStatusCheckingThread.Abort();
						GatewayStatusCheckingThread = null;
					}

					LoadPortModels.ForEach(x => x.LoadPortNodeConnectionStatus = false);
					LoadPortModels.Where(x => x.LoadPortState.Equals(LoadPortState.SensorTesting)).ToList().ForEach(x => x.LoadPortState = LoadPortState.Empty);
				}
				else
				{
					string ipAddress = ConnectionInfos.Find(x => x.GatewayID.Equals(gatewayID)).GatewayIP;
					LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(ipAddress)).ToList().ForEach(x => x.LoadPortNodeConnectionStatus = false);
				}

				//EF_Service.EF_UpdateLoadPort(GlobalClassFunction.LoadPortMap(LoadPortModels));
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		public void ClosePTL()
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");

				if (PTLInfo.m_GatewayConfig.ToList().Count > 0)
				{
					DisconnectFromPTL(); 
				}

				PTLUtils.AB_API_Close();
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		private async void OpenPTL()
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");

				int ret = PTLUtils.AB_API_Open();

				LogHelper.PTL(3, $"({nameof(OpenPTL)}) Initialize [{PTLUtils.DecodeMsg(ret)}]");

				int id = 0;
				byte[] ip = new byte[21];
				int port = 0;

				PTLInfo.GatewayCount = PTLUtils.AB_GW_Cnt();
				for (int i = 0; i <= PTLInfo.GatewayCount - 1; i++)
				{
					ret = PTLUtils.AB_GW_Conf(i, ref id, ref ip[0], ref port);

					string tmpstr = string.Empty;
					for (int j = 0; j <= 20; j++)
					{
						if (ip[j] == 0)
						{
							break;
						}
						tmpstr = tmpstr + Convert.ToChar(ip[j]);
					}

					ConnectionInfos.Add(new PTLConnectionInfo
					{
						GatewayID = id,
						GatewayIP = tmpstr,
						GatewayPort = port,
						GatewayConnectionCode = ret,
						GatewayConnectionValue = PTLUtils.DecodeMsg(ret),
					});
				}

				InitializeSettingsPTL();

				Messenger.Default.Send(ConnectionInfos);

				if (ConfigurationManager.AppSettings["EnableAutoConnectPTL"].Equals("1"))
				{
					await Task.Delay(1000);
					ConnectToPTL();
				}

			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		private void OpenGateway(int gatewayID = 0)
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");

				int ret = 0;

				if (gatewayID.Equals(0))
				{
					for (int i = 1; i <= PTLInfo.GatewayCount; i++)
					{
						ret = PTLUtils.AB_GW_Open(i);

						if (ret < 0)
						{
							LogHelper.PTL(5, $"({nameof(OpenGateway)}) Gateway {i}: {PTLInfo.m_GatewayConfig[i - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");
						}
						else
						{
							LogHelper.PTL(3, $"({nameof(OpenGateway)}) Gateway {i}: {PTLInfo.m_GatewayConfig[i - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");
						}
					}

				}
				else
				{
					ret = PTLUtils.AB_GW_Open(gatewayID);

					if (ret < 0)
					{
						LogHelper.PTL(5, $"({nameof(OpenGateway)}) Gateway {gatewayID}: {PTLInfo.m_GatewayConfig[gatewayID - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");
					}
					else
					{
						LogHelper.PTL(3, $"Gateway {gatewayID}: {PTLInfo.m_GatewayConfig[gatewayID - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		private void CloseGateway(int gatewayID = 0, int nodeAddress = 0)
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
				LogHelper.PTL(3, $"GatewayID : [{gatewayID}], NodeAddress : [{nodeAddress}]");

				if (gatewayID.Equals(0) && nodeAddress.Equals(0))
				{
					for (int nGw = 1; nGw <= PTLInfo.GatewayCount; nGw++)
					{
						int ret = PTLUtils.AB_GW_Status(nGw);

						if (ret > 0)
						{
							LogHelper.PTL(3, $"({nameof(CloseGateway)}) Gateway {nGw}: {PTLInfo.m_GatewayConfig[nGw - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");

							ret = PTLUtils.AB_LB_DspStr(nGw, -252, "", 0, -3); //turn off all tags on port1
							ret = PTLUtils.AB_LB_DspStr(nGw, 252, "", 0, -3); //turn off all tags on port2
							ret = PTLUtils.AB_GW_Close(nGw); //Close the specified Gateway
						}
						else
						{
							LogHelper.PTL(5, $"({nameof(CloseGateway)}) Gateway {nGw}: {PTLInfo.m_GatewayConfig[nGw - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");
						}
					}
				}
				else if (nodeAddress.Equals(0))
				{
					int ret = PTLUtils.AB_GW_Status(gatewayID);

					if (ret > 0)
					{

						LogHelper.PTL(3, $"({nameof(CloseGateway)}) Gateway {gatewayID}: {PTLInfo.m_GatewayConfig[gatewayID - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");

						ret = PTLUtils.AB_LB_DspStr(gatewayID, -252, "", 0, -3); //turn off all tags on port1
						ret = PTLUtils.AB_LB_DspStr(gatewayID, 252, "", 0, -3); //turn off all tags on port2
						ret = PTLUtils.AB_GW_Close(gatewayID); //Close the specified Gateway
					}
					else
					{
						LogHelper.PTL(5, $"({nameof(CloseGateway)}) Gateway {gatewayID}: {PTLInfo.m_GatewayConfig[gatewayID - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");
					}
				}
				else
				{
					int ret = PTLUtils.AB_GW_Status(gatewayID);

					if (ret > 0)
					{

						LogHelper.PTL(3, $"({nameof(CloseGateway)}) Gateway {gatewayID}: {PTLInfo.m_GatewayConfig[gatewayID - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");

						ret = PTLUtils.AB_LB_DspStr(gatewayID, 0-nodeAddress, "", 0, -3); //turn off all tags on port1
						ret = PTLUtils.AB_LB_DspStr(gatewayID, nodeAddress, "", 0, -3); //turn off all tags on port2
						ret = PTLUtils.AB_GW_Close(gatewayID); //Close the specified Gateway
					}
					else
					{
						LogHelper.PTL(5, $"({nameof(CloseGateway)}) Gateway {gatewayID}: {PTLInfo.m_GatewayConfig[gatewayID - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");
					}
				}

				GetGatewayConnectionStatus();
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		private void AB_LoadConf()
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");

				int id = 0;
				byte[] ip = new byte[21];
				int port = 0;

				PTLInfo.GatewayCount = PTLUtils.AB_GW_Cnt();
				LogHelper.PTL(3, $"({nameof(AB_LoadConf)}) GatewayCount: [{PTLInfo.GatewayCount}]");

				if (PTLInfo.GatewayCount == 0)
				{
					throw new Exception("Gateway Empty. Please check IPINDEX file");
				}

				PTLInfo.m_GatewayConfig = new PTLInfo.m_GatewayConfigType[PTLInfo.GatewayCount];

				for (int i = 0; i <= PTLInfo.GatewayCount - 1; i++)
				{
					int ret = PTLUtils.AB_GW_Conf(i, ref id, ref ip[0], ref port);
					if (ret >= 0)
					{
						PTLInfo.m_GatewayConfig[i].gateway_id = id;
						PTLInfo.m_GatewayConfig[i].port = port;
						string tmpstr = string.Empty;
						for (int j = 0; j <= 20; j++)
						{
							if (ip[j] == 0)
							{
								break;
							}
							tmpstr = tmpstr + Convert.ToChar(ip[j]);
						}
						PTLInfo.m_GatewayConfig[i].ip = tmpstr;

						LogHelper.PTL(3, $"({nameof(AB_LoadConf)}) Gateway {id}: {tmpstr} [{port}]");
					}
					else
					{
						PTLInfo.m_GatewayConfig[i].gateway_id = 0;
						PTLInfo.m_GatewayConfig[i].port = 0;
						PTLInfo.m_GatewayConfig[i].ip = "";
						LogHelper.PTL(5, $"({nameof(AB_LoadConf)}) Gateway {i} = [{PTLUtils.DecodeMsg(ret)}]");
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}
		
		private void InitializeSettingsPTL()
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");

				AB_LoadConf();
				PTLInfo.DiagnoseGatewayTimeOut = new int[PTLInfo.GatewayCount + 1];
				PTLInfo.DiagnoseGatewayOk = new bool[PTLInfo.GatewayCount + 1];
				PTLInfo.AutoScan = true;
				PTLInfo.ScanTimer = 10;
				PTLInfo.InitClearTags = false;
				PTLInfo.SetBestPoll = false;
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		private void InitClearTags(int nGw)
		{
			int ret;
			try
			{
				if (!PTLInfo.InitClearTags)
				{
					LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
					LogHelper.PTL(3, $"GatewayID : [{nGw}]");

					ret = PTLUtils.AB_LB_DspNum(nGw, -252, 0, 0, -3);

					if (ret < 0)
					{
						throw new Exception($"({nameof(InitClearTags)}) Gateway {nGw}: {PTLInfo.m_GatewayConfig[nGw - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");
					}
					else
					{
						LogHelper.PTL(3, $"({nameof(InitClearTags)}) Gateway {nGw}: {PTLInfo.m_GatewayConfig[nGw - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");
						PTLInfo.InitClearTags = true;

						GetTagConnectionStatus();
					}

					LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
				}
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
		}

		#region GatewayMessage

		private async void GatewayStatusChecking()
		{
			try
			{

                EF_Service.EF_UpdateMemory(HMIDBMemory.ALL, nameof(GatewayStatusChecking));
                while (PermissionRunning)
				{
					for (int nGw = 1; nGw <= PTLInfo.GatewayCount; nGw++)
					{
						int ret = PTLUtils.AB_GW_Status(nGw); //Get the status of current Gateway or specified Gateway
						if (PTLInfo.AutoScan) //auto scan => auto feedback gateway connect status (but step by step receive will not,so it wiil not use below method)
						{
							if (!PTLInfo.SetBestPoll) //bother setting best polling(auto scan too fast)
							{
								if (ret == 7) //gateway response fastly
								{
									InitClearTags(nGw);
								}

								GetGatewayConnectionStatus(nGw);

								//if (!PTLInfo.m_GatewayConfig[nGw - 1].statusCode.Equals(ret))
								//{
								//    PTLInfo.m_GatewayConfig[nGw - 1].statusCode = ret;
								//    LogHelper.PTL(3, $"({nameof(GatewayStatusChecking)}) Gateway {nGw}: {PTLInfo.m_GatewayConfig[nGw - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");

								//    ConnectionInfos.Find(x => x.GatewayIP.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip)).GatewayConnectionCode = ret;
								//    ConnectionInfos.Find(x => x.GatewayIP.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip)).GatewayConnectionValue = PTLUtils.DecodeMsg(ret);

								//    Messenger.Default.Send(ConnectionInfos);
									

								//    if (PTLInfo.m_GatewayConfig[nGw - 1].gateway_id.Equals(1))
								//    {
								//        if(!PTLInfo.m_GatewayConfig[nGw - 1].statusCode.Equals(7))
								//        {
								//            AlarmService.DefineAlarm(1071); //Warning
								//        }
								//    }
								//    else if(PTLInfo.m_GatewayConfig[nGw - 1].gateway_id.Equals(2))
								//    {
								//        if (!PTLInfo.m_GatewayConfig[nGw - 1].statusCode.Equals(7))
								//        {
								//            AlarmService.DefineAlarm(1072); //Warning
								//        }
								//    }
								//}
							}
						}
						else
						{
							if (!PTLInfo.m_GatewayConfig[nGw - 1].statusCode.Equals(ret))
							{
								PTLInfo.m_GatewayConfig[nGw - 1].statusCode = ret;
								LogHelper.PTL(3, $"({nameof(GatewayStatusChecking)}) Gateway {nGw}: {PTLInfo.m_GatewayConfig[nGw - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");

								ConnectionInfos.Find(x => x.GatewayIP.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip)).GatewayConnectionCode = ret;
								ConnectionInfos.Find(x => x.GatewayIP.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip)).GatewayConnectionValue = PTLUtils.DecodeMsg(ret);

								Messenger.Default.Send(ConnectionInfos);

								if (PTLInfo.m_GatewayConfig[nGw - 1].gateway_id.Equals(1))
								{
									if (!PTLInfo.m_GatewayConfig[nGw - 1].statusCode.Equals(7))
									{
										AlarmService.DefineAlarm(1071); //Warning

										if (LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip)).Any(x => x.LoadPortNodeConnectionStatus))
										{
											LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip) && x.LoadPortNodeConnectionStatus).ToList().ForEach(x =>
											{
												x.LoadPortNodeConnectionStatus = false;
												LogHelper.PTL(3, $"({nameof(GatewayStatusChecking)}) Gateway {PTLInfo.m_GatewayConfig[nGw - 1].gateway_id}: LoadPort {x.LoadPortID} connection set to FALSE in memory");
											});
											EF_Service.EF_UpdateLoadPort(GlobalClassFunction.LoadPortMap(LoadPortModels));
										}
									}
									else
									{
										//AlarmService.ClearAlarm(1071); //Warning

										//if (LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip)).Any(x => x.LoadPortNodeConnectionStatus.Equals(false)))
										//{
          //                                  await Task.Delay(8000);
          //                                  GetIOValue(nGw);
										//}
									}
                                }
								else if (PTLInfo.m_GatewayConfig[nGw - 1].gateway_id.Equals(2))
								{
                                    if (!PTLInfo.m_GatewayConfig[nGw - 1].statusCode.Equals(7))
                                    {
                                        AlarmService.DefineAlarm(1072); //Warning

										if (LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip)).Any(x => x.LoadPortNodeConnectionStatus))
										{
											LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip) && x.LoadPortNodeConnectionStatus).ToList().ForEach(x =>
											{
												x.LoadPortNodeConnectionStatus = false;
												LogHelper.PTL(3, $"({nameof(GatewayStatusChecking)}) Gateway {PTLInfo.m_GatewayConfig[nGw - 1].gateway_id}: LoadPort {x.LoadPortID} connections set to FALSE in memory");
											});
											EF_Service.EF_UpdateLoadPort(GlobalClassFunction.LoadPortMap(LoadPortModels));
										}
                                    }
                                    else
                                    {
           //                             AlarmService.ClearAlarm(1072); //Warning

           //                             if (LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip)).Any(x => x.LoadPortNodeConnectionStatus.Equals(false)))
           //                             {
											//await Task.Delay(8000);
           //                                 GetIOValue(nGw);
           //                             }
                                    }
                                }
							}
						}
					}

					if (PTLInfo.AutoScan)
					{
						for (int nGw = 1; nGw <= PTLInfo.GatewayCount; nGw++)
						{
							if (PTLUtils.AB_GW_Status(nGw) == 7)
							{
								ReceiveMessageFromGateway();
							}
						}
					}
					else
					{
						ReceiveMessageFromGateway();
					}

                    await Task.Delay(PTLInfo.ScanTimer);
				}
			}
			catch (ThreadAbortException ex)
			{
				LogHelper.PTL(5, ex.Message);
			}

		}

        private int ReceiveMessageFromGateway()
		{
			////ret>=0:ok,else:err
			int ret = -1;

			try
			{
				byte[] ccbdata = new byte[255];
				int ccblen = 0;
				int nGwId = 0;
				int nNode = 0;
				short TagCommand = -1;
				short nMsg_type = -1;
				string m_RcvTagData = string.Empty;

				//================================='
				//By API COMMAND
				ret = PTLUtils.AB_Tag_RcvMsg(ref nGwId, ref nNode, ref TagCommand, ref nMsg_type, ref ccbdata[0], ref ccblen);

				if (ret > 0)
				{
					int m_GwPort;
					int m_GwID = nGwId;
					int m_NodeAddress = nNode;
					short m_TagCommand = TagCommand;
					string m_SubCommandMess = string.Empty;

					if (m_NodeAddress < 0)
						m_GwPort = 1;
					else
						m_GwPort = 2;
					m_NodeAddress = Math.Abs(m_NodeAddress);

					for (int x = 0; x < ccblen; x++)
					{
						m_RcvTagData += Convert.ToChar(ccbdata[x]);
					}

					LogHelper.PTL(3, $"[Gateway:{m_GwID}, Port:{m_GwPort}, NodeAddress:{m_NodeAddress}, TagCommand:{m_TagCommand}, SubCommand:{m_SubCommandMess}] = {m_RcvTagData}");

					switch (m_TagCommand)
					{
						case 6:
							break;
						case 7:
							break;
						case 9:
							string tmpstr = string.Empty;
							byte tmp = 0;
							int maxNode = 0;
							//Get Max Node
							for (int k = 1; k <= 250; k++)
							{
								if (((k - 1) % 8) == 0)
								{
									tmp = ccbdata[3 + ((k - 1) / 8)];
								}
								if ((tmp % 2) == 1)
								{
									tmpstr = tmpstr + "0";
								}
								else
								{
									tmpstr = tmpstr + "1";
									maxNode = k;
								}
								tmp = (byte)(tmp / 2);
							}

							//set best polling range
							if (GlobalAttributesClass.SetBestPollRangeBool.ContainsKey(m_GwID))
							{
								if (GlobalAttributesClass.SetBestPollRangeBool[m_GwID])
								{
									ret = PTLUtils.AB_GW_SetPollRang(m_GwID, 0 - maxNode);
									GlobalAttributesClass.SetBestPollRangeBool[m_GwID] = false;
								}
							}

							if (GlobalAttributesClass.DetectTagStatus.ContainsKey(m_GwID))
							{
								if (GlobalAttributesClass.DetectTagStatus[m_GwID])
								{
									for (int k = 1; k <= maxNode; k++)
									{
										bool connectIsGood = false;

										if (tmpstr.Substring(k - 1, 1).Equals("1"))
										{
											connectIsGood = true;
										}
										else
										{
											//int errorCode = LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(k)).Equals("Tray") ? 1100 + k : 1500 + k;
											int errorCode = (LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(k)).LoadPortRack.Equals("AA") ||
														     LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(k)).LoadPortRack.Equals("AB") ||
														     LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(k)).LoadPortRack.Equals("AC"))
														     ? 1100 + k : 1500 + k;
											AlarmService.DefineAlarm(errorCode); //Warning
										}

										LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(k)).LoadPortNodeConnectionStatus = connectIsGood;
									}

									LogHelper.General(3, $"{nameof(ReceiveMessageFromGateway)}| TagCmd: {m_TagCommand} - ResetLEDState");
									ResetLedState();
									UpdateLoadPortDataModel();
								}
							}
							break;
						case 10:
							if (LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(m_NodeAddress)).LoadPortNodeConnectionStatus)
							{
								LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(m_NodeAddress)).LoadPortNodeConnectionStatus = false;
								//int errorCode = LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(m_NodeAddress)).LoadPortType.Equals("Tray") ? 1100 + m_NodeAddress : 1500 + m_NodeAddress;
								int errorCode = (LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(m_NodeAddress)).LoadPortRack.Equals("AA") ||
												 LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(m_NodeAddress)).LoadPortRack.Equals("AB") ||
												 LoadPortModels.Find(x => x.LoadPortIPAddress.Equals(ConnectionInfos.Find(y => y.GatewayID.Equals(m_GwID)).GatewayIP) && x.LoadPortNodeAddress.Equals(m_NodeAddress)).LoadPortRack.Equals("AC"))
												 ? 1100 + m_NodeAddress : 1500 + m_NodeAddress;
								AlarmService.DefineAlarm(errorCode); //Warning
							}
							break;
						case 12:
							break;
						case 13:
							break;
						case 15:
							break;
						case 36:
							break;
						case 60: //IO trigger
							bool isInit = false;
							bool IsLoaded = false;

                            if (ccblen.Equals(5))
							{
								isInit = true;
                                if (m_RcvTagData.Substring(0, 1).Equals("0"))
								{
                                    if (m_RcvTagData.IndexOf('1') > 0)
									{
										IsLoaded = true;
									}

                                    LoadPortDataChange(isInit, m_GwID, m_NodeAddress, IsLoaded);
                                }
							}
							else
							{
                                if (m_RcvTagData.Substring(0, 1).Equals("4"))
                                {
                                    m_RcvTagData = m_RcvTagData.Substring(1, 4);

                                    if (m_RcvTagData.IndexOf('1') > 0)
                                    {
                                        IsLoaded = true;
                                    }

                                    LoadPortDataChange(isInit, m_GwID, m_NodeAddress, IsLoaded);
                                }
                            }
							break;
						case 100:
							break;
						case 252:
							break;
						default:
							break;
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}

			return ret;
		}

		#endregion

		public void ResetError()
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");

				LoadPortModels.ToList().ForEach(x =>
				{
					if (x.LoadPortError)
					{
						//Messenger.Default.Send(new LogValue
						//{
						//	Value = $"[CLEARED]-{x.LoadPortID}",
						//	Page = HMIPage.LoadingPage
						//});

						//Messenger.Default.Send(new LogValue
						//{
						//	Value = $"[CLEARED]-{x.LoadPortID}",
						//	Page = HMIPage.UnloadingPage
						//});


						//LoadPort_VM.LoadPortModelData(x);

						x.LoadPortError = false;
                        LoadPort_VM.LoadPortModelData(x);

                        int gatewayID = ConnectionInfos.Find(y => y.GatewayIP.Equals(x.LoadPortIPAddress)).GatewayID;
						//SetLedState(gatewayID, x.LoadPortNodeAddress.Value, LoadPortState.Empty);
						GetIOValue(gatewayID, x.LoadPortNodeAddress.Value);
					}
					else if(!x.LoadPortNodeConnectionStatus && x.LoadPortIsEnable)
					{
                        int gatewayID = ConnectionInfos.Find(y => y.GatewayIP.Equals(x.LoadPortIPAddress)).GatewayID;
                        GetIOValue(gatewayID, x.LoadPortNodeAddress.Value);
                    }
				});

				//UpdateLoadPortDataModel();
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		#region ConnectionStatus

		public async void GetTagConnectionStatus(int gatewayID = 0)
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
				LogHelper.PTL(3, $"GatewayID : [{gatewayID}]");

				if (gatewayID.Equals(0))
				{
					ConnectionInfos.ForEach(async x =>
					{
						if (GlobalAttributesClass.DetectTagStatus.ContainsKey(x.GatewayID))
						{
							GlobalAttributesClass.DetectTagStatus[x.GatewayID] = true;
						}
						else
						{
							GlobalAttributesClass.DetectTagStatus.Add(x.GatewayID, true);
						}

						PTLUtils.AB_GW_TagDiag(x.GatewayID, 1);

						await Task.Delay(8000);
					});
				}
				else
				{
					if (GlobalAttributesClass.DetectTagStatus.ContainsKey(gatewayID))
					{
						GlobalAttributesClass.DetectTagStatus[gatewayID] = true;
					}
					else
					{
						GlobalAttributesClass.DetectTagStatus.Add(gatewayID, true);
					}

					PTLUtils.AB_GW_TagDiag(gatewayID, 1);

					await Task.Delay(8000);
				}
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		public async void GetGatewayConnectionStatus(int gatewayID = 0)
		{
			try
			{
				if (gatewayID.Equals(0))
				{
					for (int nGw = 1; nGw <= PTLInfo.GatewayCount; nGw++)
					{
						LogHelper.PTL(3, $"Start to : [GetGatewayConnectionStatus]");
						LogHelper.PTL(3, $"GatewayID : [{gatewayID}]");

						int ret = PTLUtils.AB_GW_Status(nGw); //Get the status of current Gateway or specified Gateway
						
						if (!PTLInfo.m_GatewayConfig[nGw - 1].statusCode.Equals(ret))
						{
							PTLInfo.m_GatewayConfig[nGw - 1].statusCode = ret;
							LogHelper.PTL(3, $"({nameof(GatewayStatusChecking)}) Gateway {nGw}: {PTLInfo.m_GatewayConfig[nGw - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");

							ConnectionInfos.Find(x => x.GatewayIP.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip)).GatewayConnectionCode = ret;
							ConnectionInfos.Find(x => x.GatewayIP.Equals(PTLInfo.m_GatewayConfig[nGw - 1].ip)).GatewayConnectionValue = PTLUtils.DecodeMsg(ret);

							Messenger.Default.Send(ConnectionInfos);
						}
					}
				}
				else
				{
					int ret = PTLUtils.AB_GW_Status(gatewayID); //Get the status of current Gateway or specified Gateway
					
					if (!PTLInfo.m_GatewayConfig[gatewayID - 1].statusCode.Equals(ret))
					{
						PTLInfo.m_GatewayConfig[gatewayID - 1].statusCode = ret;
						LogHelper.PTL(3, $"({nameof(GatewayStatusChecking)}) Gateway {gatewayID}: {PTLInfo.m_GatewayConfig[gatewayID - 1].ip} [{PTLUtils.DecodeMsg(ret)}]");

						ConnectionInfos.Find(x => x.GatewayIP.Equals(PTLInfo.m_GatewayConfig[gatewayID - 1].ip)).GatewayConnectionCode = ret;
						ConnectionInfos.Find(x => x.GatewayIP.Equals(PTLInfo.m_GatewayConfig[gatewayID - 1].ip)).GatewayConnectionValue = PTLUtils.DecodeMsg(ret);

						Messenger.Default.Send(ConnectionInfos);

						if (PTLInfo.m_GatewayConfig[gatewayID - 1].gateway_id.Equals(1))
						{
							if (!PTLInfo.m_GatewayConfig[gatewayID - 1].statusCode.Equals(7))
							{
								AlarmService.DefineAlarm(1071); //Warning

                                if (LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[gatewayID - 1].ip)).Any(x => x.LoadPortNodeConnectionStatus))
                                {
                                    LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[gatewayID - 1].ip) && x.LoadPortNodeConnectionStatus).ToList().ForEach(x =>
                                    {
                                        x.LoadPortNodeConnectionStatus = false;
                                        LogHelper.PTL(3, $"({nameof(GatewayStatusChecking)}) Gateway ID: {gatewayID}: LoadPort : {x.LoadPortID} set FALSE in memory");
                                    });
									try
									{
										EF_Service.EF_UpdateLoadPort(GlobalClassFunction.LoadPortMap(LoadPortModels));
									} 
									catch(Exception ex)
									{
                                        LogHelper.PTL(5, $"DB update failed for Gateway {gatewayID}: {ex.Message}");
                                    }
                                }
							}
							else
							{
                                AlarmService.ClearAlarm(1071); //Warning

                                if (LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[gatewayID - 1].ip)).Any(x => x.LoadPortNodeConnectionStatus.Equals(false)))
                                {
                                    await Task.Delay(8000);
                                    GetIOValue(gatewayID);
                                }
                            }
						}
						else if (PTLInfo.m_GatewayConfig[gatewayID - 1].gateway_id.Equals(2))
						{
                            if (!PTLInfo.m_GatewayConfig[gatewayID - 1].statusCode.Equals(7))
                            {
                                AlarmService.DefineAlarm(1072); //Warning

                                if (LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[gatewayID - 1].ip)).Any(x => x.LoadPortNodeConnectionStatus))
                                {
                                    LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[gatewayID - 1].ip) && x.LoadPortNodeConnectionStatus).ToList().ForEach(x =>
                                    {
                                        x.LoadPortNodeConnectionStatus = false;
                                        LogHelper.PTL(3, $"({nameof(GatewayStatusChecking)}) Gateway ID: {gatewayID}: LoadPort: {x.LoadPortID} set FALSE in memory");
                                    });
									try
									{
										EF_Service.EF_UpdateLoadPort(GlobalClassFunction.LoadPortMap(LoadPortModels));
									}
									catch (Exception ex)
									{
                                        LogHelper.PTL(5, $"DB update failed for Gateway {gatewayID}: {ex.Message}");
                                    }
                                }
                            }
                            else
                            {
                                AlarmService.ClearAlarm(1072); //Warning

                                if (LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(PTLInfo.m_GatewayConfig[gatewayID - 1].ip)).Any(x => x.LoadPortNodeConnectionStatus.Equals(false)))
                                {
                                    await Task.Delay(8000);
                                    GetIOValue(gatewayID);
                                }
                            }
                        }
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				//LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		#endregion

		#region Setup

		public async void SetDeviceBestPollRange(int gatewayID = 0)
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
				LogHelper.PTL(3, $"GatewayID : [{gatewayID}]");

				if (gatewayID.Equals(0))
				{
					ConnectionInfos.ForEach(async x =>
					{
						if (GlobalAttributesClass.SetBestPollRangeBool.ContainsKey(x.GatewayID))
						{
							GlobalAttributesClass.SetBestPollRangeBool[x.GatewayID] = true;
						}
						else
						{
							GlobalAttributesClass.SetBestPollRangeBool.Add(x.GatewayID, true);
						}

						PTLUtils.AB_GW_SetPollRang(x.GatewayID, -250);

						await Task.Delay(8000);

						PTLUtils.AB_GW_TagDiag(x.GatewayID, 1);
					});
				}
				else
				{
					if (GlobalAttributesClass.SetBestPollRangeBool.ContainsKey(gatewayID))
					{
						GlobalAttributesClass.SetBestPollRangeBool[gatewayID] = true;
					}
					else
					{
						GlobalAttributesClass.SetBestPollRangeBool.Add(gatewayID, true);
					}

					PTLUtils.AB_GW_SetPollRang(gatewayID, -250);

					await Task.Delay(8000);

					PTLUtils.AB_GW_TagDiag(gatewayID, 1);
				}
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		public void GetIOValue(int gatewayID = 0, int nodeAddress = 0)
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
				LogHelper.PTL(3, $"GatewayID : [{gatewayID}], NodeAddress = [{nodeAddress}]");

				if (gatewayID.Equals(0) && nodeAddress.Equals(0))
				{
					LoadPortModels.ForEach(x =>
					{
                        int temp_gatewayID = ConnectionInfos.Find(y => y.GatewayIP.Equals(x.LoadPortIPAddress)).GatewayID;

                        int ret = PTLUtils.AB_DIO_ReadIoStatus(temp_gatewayID, int.Parse($"-{x.LoadPortNodeAddress.Value}"));
					});
				}
				else if (nodeAddress.Equals(0))
				{
					string address = ConnectionInfos.Find(y => y.GatewayID.Equals(gatewayID)).GatewayIP;

					LoadPortModels.Where(x => x.LoadPortIPAddress.Equals(address)).ToList().ForEach(x =>
					{
                        int ret = PTLUtils.AB_DIO_ReadIoStatus(gatewayID, int.Parse($"-{x.LoadPortNodeAddress.Value}"));
                    });
				}
				else
				{
					int ret = PTLUtils.AB_DIO_ReadIoStatus(gatewayID, int.Parse($"-{nodeAddress}"));
				}
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		#endregion

		#region UpdateDB

		public void UpdateLoadPortDataModel()
		{
			try
			{
				EF_Service.EF_UpdateLoadPort(GlobalClassFunction.LoadPortMap(LoadPortModels));
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
		}

		#endregion

		#region SetLedState

		public void SetLedState(int gatewayID, int nodeAddress, LoadPortState state, bool isError = false)
		{
			try
			{
				if (connectionInfos.Find(x => x.GatewayID.Equals(gatewayID)).GatewayConnectionCode.Equals(7))
				{
					LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
					LogHelper.PTL(3, $"GatewayID : [{gatewayID}], NodeAddress : [{nodeAddress}], PortState : [{state}]");

					byte Led_Colour = 0;
					byte Led_State = 0;

					if (!isError)
					{
						Led_Colour = (byte)LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(state)).LoadPortLEDColour;
						Led_State = (byte)LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(state)).LoadPortLEDState;
					}
					else
					{
						Led_Colour = (byte)LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Error)).LoadPortLEDColour;
						Led_State = (byte)LoadPortConfig.Find(x => x.LoadPortStatusState.Equals(LoadPortState.Error)).LoadPortLEDState;
					}

					PTLUtils.AB_LED_Status(gatewayID, nodeAddress, Led_Colour, Led_State); 
				}
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		public void LoadUnloadProcessSetLed(LoadPortModel model, LoadPortState state)
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
				LogHelper.PTL(3, $"IPAddress : [{model.LoadPortIPAddress}], NodeAddress : [{model.LoadPortNodeAddress}], PortState : [{state}]");

				int gatewayid = ConnectionInfos.Find(y => y.GatewayIP.Equals(model.LoadPortIPAddress)).GatewayID;

				LoadPortModel tmpModel = LoadPortModels.Find(x => x.LoadPortID.Equals(model.LoadPortID));
				tmpModel.LoadPortState = state;

				SetLedState(gatewayid, model.LoadPortNodeAddress.Value, state);

                LoadPort_VM.LoadPortModelData(tmpModel);
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		public void ResetLedState()
		{
			try
			{
                LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
                ConnectionInfos.ForEach(x =>
				{
					if (x.GatewayConnectionCode.Equals(7))
					{
						LoadPortModels.Where(y => y.LoadPortIPAddress.Equals(x.GatewayIP)).ToList().ForEach(y =>
						{
							SetLedState(x.GatewayID, y.LoadPortNodeAddress.Value, y.LoadPortState);
						});
					}
				});
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
                LogHelper.PTL(3, $"End Of Process : [{MethodBase.GetCurrentMethod().Name}]");
            }
		}

		#endregion

		#region Testing

		public async void EnableLightLEDTesting(int mode = 0)
		{
			LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
			LogHelper.PTL(3, $"Mode : [{mode}]");

			byte colourID = 0;
			int counting = 0;
			GlobalAttributesClass.LedTestingEnable = true;

			try
			{
				do
				{
					if (mode.Equals(0) || mode.Equals(1))
					{
						ConnectionInfos.OrderBy(x => x.GatewayID).ToList().ForEach(x =>
						{
							if (x.GatewayConnectionCode.Equals(7))
							{
								LoadPortModels.OrderBy(y => y.LoadPortNodeAddress).ToList().ForEach(async y =>
								{
									if (y.LoadPortIPAddress.Equals(x.GatewayIP) && y.LoadPortNodeConnectionStatus)
									{
										PTLUtils.AB_LED_Status(x.GatewayID, y.LoadPortNodeAddress.Value, colourID, 1);
										colourID += 1;

										if (colourID == 6)
										{
											colourID = 0;
										}

										await Task.Delay(1000);
									}
								});
							}
						});
					}

					if (mode.Equals(0) || mode.Equals(2))
					{
						byte ledState = 0;

						do
						{
							ConnectionInfos.OrderBy(x => x.GatewayID).ToList().ForEach(x =>
							{
								if (x.GatewayConnectionCode.Equals(7))
								{
									PTLUtils.AB_LED_Status(x.GatewayID, -252, ledState, 1);
								}
							});
							await Task.Delay(1000);
							ledState += 1;

						} while (ledState < 7);
					}

					counting += 1;

				} while (counting <= 3);

				ResetLedState();
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		public void DisableLightLEDTesting()
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
				GlobalAttributesClass.LedTestingEnable = false;
				ResetLedState();
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		public void EnableSensorTesting()
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");

				if (MessageBox.Show("Please empty all LoadPort before proceed with sensor testing", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning).Equals(MessageBoxResult.OK))
				{
					if (!LoadPortModels.Any(x => !x.LoadPortState.Equals(LoadPortState.Empty) && !x.LoadPortState.Equals(LoadPortState.Disabled)))
					{
						ConnectionInfos.ForEach(x =>
						{
							if (x.GatewayConnectionCode.Equals(7))
							{
								LoadPortModels.Where(y => y.LoadPortIPAddress.Equals(x.GatewayIP) && y.LoadPortIsEnable).ToList().ForEach(y =>
								{
									PTLUtils.AB_LED_Status(x.GatewayID, y.LoadPortNodeAddress.Value, 3, 1);
									y.LoadPortState = LoadPortState.SensorTesting;
								});
							}
						});

						UpdateLoadPortDataModel();
					}
					else
					{
						MessageBox.Show("Not all LoadPort is empty !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						throw new Exception("Not all LoadPort is empty !");
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

		public void DisableSensorTesting()
		{
			try
			{
				LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
				ConnectionInfos.ForEach(x =>
				{
					if (x.GatewayConnectionCode.Equals(7))
					{
						LoadPortModels.Where(y => y.LoadPortIPAddress.Equals(x.GatewayIP) && y.LoadPortIsEnable).ToList().ForEach(y =>
						{
							SetLedState(x.GatewayID, y.LoadPortNodeAddress.Value, LoadPortState.Empty);
							y.LoadPortState = LoadPortState.Empty;
						});
					}
				});

				UpdateLoadPortDataModel();
			}
			catch (Exception ex)
			{
				LogHelper.PTL(5, ex.Message);
			}
			finally
			{
				LogHelper.PTL(3, $"End of process : [{MethodBase.GetCurrentMethod().Name}]");
			}
		}

        #endregion

        private async void LoadPortDataChange(bool isInit, int GwID, int NodeAddress, bool IsLoaded)
		{
			await Task.Run(() => HandleLoadPortDataChange(isInit, GwID, NodeAddress, IsLoaded)).ConfigureAwait(false);
		}

        private void HandleLoadPortDataChange(bool isInit, int GwID, int NodeAddress, bool IsLoaded)
		{
			LoadPortModel model = FindLoadPortByIPAndAddress(ConnectionInfos.Find(y => y.GatewayID.Equals(GwID)).GatewayIP, NodeAddress);

            UpdateLoadPortConnection(ref model);

			LogHelper.General(3, $"[HandleLoadPortDataChange] - Init: [{isInit}] / GwID: [{GwID}] / NodeAddress: [{NodeAddress}] / Loaded: [{IsLoaded}] / LoadportEnable: [{model.LoadPortIsEnable}] / LoadportState: [{model.LoadPortState}]");
            if (model.LoadPortIsEnable)
			{
                switch (model.LoadPortState)
                {
                    case LoadPortState.LoadProcess:
                        UpdateLoadPortState_LoadProcess(ref model, GwID, IsLoaded);
                        break;
                    case LoadPortState.Loaded:
                        UpdateLoadPortState_Loaded(ref model, isInit, GwID, IsLoaded);
                        break;
                    case LoadPortState.Empty:
                        UpdateLoadPortState_Empty(ref model, isInit, GwID, IsLoaded);
                        break;
                    case LoadPortState.UnloadProcess:
                        UpdateLoadPortState_UnloadProcess(ref model, GwID, IsLoaded);
                        break;
                    case LoadPortState.CoolingCompleted:
                        UpdateLoadPortState_CoolingCompleted(ref model, isInit, GwID, IsLoaded);
                        break;
                    case LoadPortState.SensorTesting:
                        if (IsLoaded)
                        {
							PTLUtils.AB_LED_Status(GwID, NodeAddress, 1, 1);
						}
						else
						{
                            PTLUtils.AB_LED_Status(GwID, NodeAddress, 3, 1);
                        }
                        break;
					case LoadPortState.Abort:
						UpdateLoadPortState_AbortProcess(ref model, GwID, IsLoaded);
							break;
                }
			}

			LoadPort_VM.LoadPortModelData(model);
		}

		private void UpdateLoadPortConnection(ref LoadPortModel model)
		{
			if (!model.LoadPortNodeConnectionStatus)
			{
                model.LoadPortNodeConnectionStatus = true;
			}
		}

        private void UpdateLoadPortState_LoadProcess(ref LoadPortModel model, int GwID, bool IsLoaded)
        {
            try
            {
                if (IsLoaded)
                {
                    if (UserAccount != null)
                    {
                        // Update load port state to Loaded
                        model.LoadPortState = LoadPortState.Loaded;
                        SetLedState(GwID, model.LoadPortNodeAddress.Value, LoadPortState.Loaded);

                        // Move Pre-Assign Carrier to LoadPortCarrier
                        string preassign = model.PreAssignLoad;
                        string loadportID = model.LoadPortID;
                        model.PreAssignLoad = null;
                        model.LoadPortCarrierLoad = preassign;

                        LoadPort_VM.LoadPortModelData(model);

                        LogHelper.General(3, $"{UserAccount.UserName} loaded carrier {preassign} into {loadportID}");

                        // Update carrier status asynchronously
                        EF_Service.EF_UpdateCarrierStatusAsync(preassign, loadportID, JobStatus.Cooling, UserAccount.UserName);

						LoadingLogMessageFromPTL(new LogValue
						{
							Value = $"{preassign} Loaded Into {loadportID}",
							Page = HMIPage.LoadingPage
						});
                    }
                    else
                    {
                        // No user account, indicate unauthorized action
                        ErrorUnauthorizedAction(ref model, GwID, 1);
                    }
                }
                else
                {
                    // Load port is not loaded, clear any unauthorized action errors
                    ClearErrorUnauthorizedAction(ref model, GwID, 1);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                LogHelper.General(5, $"An error occurred during LoadProcess: {ex.Message}");
            }
        }

        private void UpdateLoadPortState_Loaded(ref LoadPortModel model, bool isInit, int GwID, bool IsLoaded)
        {
			if (IsLoaded)
			{
                ClearErrorUnauthorizedAction(ref model, GwID, 2);
            }
			else
			{
                if (isInit)
                {
                    ErrorUnauthorizedAction(ref model, GwID, 2);
                }
                else
                {
					if (LoadPortModels.Any(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess) || x.LoadPortState.Equals(LoadPortState.LoadProcess)))
					{
                        int address = model.LoadPortNodeAddress.Value;
                        Task.Run(() =>
                        {
                            DateTime dt_Now = DateTime.Now;

                            while (DateTime.Now - dt_Now <= TimeSpan.FromSeconds(1))
                            {
                                //Buffer to double check the error.
                            }

                            GetIOValue(GwID, address);
                        });
					}
					else
					{
                        ErrorUnauthorizedAction(ref model, GwID, 2);
                    }
                }
            }
        }

        private void UpdateLoadPortState_Empty(ref LoadPortModel model, bool isInit, int GwID, bool IsLoaded)
        {
			if (IsLoaded)
			{
                if (isInit)
                {
                    ErrorUnauthorizedAction(ref model, GwID, 1);
                }
                else
                {
                    if (LoadPortModels.Any(x => x.LoadPortState.Equals(LoadPortState.LoadProcess) || x.LoadPortState.Equals(LoadPortState.UnloadProcess)))
                    {
                        int address = model.LoadPortNodeAddress.Value;
                        Task.Run(() =>
                        {
                            DateTime dt_Now = DateTime.Now;

                            while (DateTime.Now - dt_Now <= TimeSpan.FromSeconds(1))
                            {
                                //Buffer to double check the error.
                            }

                            GetIOValue(GwID, address);
                        });
                    }
                    else
                    {
                        ErrorUnauthorizedAction(ref model, GwID, 1);
                    }
                }
            }
			else
			{
                ClearErrorUnauthorizedAction(ref model, GwID, 1);
            }
        }

        private void UpdateLoadPortState_UnloadProcess(ref LoadPortModel model, int GwID, bool IsLoaded)
        {
            try
            {
                if (IsLoaded)
                {
                    // Load port is loaded, clear any unauthorized action errors
                    ClearErrorUnauthorizedAction(ref model, GwID, 2);
                }
                else
                {
                    if (UserAccount != null)
                    {
                        // There's a user account, proceed with the unload process
                        string loadportID = model.LoadPortID;
                        string carrierID = model.LoadPortCarrierLoad;

                        // Update load port state to Empty
                        model.LoadPortState = LoadPortState.Empty;
                        SetLedState(GwID, model.LoadPortNodeAddress.Value, LoadPortState.Empty);
                        model.LoadPortCarrierLoad = null;

                        LoadPort_VM.LoadPortModelData(model);

                        LogHelper.General(3, $"{UserAccount.UserName} unloaded carrier {carrierID} from {loadportID}");

                        // Update carrier status asynchronously
                        EF_Service.EF_UpdateCarrierStatusAsync(carrierID, loadportID, JobStatus.Complete, UserAccount.UserName);

                        UnloadingLogMessageFromPTL(new LogValue
                        {
                            Value = $"{carrierID} Removed From {loadportID}",
                            Page = HMIPage.UnloadingPage
                        });
                    }
                    else
                    {
                        // No user account, indicate unauthorized action
                        ErrorUnauthorizedAction(ref model, GwID, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                LogHelper.General(5, $"An error occurred during UnloadProcess: {ex.Message}");
            }
        }
		
        private void UpdateLoadPortState_AbortProcess(ref LoadPortModel model, int GwID, bool IsLoaded)
        {
            try
            {
                if (!IsLoaded)
                {
                    // There's a user account, proceed with the unload process
                    string loadportID = model.LoadPortID;
                    string carrierID = model.LoadPortCarrierLoad;

                    // Update load port state to Empty
                    model.LoadPortState = LoadPortState.Empty;
                    SetLedState(GwID, model.LoadPortNodeAddress.Value, LoadPortState.Empty);
                    model.LoadPortCarrierLoad = null;

                    LoadPort_VM.LoadPortModelData(model);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                LogHelper.General(5, $"An error occurred during UnloadProcess: {ex.Message}");
            }
        }

        private void UpdateLoadPortState_CoolingCompleted(ref LoadPortModel model, bool isInit, int GwID, bool IsLoaded)
        {
			if (IsLoaded)
			{
                ClearErrorUnauthorizedAction(ref model, GwID, 2);
            }
			else
			{
                if (isInit)
                {
                    ErrorUnauthorizedAction(ref model, GwID, 2);
                }
                else
                {
					

					if (LoadPortModels.Any(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess) || x.LoadPortState.Equals(LoadPortState.LoadProcess)))
                    {
                        int address = model.LoadPortNodeAddress.Value;
                        Task.Run(() =>
                        {
                            DateTime dt_Now = DateTime.Now;
                            
                            while (DateTime.Now - dt_Now <= TimeSpan.FromSeconds(2))
                            {
                                //Buffer to double check the error.
                            }
                            LogHelper.General(3, $" Entered IO Recheck");
                            GetIOValue(GwID, address);
                        });
                    }
                    else
                    {
                        ErrorUnauthorizedAction(ref model, GwID, 2);
                    }
                }
            }
        }

        private LoadPortModel FindLoadPortByIPAndAddress(string GatewayIP, int NodeAddress)
        {
            return LoadPortModels.FirstOrDefault(x => x.LoadPortIPAddress.Equals(GatewayIP) && x.LoadPortNodeAddress.Equals(NodeAddress));
        }

        private void HandleUnauthorizedAction(ref LoadPortModel model, int GwID, int index, string errorMessage)
        {
            int errorCode = index == 1 ? ((model.LoadPortRack.Equals("AA") || model.LoadPortRack.Equals("AB") || model.LoadPortRack.Equals("AC")) ? 300 + model.LoadPortNodeAddress.Value : 600 + model.LoadPortNodeAddress.Value) :
                                              ((model.LoadPortRack.Equals("AA") || model.LoadPortRack.Equals("AB") || model.LoadPortRack.Equals("AC")) ? 100 + model.LoadPortNodeAddress.Value : 500 + model.LoadPortNodeAddress.Value);

            if (!model.LoadPortError)
            {
                AlarmService.DefineAlarm(errorCode);
                model.LoadPortError = true;
                SetLedState(GwID, model.LoadPortNodeAddress.Value, LoadPortState.Error);
                LoadPort_VM.LoadPortModelData(model);

                LoadingLogMessageFromPTL(new LogValue
				{
					Value = errorMessage,
					Page = HMIPage.LoadingPage
				});

                UnloadingLogMessageFromPTL(new LogValue
                {
                    Value = errorMessage,
                    Page = HMIPage.UnloadingPage
                });

            }
            else
            {
                SetLedState(GwID, model.LoadPortNodeAddress.Value, LoadPortState.Error);
            }
        }

        private void HandleClearUnauthorizedAction(ref LoadPortModel model, int GwID, int index, string clearedMessage)
        {
            LogHelper.PTL(3, $"Start to : [{MethodBase.GetCurrentMethod().Name}]");
            if (model.LoadPortError)
            {
                model.LoadPortError = false;

                LoadPort_VM.LoadPortModelData(model);

                SetLedState(GwID, model.LoadPortNodeAddress.Value, model.LoadPortState);

                int errorCode = index == 1 ? ((model.LoadPortRack.Equals("AA") || model.LoadPortRack.Equals("AB") || model.LoadPortRack.Equals("AC")) ? 300 + model.LoadPortNodeAddress.Value : 600 + model.LoadPortNodeAddress.Value) :
                                              ((model.LoadPortRack.Equals("AA") || model.LoadPortRack.Equals("AB") || model.LoadPortRack.Equals("AC")) ? 100 + model.LoadPortNodeAddress.Value : 500 + model.LoadPortNodeAddress.Value);
                AlarmService.ClearAlarm(errorCode);

				LoadingLogMessageFromPTL(new LogValue
				{
					Value = clearedMessage,
					Page = HMIPage.LoadingPage
				});
				
                UnloadingLogMessageFromPTL(new LogValue
                {
                    Value = clearedMessage,
                    Page = HMIPage.UnloadingPage
                });
			}
			else
			{
                SetLedState(GwID, model.LoadPortNodeAddress.Value, model.LoadPortState);
            }
            LogHelper.PTL(3, $"End Of Process : [{MethodBase.GetCurrentMethod().Name}]");
        }

        // Usage in ErrorUnauthorizedAction
        private void ErrorUnauthorizedAction(ref LoadPortModel model, int GwID, int index)
        {
			LogHelper.General(3, $"{nameof(ErrorUnauthorizedAction)}| LoadportID: {model.LoadPortID} / GwID: {GwID} / Index: {index}");
            switch (index)
            {
                case 1:
                    HandleUnauthorizedAction(ref model, GwID, 1, $"[ERROR] - Unauthorized Carrier Inside {model.LoadPortID}");
                    break;
                case 2:
                    HandleUnauthorizedAction(ref model, GwID, 2, $"[ERROR] - Unauthorized Removal Carrier From {model.LoadPortID}");
                    break;
            }
        }

        // Usage in ClearErrorUnauthorizedAction
        private void ClearErrorUnauthorizedAction(ref LoadPortModel model, int GwID, int index)
        {
			LogHelper.General(3, $"{nameof(ClearErrorUnauthorizedAction)}| LoadportID: {model.LoadPortID} / GwID: {GwID} / Index: {index}");
            switch (index)
            {
                case 1:
                    HandleClearUnauthorizedAction(ref model, GwID, 1, $"[CLEARED]-{model.LoadPortID}");
                    break;
                case 2:
                    HandleClearUnauthorizedAction(ref model, GwID, 2, $"[CLEARED]-{model.LoadPortID}");
                    break;
            }
        }

    }
}
