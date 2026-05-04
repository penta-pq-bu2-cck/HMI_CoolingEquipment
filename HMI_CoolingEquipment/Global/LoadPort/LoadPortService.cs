using HMI_CoolingEquipment.Models;
using HMI_CoolingEquipment.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMI_CoolingEquipment.Global
{
    public static class LoadPortService
    {
        public static List<LOADPORT> GetLoadPortConfig(string filepath)
        {
            List<LOADPORT> loadport = new List<LOADPORT>();
            LOADPORT temp_model;
            try
            {

                using (OleDbConnection oleDbConnection = new OleDbConnection($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filepath};Extended Properties =\"Excel 12.0;HDR=NO;IMEX=1;\""))
                {
                    oleDbConnection.Open();
                    DataTable dataTable = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        string sheetName = dataRow["TABLE_NAME"].ToString();

                        if (sheetName.Contains("LoadPort"))
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
                                        temp_model = new LOADPORT();

                                        if (!string.IsNullOrEmpty(oleDbDataReader[1].ToString()) && !string.IsNullOrEmpty(oleDbDataReader[2].ToString()) && !string.IsNullOrEmpty(oleDbDataReader[3].ToString()) && !string.IsNullOrEmpty(oleDbDataReader[4].ToString()) && !string.IsNullOrEmpty(oleDbDataReader[5].ToString()) && !string.IsNullOrEmpty(oleDbDataReader[6].ToString()) && !string.IsNullOrEmpty(oleDbDataReader[7].ToString()) && !string.IsNullOrEmpty(oleDbDataReader[8].ToString()))
                                        {
                                            temp_model.LoadPortID = oleDbDataReader[1].ToString();
                                            temp_model.LoadPortRack = oleDbDataReader[2].ToString();
                                            temp_model.LoadPortColumn = oleDbDataReader[3].ToString();
                                            temp_model.LoadPortRow = oleDbDataReader[4].ToString();
                                            temp_model.LoadPortPriority = oleDbDataReader[5].ToString();
                                            temp_model.LoadPortType = oleDbDataReader[6].ToString();
                                            temp_model.LoadPortIPAddress = oleDbDataReader[7].ToString();
                                            temp_model.LoadPortNodeAddress = int.Parse(oleDbDataReader[8].ToString());
                                            temp_model.LoadPortIsEnable = oleDbDataReader[9].ToString().Equals("0") ? false : true;

                                            loadport.Add(temp_model);
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
            return loadport;

        }
    }
}
