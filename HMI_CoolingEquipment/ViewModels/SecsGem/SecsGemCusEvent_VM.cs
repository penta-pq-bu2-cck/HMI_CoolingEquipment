using System.Data.OleDb;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using HMI_CoolingEquipment.Models;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace HMI_CoolingEquipment.ViewModels
{
    public class SecsGemCusEvent_VM : ViewModelBase
    {
        private ObservableCollection<SecsGemCusEvent> _secsGemCusEventList = new ObservableCollection<SecsGemCusEvent>();
        public ObservableCollection<SecsGemCusEvent> SecsGemCusEventList
        {
            get { return _secsGemCusEventList;}
            set
            {
                if(value != SecsGemCusEventList)
                {
                    _secsGemCusEventList = value;
                    RaisePropertyChanged(nameof(SecsGemCusEventList));
                }
            }
        }

        public void SecsGemCusEventWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string GemDBFilePat = Assembly.GetEntryAssembly().Location.Replace(Assembly.GetEntryAssembly().Location.Split('\\')[Assembly.GetEntryAssembly().Location.Split('\\').Count() - 1], "GEM.accdb");

            try
            {
                string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={GemDBFilePat}";
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    connection.Open();
                    OleDbDataReader reader = null;
                    OleDbCommand command = new OleDbCommand("SELECT * from  secs_cusevent", connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SecsGemCusEventList.Add(new SecsGemCusEvent()
                        {
                            EventID = reader[0].ToString(),
                            EventName = reader[1].ToString(),
                            EventGroup = reader[2].ToString(),
                            EventText = reader[3].ToString(),
                            EventEnable = reader[4].ToString(),
                            SelectEventCommand = new RelayCommand<SecsGemCusEvent>(SelectEventCommandEvent)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SelectEventCommandEvent(SecsGemCusEvent _SecsGemCusEvent)
        {
            Messenger.Default.Send(_SecsGemCusEvent);
        }
    }
}
