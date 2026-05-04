using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

namespace HMI_CoolingEquipment
{
    public static class AlarmService
    {
        public static List<ALARMDEFINITION> GetAlarmDefinition(string filepath)
        {
            List<ALARMDEFINITION> alarmDefinitionModels = new List<ALARMDEFINITION>();
            ALARMDEFINITION temp_model;
            try
            {
                using (OleDbConnection oleDbConnection = new OleDbConnection($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filepath};Extended Properties =\"Excel 12.0;HDR=NO;IMEX=1;\""))
                {
                    oleDbConnection.Open();
                    DataTable dataTable = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        string sheetName = dataRow["TABLE_NAME"].ToString();

                        if (sheetName.Contains("Error") || sheetName.Contains("Warning"))
                        {
                            AlarmType type = sheetName.Contains("Error") ? AlarmType.Error : AlarmType.Warning;

                            using (OleDbCommand oleDbCommand = new OleDbCommand($"SELECT * FROM [{dataRow["TABLE_NAME"]}]", oleDbConnection))
                            {
                                using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                                {
                                    string tablePresence = string.Empty;

                                    while (oleDbDataReader.Read())
                                    {
                                        if (oleDbDataReader[0].ToString().Contains("START"))
                                        {
                                            oleDbDataReader.Read();
                                            break;
                                        }
                                    }

                                    while (oleDbDataReader.Read() && !oleDbDataReader[0].ToString().Contains("END"))
                                    {
                                        temp_model = new ALARMDEFINITION();

                                        temp_model.AlarmType = type;

                                        temp_model.AlarmCode = int.Parse(oleDbDataReader[1].ToString());

                                        if (!string.IsNullOrEmpty(oleDbDataReader[2].ToString()))
                                        {
                                            temp_model.AlarmModule = oleDbDataReader[2].ToString();
                                        }

                                        if (!string.IsNullOrEmpty(oleDbDataReader[3].ToString()))
                                        {
                                            temp_model.AlarmMessage = oleDbDataReader[3].ToString();
                                        }

                                        if (!string.IsNullOrEmpty(oleDbDataReader[4].ToString()))
                                        {
                                            temp_model.AlarmAction = oleDbDataReader[4].ToString();
                                        }

                                        alarmDefinitionModels.Add(temp_model);
                                    }
                                }
                            }
                        }
                    }
                    oleDbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
                throw ex;
            }

            return alarmDefinitionModels;
        }

        public static void DefineAlarm(int alarmCode)
        {
            EF_Service.EF_InsertAlarmToAlarmCurrentTableAsync(alarmCode);
        }

        public static void ClearAlarm(int alarmCode = 0)
        {
            EF_Service.MoveAlarmFromCurrentToHistory(alarmCode);
        }
    }
}
