using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Models;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Reflection;
using System.Windows;
using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;

namespace HMI_CoolingEquipment.ViewModels
{
    public class SecsGemCusVar_VM : ViewModelBase
    {
        private ObservableCollection<SecsGemCusVar> _SecsGemCusVarList = new ObservableCollection<SecsGemCusVar>();
        public ObservableCollection<SecsGemCusVar> SecsGemCusVarList
        {
            get { return _SecsGemCusVarList; }
            set
            {
                if (value != SecsGemCusVarList)
                {
                    _SecsGemCusVarList = value;
                    RaisePropertyChanged(nameof(SecsGemCusVarList));
                }
            }
        }

        public void SecsGemCusVarWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string GemDBFilePat = Assembly.GetEntryAssembly().Location.Replace(Assembly.GetEntryAssembly().Location.Split('\\')[Assembly.GetEntryAssembly().Location.Split('\\').Count() - 1], "GEM.accdb");

            try
            {
                string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={GemDBFilePat}";
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    connection.Open();
                    OleDbDataReader reader = null;
                    OleDbCommand command = new OleDbCommand("SELECT * from  secs_cusvar", connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SecsGemCusVarList.Add(new SecsGemCusVar()
                        {
                            VarID = reader[0].ToString(),
                            VarName = reader[1].ToString(),
                            VarGroup = reader[2].ToString(),
                            VarDesc = reader[3].ToString(),
                            VarType = reader[4].ToString(),
                            VarFormat = reader[5].ToString(),
                            VarVal = reader[6].ToString(),
                            VarDefaultVal = reader[7].ToString(),
                            VarMin = reader[8].ToString(),
                            VarMax = reader[9].ToString(),
                            VarUnit = reader[10].ToString(),
                            VarParentKey = reader[11].ToString(),
                            VarAccessId = reader[12].ToString(),
                            VarOrderByAccessId = reader[13].ToString(),
                            VarAllowDuplicates = reader[14].ToString(),
                            SelectVarCommand = new RelayCommand<SecsGemCusVar>(SelectVarCommandEvent)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SelectVarCommandEvent(SecsGemCusVar _SecsGemCusVar)
        {
            Messenger.Default.Send(_SecsGemCusVar);
        }
    }
}
