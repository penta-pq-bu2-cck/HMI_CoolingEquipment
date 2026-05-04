using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Global;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace HMI_CoolingEquipment.ViewModels
{
    public class Settings_VM : ViewModelBase
    {
        public ICommand ImportAlarmCommand { get; set; }
        public ICommand ImportSettingsCommand { get; set; }
        public ICommand ImportLoadPortConfigurationCommand { get; set; }
        public ICommand SaveButtonCommand { get; set; }

        private List<SettingsModel> _settings = new List<SettingsModel>();
        public List<SettingsModel> Settings
        {
            get { return _settings; }
            set
            {
                if (value != _settings)
                {
                    _settings = value;
                    RaisePropertyChanged(nameof(Settings));
                }
            }
        }

        public Settings_VM()
        {
            Messenger.Default.Register<List<SettingsModel>>(this, UpdateSettingsModel);
            ImportAlarmCommand = new RelayCommand(ImportAlarmCommandEvent);
            ImportSettingsCommand = new RelayCommand(ImportSettingsCommandEvent);
            ImportLoadPortConfigurationCommand = new RelayCommand(ImportLoadPortConfigurationCommandEvent);
            SaveButtonCommand = new RelayCommand(SaveButtonCommandEvent);
        }

        private void UpdateSettingsModel(List<SettingsModel> model)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Settings.Clear();
                model.Where(x => x.Page.Equals(HMIPage.Settings));

                if (model.Count() > 0)
                {
                    Settings = model;
                }

                if (Settings.Exists(x => x.Items.Equals("Loading Mode")))
                {
                    Settings.Find(x => x.Items.Equals("Loading Mode")).ComboBoxValues.Clear();
                    Settings.Find(x => x.Items.Equals("Loading Mode")).ComboBoxValues.Add("Load by Carrier ID");
                    Settings.Find(x => x.Items.Equals("Loading Mode")).ComboBoxValues.Add("Load by Lot ID");
                    Settings.Find(x => x.Items.Equals("Loading Mode")).Value = (int.Parse(Settings.Find(x => x.Items.Equals("Loading Mode")).Value)).ToString();

                    GlobalAttributesClass.LoadingMode = byte.Parse(Settings.Find(x => x.Items.Equals("Loading Mode")).Value);
                }
            }));
        }

        private void ImportAlarmCommandEvent()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                DialogResult drResult = ofd.ShowDialog();
                if (drResult == DialogResult.OK)
                {
                    string sFileName = ofd.FileName;

                    List<ALARMDEFINITION> model = AlarmService.GetAlarmDefinition(ofd.FileName);
                    EF_Service.EF_UpdateAlarmDefinition(model);
                    SecsGemService.UploadAlarmToDatabase(model);

                    System.Windows.MessageBox.Show($"Alarm Import Successful", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
                System.Windows.MessageBox.Show($"Error:\n{ex.Message}", "Import Alarm Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportSettingsCommandEvent()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                DialogResult drResult = ofd.ShowDialog();
                if (drResult == DialogResult.OK)
                {
                    string sFileName = ofd.FileName;

                    List<SETTINGS> model = SettingsService.GetSettings(ofd.FileName);
                    EF_Service.EF_UpdateSettings(model);

                    System.Windows.MessageBox.Show($"Settings Import Successful", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
                System.Windows.MessageBox.Show($"Error:\n{ex.Message}", "Import Settings Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ImportLoadPortConfigurationCommandEvent()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                DialogResult drResult = ofd.ShowDialog();
                if (drResult == DialogResult.OK)
                {
                    string sFileName = ofd.FileName;

                    if (System.Windows.MessageBox.Show("Please make sure all LoadPort are empty before you proceed to configure the LoadPort", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning).Equals(MessageBoxResult.OK))
                    {
                        List<LOADPORT> model = LoadPortService.GetLoadPortConfig(ofd.FileName);
                        EF_Service.EF_UpdateLoadPortConfiguration(model); 
                    }

                    System.Windows.MessageBox.Show($"LoadPort Configuration Import Successful", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
                System.Windows.MessageBox.Show($"Error:\n{ex.Message}", "Import LoadPort Configuration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButtonCommandEvent()
        {
            try
            {
                if (GlobalAttributesClass.HMINoActiveJob)
                {
                    EF_Service.EF_UpdateSettingsValue(GlobalClassFunction.SettingsMap(Settings));
                    System.Windows.MessageBox.Show($"Settings Value Saved", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    System.Windows.MessageBox.Show("Please Complete Ongoing Job Before Changing System Settings","Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.General(5, ex.Message);
                System.Windows.MessageBox.Show($"Error:\n{ex.Message}", "Failed to save settings value", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
