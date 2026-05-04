using ModbusLib;
using ModbusLib.Protocols;
using System.Net;
using System;
using System.Net.Sockets;
using System.Text;
using System.Configuration;
using System.Timers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HMI_CoolingEquipment.Communication
{
    public static class ModBus_Service
    {
        private static Socket _socket;
        private static ICommClient _portClient;
        private static ModbusClient _driver;
        private static int _transactionId = 0;
        private static int BlinkingCounting = 0;
        private static ushort CurrentAddress = 0;
        private static Timer BlinkingTimer = new Timer(500);
        public static Timer HeartBeatTimer = new Timer(100);

        public static ushort RedAddress = 16;
        public static ushort YellowAddress = 17;
        public static ushort GreenAddress = 18;
        public static ushort BuzzerAddress = 19;

        public static void ConnectToModBus()
        {
            if (ConfigurationManager.AppSettings["EnableTowerLight"].Equals("1"))
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                _socket.SendTimeout = 2000;
                _socket.ReceiveTimeout = 2000;
                _socket.Connect(new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["TowerLightIP"]), 502));
                _portClient = _socket.GetClient();
                _driver = new ModbusClient(new ModbusTcpCodec()) { Address = 1 };
                _driver.OutgoingData += DriverOutgoingData;
                _driver.IncommingData += DriverIncommingData;
                LogHelper.General(3, String.Format("Connected using TCP to {0}", _socket.RemoteEndPoint));
                BlinkingTimer.Elapsed += BlinkingLED;
                HeartBeatTimer.Elapsed += ExecuteReadCommand;
                HeartBeatTimer.Start();
            }
        }

        private static void DriverIncommingData(byte[] data, int len)
        {
            var hex = new StringBuilder(len);
            for (int i = 0; i < len; i++)
            {
                hex.AppendFormat("{0:x2} ", data[i]);
            }
        }

        private static void DriverOutgoingData(byte[] data)
        {
            var hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                hex.AppendFormat("{0:x2} ", b);
        }

        public static void LedBuzzerTrigger(ushort address, bool isEnable)
        {
            try
            {
                if (ConfigurationManager.AppSettings["EnableTowerLight"].Equals("1"))
                {
                    if (isEnable)
                    {
                        switch (address)
                        {
                            case 18:
                                var command = new ModbusCommand(ModbusCommand.FuncWriteCoil)
                                {
                                    Offset = address,
                                    Count = 1,
                                    TransId = _transactionId++,
                                    Data = new ushort[1]
                                };
                                command.Data[0] = (ushort)(isEnable ? 256 : 0 & 0x0100);
                                if (_driver != null)
                                {
                                    var result = _driver.ExecuteGeneric(_portClient, command); 
                                }
                                break;
                            case 16:
                                CurrentAddress = address;

                                if (!BlinkingTimer.Enabled)
                                {
                                    BlinkingTimer.Start();
                                }

                                break;
                            case 17:
                                CurrentAddress = address;

                                if (!BlinkingTimer.Enabled)
                                {
                                    BlinkingTimer.Start();
                                }
                                break;

                        }
                    }
                    else
                    {
                        if (BlinkingTimer.Enabled)
                        {
                            BlinkingTimer.Stop();
                        }

                        var command = new ModbusCommand(ModbusCommand.FuncWriteCoil)
                        {
                            Offset = address,
                            Count = 1,
                            TransId = _transactionId++,
                            Data = new ushort[1]
                        };
                        command.Data[0] = (ushort)(0 & 0x0100);
                        if (_driver != null)
                        {
                            var result = _driver.ExecuteGeneric(_portClient, command); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }
        }

        public static void DisconnectFromModBus()
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket.Dispose();
                _socket = null;
            }

            _portClient = null;
            _driver = null;

            LogHelper.General(3, "Disconnected");
        }

        private static void BlinkingLED(object sender, ElapsedEventArgs e)
        {
            var command = new ModbusCommand(ModbusCommand.FuncWriteCoil)
            {
                Offset = CurrentAddress,
                Count = 1,
                TransId = _transactionId++,
                Data = new ushort[1]
            };
            command.Data[0] = (ushort)(BlinkingCounting.Equals(1) ? 256 : 0 & 0x0100);
            var result = _driver.ExecuteGeneric(_portClient, command);

            if (ConfigurationManager.AppSettings["EnableBuzzer"].Equals("1") && CurrentAddress.Equals(16))
            {
                command = new ModbusCommand(ModbusCommand.FuncWriteCoil)
                {
                    Offset = BuzzerAddress,
                    Count = 1,
                    TransId = _transactionId++,
                    Data = new ushort[1]
                };
                command.Data[0] = (ushort)(BlinkingCounting.Equals(1) ? 256 : 0 & 0x0100);
                result = _driver.ExecuteGeneric(_portClient, command);
            }

            BlinkingCounting = BlinkingCounting.Equals(1) ? 0 : 1;
        }

        private static void ExecuteReadCommand(object sender, ElapsedEventArgs e)
        {
            try
            {
                var command = new ModbusCommand(ModbusCommand.FuncReadCoils) { Offset = 0, Count = 64, TransId = _transactionId++ };
                if (_driver != null)
                {
                    var result = _driver.ExecuteGeneric(_portClient, command);
                    if (result.Status == CommResponse.Ack)
                    {

                    }
                    else
                    {
                        LogHelper.General(5, String.Format("Failed to execute Heartbeat: Error code:{0}", result.Status));
                    }
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
            }
        }
    }
}
