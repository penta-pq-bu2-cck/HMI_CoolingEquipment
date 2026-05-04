using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Models;
using NLog;
using System;
using System.Diagnostics;

namespace HMI_CoolingEquipment
{
    public static class LogHelper
    {
        private static Logger generalLog = LogManager.GetLogger("General");
        private static Logger opcLog = LogManager.GetLogger("OPC");
        private static Logger secsgemLog = LogManager.GetLogger("SecsGem");
        private static Logger tcpLog = LogManager.GetLogger("TCP");
        private static Logger efLog = LogManager.GetLogger("EF");
        private static Logger ptlLog = LogManager.GetLogger("PTL");
        private static Logger alarmLog = LogManager.GetLogger("Alarm");
        private static Logger errorLog = LogManager.GetLogger("Error"); 
        private static Exception ex;

        /// <summary>
        /// A helper method to log messages to the General Log.
        /// </summary>
        /// <param name="number">The log level. (1:Trace, 2:Debug, 3:Info, 4:Warning, 5:Error, 6:Fatal)</param>
        /// <param name="message">The message to log.</param>
        /// <param name="className">The class name.</param>
        public static void General(int number, string message)
        {
            Debug.WriteLine($"[{nameof(General)}] Type: {number} | Message: {message}");
            switch (number)
            {
                case 1:
                    generalLog.Trace(message);
                    break;

                case 2:
                    generalLog.Debug(message);
                    break;

                case 3:
                    generalLog.Info(message);
                    break;

                case 4:
                    generalLog.Warn(message);
                    break;

                case 5:
                    ex = new Exception(message);
                    generalLog.Error(ex);
                    errorLog.Error(ex);
                    break;

                case 6:
                    ex = new Exception(message);
                    generalLog.Fatal(ex);
                    errorLog.Fatal(ex);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// A helper method to log messages to the OPC Log.
        /// </summary>
        /// <param name="number">The log level. (1:Trace, 2:Debug, 3:Info, 4:Warning, 5:Error, 6:Fatal)</param>
        /// <param name="message">The message to log.</param>
        /// <param name="className">The class name.</param>
        public static void OPC(int number, string message)
        {
            Debug.WriteLine($"[{nameof(OPC)}] Type: {number} | Message: {message}");
            switch (number)
            {
                case 1:
                    opcLog.Trace(message);
                    break;

                case 2:
                    opcLog.Debug(message);
                    break;

                case 3:
                    opcLog.Info(message);
                    break;

                case 4:
                    opcLog.Warn(message);
                    break;

                case 5:
                    ex = new Exception(message);
                    opcLog.Error(ex);
                    errorLog.Error(ex);
                    break;

                case 6:
                    ex = new Exception(message);
                    opcLog.Fatal(ex);
                    errorLog.Fatal(ex);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// A helper method to log messages to the SecsGem Log.
        /// </summary>
        /// <param name="number">The log level. (1:Trace, 2:Debug, 3:Info, 4:Warning, 5:Error, 6:Fatal)</param>
        /// <param name="message">The message to log.</param>
        /// <param name="className">The class name.</param>
        public static void SecsGem(int number, string message)
        {
            LogValue secsGemLogValue = new LogValue();
            Debug.WriteLine($"[{nameof(SecsGem)}] Type: {number} | Message: {message}");
            switch (number)
            {
                case 1:
                    secsgemLog.Trace(message);
                    secsGemLogValue.Value = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} [Trace] = {message}";
                    break;

                case 2:
                    secsgemLog.Debug(message);
                    secsGemLogValue.Value = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} [Debug] = {message}";
                    break;

                case 3:
                    secsgemLog.Info(message);
                    secsGemLogValue.Value = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} [Info] = {message}";
                    break;

                case 4:
                    secsgemLog.Warn(message);
                    secsGemLogValue.Value = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} [Warn] = {message}";
                    break;

                case 5:
                    ex = new Exception(message);
                    secsgemLog.Error(ex);
                    errorLog.Error(ex);
                    secsGemLogValue.Value = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} [Error] = {message}";
                    break;

                case 6:
                    ex = new Exception(message);
                    secsgemLog.Fatal(ex);
                    errorLog.Fatal(ex);
                    secsGemLogValue.Value = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} [Fatal] = {message}";
                    break;

                default:
                    break;
            }

            secsGemLogValue.Page = HMIPage.SecsGemPage;

            Messenger.Default.Send(secsGemLogValue);
        }

        /// <summary>
        /// A helper method to log messages to the TCP Log.
        /// </summary>
        /// <param name="number">The log level. (1:Trace, 2:Debug, 3:Info, 4:Warning, 5:Error, 6:Fatal)</param>
        /// <param name="message">The message to log.</param>
        /// <param name="className">The class name.</param>
        public static void TCP(int number, string message)
        {
            Debug.WriteLine($"[{nameof(TCP)}] Type: {number} | Message: {message}");
            switch (number)
            {
                case 1:
                    tcpLog.Trace(message);
                    break;

                case 2:
                    tcpLog.Debug(message);
                    break;

                case 3:
                    tcpLog.Info(message);
                    break;

                case 4:
                    tcpLog.Warn(message);
                    break;

                case 5:
                    ex = new Exception(message);
                    tcpLog.Error(ex);
                    errorLog.Error(ex);
                    break;

                case 6:
                    ex = new Exception(message);
                    tcpLog.Fatal(ex);
                    errorLog.Fatal(ex);
                    break;

                default:
                    break;
            }
        }
        
        /// <summary>
        /// A helper method to log messages to the SiemensOPC Log.
        /// </summary>
        /// <param name="number">The log level. (1:Trace, 2:Debug, 3:Info, 4:Warning, 5:Error, 6:Fatal)</param>
        /// <param name="message">The message to log.</param>
        /// <param name="className">The class name.</param>
        public static void SiemensOPC(int number, string message)
        {
            Debug.WriteLine($"[{nameof(SiemensOPC)}] Type: {number} | Message: {message}");
            switch (number)
            {
                case 1:
                    secsgemLog.Trace(message);
                    break;

                case 2:
                    secsgemLog.Debug(message);
                    break;

                case 3:
                    secsgemLog.Info(message);
                    break;

                case 4:
                    secsgemLog.Warn(message);
                    break;

                case 5:
                    ex = new Exception(message);
                    secsgemLog.Error(ex);
                    errorLog.Error(ex);
                    break;

                case 6:
                    ex = new Exception(message);
                    secsgemLog.Fatal(ex);
                    errorLog.Fatal(ex);
                    break;

                default:
                    break;
            }
        }


        /// <summary>
        /// A helper method to log messages to the EntityFramework Log.
        /// </summary>
        /// <param name="number">The log level. (1:Trace, 2:Debug, 3:Info, 4:Warning, 5:Error, 6:Fatal)</param>
        /// <param name="message">The message to log.</param>
        /// <param name="className">The class name.</param>
        public static void EF(int number, string message)
        {
            Debug.WriteLine($"[{nameof(EF)}] Type: {number} | Message: {message}");
            switch (number)
            {
                case 1:
                    efLog.Trace(message);
                    break;

                case 2:
                    efLog.Debug(message);
                    break;

                case 3:
                    efLog.Info(message);
                    break;

                case 4:
                    efLog.Warn(message);
                    break;

                case 5:
                    ex = new Exception(message);
                    efLog.Error(ex);
                    errorLog.Error(ex);
                    break;

                case 6:
                    ex = new Exception(message);
                    efLog.Fatal(ex);
                    errorLog.Fatal(ex);
                    break;

                default:
                    break;
            }
        } 
        
        /// <summary>
        /// A helper method to log messages to the PickToLight Log.
        /// </summary>
        /// <param name="number">The log level. (1:Trace, 2:Debug, 3:Info, 4:Warning, 5:Error, 6:Fatal)</param>
        /// <param name="message">The message to log.</param>
        /// <param name="className">The class name.</param>
        public static void PTL(int number, string message)
        {
            Debug.WriteLine($"[{nameof(PTL)}] Type: {number} | Message: {message}");
            switch (number)
            {
                case 1:
                    ptlLog.Trace(message);
                    break;

                case 2:
                    ptlLog.Debug(message);
                    break;

                case 3:
                    ptlLog.Info(message);
                    break;

                case 4:
                    ptlLog.Warn(message);
                    break;

                case 5:
                    ex = new Exception(message);
                    ptlLog.Error(ex);
                    errorLog.Error(ex);
                    break;

                case 6:
                    ex = new Exception(message);
                    ptlLog.Fatal(ex);
                    errorLog.Fatal(ex);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// A helper method to log messages to the Alarm Log.
        /// </summary>
        /// <param name="number">The log level. (1:Trace, 2:Debug, 3:Info, 4:Warning, 5:Error, 6:Fatal)</param>
        /// <param name="message">The message to log.</param>
        /// <param name="className">The class name.</param>
        public static void Alarm(int number, string message)
        {
            Debug.WriteLine($"[{nameof(Alarm)}] Type: {number} | Message: {message}");
            switch (number)
            {
                case 1:
                    alarmLog.Trace(message);
                    break;

                case 2:
                    alarmLog.Debug(message);
                    break;

                case 3:
                    string[] alarm = message.Split(',');
                    LogManager.Configuration.Variables["AlarmDateTime"] = alarm[0];
                    LogManager.Configuration.Variables["AlarmType"] = alarm[1];
                    LogManager.Configuration.Variables["AlarmCode"] = alarm[2];
                    LogManager.Configuration.Variables["AlarmModule"] = alarm[3];
                    LogManager.Configuration.Variables["AlarmMessage"] = alarm[4];
                    LogManager.Configuration.Variables["AlarmAction"] = alarm[5];
                    alarmLog.Info(message);
                    break;

                case 4:
                    alarmLog.Warn(message);
                    break;

                case 5:
                    ex = new Exception(message);
                    alarmLog.Error(ex);
                    errorLog.Error(ex);
                    break;

                case 6:
                    ex = new Exception(message);
                    alarmLog.Fatal(ex);
                    errorLog.Fatal(ex);
                    break;

                default:
                    break;
            }
        }
    }
}
