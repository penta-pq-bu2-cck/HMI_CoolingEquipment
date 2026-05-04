using System.Runtime.InteropServices;

namespace HMI_CoolingEquipment.Communication
{
    public class PTLUtils
    {
        public static string DecodeMsg(int code)
        {
            string returnValue;
            string tmpstr;
            switch (code)
            {
                case -3:
                    tmpstr = "Parameter data is error !";
                    break;
                case -2:
                    tmpstr = "TCP is not created yet !";
                    break;
                case -1:
                    tmpstr = "DAP_ID out of range !";
                    break;
                case 0:
                    tmpstr = "Closed";
                    break;
                case 1:
                    tmpstr = "Open";
                    break;
                case 2:
                    tmpstr = "Listening";
                    break;
                case 3:
                    tmpstr = "Connection is Pending";
                    break;
                case 4:
                    tmpstr = "Resolving the host name";
                    break;
                case 5:
                    tmpstr = "Host is Resolved";
                    break;
                case 6:
                    tmpstr = "Waitting"; //"Waiting to Connect"
                    break;
                case 7:
                    tmpstr = "ok"; //"Connected ok "
                    break;
                case 8:
                    tmpstr = "Connection is closing";
                    break;
                case 9:
                    tmpstr = "State error has occurred";
                    break;
                case 10:
                    tmpstr = "Connection state is undetermined";
                    break;
                default:
                    tmpstr = "Unknown Error Code";
                    break;
            }

            returnValue = tmpstr;
            return returnValue;
        }

        #region Import Fuction From DLL

        [DllImport("Dapapi.dll")]
        public static extern int AB_API_Open();

        [DllImport("Dapapi.dll")]
        public static extern int AB_API_Close();

        [DllImport("Dapapi.dll")]
        public static extern int AB_GW_Cnt();

        [DllImport("Dapapi.dll")]
        public static extern int AB_GW_Conf(int ndx, ref int Gateway_id, ref byte ip, ref int port);

        [DllImport("Dapapi.dll")]
        public static extern int AB_GW_Open(int Gateway_id);

        [DllImport("Dapapi.dll")]
        public static extern int AB_GW_Close(int Gateway_id);

        [DllImport("Dapapi.dll")]
        public static extern int AB_GW_Status(int Gateway_id);

        [DllImport("Dapapi.dll")]
        public static extern int AB_GW_RcvButton(ref byte data, ref int data_cnt);
        
        [DllImport("Dapapi.dll")]
        public static extern int AB_GW_TagDiag(int Gateway_id, int port);

        [DllImport("Dapapi.dll")]
        public static extern int AB_GW_SetPollRang(int Gateway_id, int max_node);

        [DllImport("Dapapi.dll")]
        public static extern int AB_LB_DspNum(int Gateway_id, int Node_Addr, int Disp_Int, byte Dot, int interval);

        [DllImport("Dapapi.dll")]
        public static extern int AB_LB_DspStr(int Gateway_id, int Node_Addr, string Disp_Str, byte Dot, int interval);

        [DllImport("Dapapi.dll")]
        public static extern int AB_LB_DspAddr(int Gateway_id, int Node_Addr);

        [DllImport("Dapapi.dll")]
        public static extern int AB_Tag_RcvMsg(ref int Gateway_id, ref int tag_addr, ref short subcmd, ref short msg_type, ref byte data, ref int data_cnt);

        [DllImport("Dapapi.dll")]
        public static extern int AB_AHA_ClrDsp(int Gateway_id, int Node_Addr);
        
        [DllImport("Dapapi.dll")]
        public static extern int AB_DIO_ReadIoStatus(int Gateway_id, int Node_Addr);

        [DllImport("Dapapi.dll")]
        public static extern int AB_LED_Status(int Gateway_id, int Node_Addr, byte Lamp_color, byte Lamp_STA);

        #endregion
    }
}
