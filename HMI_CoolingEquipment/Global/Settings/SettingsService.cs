using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace HMI_CoolingEquipment.Global
{
    public static class SettingsService
    {
        public static List<SETTINGS> GetSettings(string filepath)
        {
            List<SETTINGS> settings = new List<SETTINGS>();
            SETTINGS temp_model;
            try
            {

                using (OleDbConnection oleDbConnection = new OleDbConnection($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filepath};Extended Properties =\"Excel 12.0;HDR=NO;IMEX=1;\""))
                {
                    oleDbConnection.Open();
                    DataTable dataTable = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        string sheetName = dataRow["TABLE_NAME"].ToString();

                        if (sheetName.Contains("Settings"))
                        {
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
                                        temp_model = new SETTINGS();
                                        
                                        if (!string.IsNullOrEmpty(oleDbDataReader[1].ToString()) && !string.IsNullOrEmpty(oleDbDataReader[2].ToString()) && !string.IsNullOrEmpty(oleDbDataReader[3].ToString()) && !string.IsNullOrEmpty(oleDbDataReader[4].ToString()))
                                        {
                                            temp_model.Items = oleDbDataReader[1].ToString();
                                            temp_model.Value = oleDbDataReader[2].ToString();
                                            temp_model.Type = (SettingsType)Enum.Parse(typeof(SettingsType), oleDbDataReader[3].ToString(), true);
                                            temp_model.Page = (HMIPage)Enum.Parse(typeof(HMIPage), oleDbDataReader[4].ToString(), true);
                                            
                                            settings.Add(temp_model); 
                                        }
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
            
            return settings;
        }
    }
}
