using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Globalization;

namespace HMI_CoolingEquipment
{
    public class GlobalAttributesClass : ViewModelBase
    {
        public static string LoadingTime = "1";
        public static string TimeFormat = "MM/dd/yyyy HH:mm:ss";
        public static string DateFormat = "MM/dd/yyyy";
        public static byte Led_State_Off = 0;
        public static byte Led_State_On = 1;
        public static byte Led_State_Blinking_Normal = 3;
        public static byte Led_State_Blinking_Error = 4;
        public static byte Led_Colour_Red = 0;
        public static byte Led_Colour_Green = 1;
        public static CultureInfo Provider = CultureInfo.InvariantCulture;
        public static Dictionary<int,bool> SetBestPollRangeBool = new Dictionary<int,bool>();
        public static Dictionary<int,bool> DetectTagStatus = new Dictionary<int,bool>();
        public static bool LedTestingEnable = false;
        public static bool HMINoActiveJob = true;
        public static bool HMIWaitingForHost = false;
        public static byte LoadingMode = 0; //0 = check by carrierID scanned, PTL light turn on for each carrier scanned only, 1 = check LotID using carrierID scanned, PTL light turn on for whole lot after scanning first carrier
    }
}