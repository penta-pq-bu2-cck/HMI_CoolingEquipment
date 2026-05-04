using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HMI_CoolingEquipment.Views;
using System.Collections.Generic;
using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Models;
using System.Windows;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using HMI_CoolingEquipment.Communication;
using System.Configuration;

namespace HMI_CoolingEquipment.ViewModels
{
    public class Alarm_VM : ViewModelBase
    {
        private readonly AlarmMessageWindow alarmMessageWindow = new AlarmMessageWindow(new AlarmMessage_VM());

        public ICommand AlarmViewCommand { get; set; }

        public Alarm_VM()
        {
            Messenger.Default.Register<List<AlarmCurrentModel>>(this, AutoPopUpAlarm);
            AlarmViewCommand = new RelayCommand(AlarmViewCommandEvent);
        }

        private void AlarmViewCommandEvent()
        {
            alarmMessageWindow.Show();
        }

        private void AutoPopUpAlarm(List<AlarmCurrentModel> model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ModBus_Service.LedBuzzerTrigger(ModBus_Service.RedAddress, false);
                ModBus_Service.LedBuzzerTrigger(ModBus_Service.YellowAddress, false);
                ModBus_Service.LedBuzzerTrigger(ModBus_Service.GreenAddress, false);
                ModBus_Service.LedBuzzerTrigger(ModBus_Service.BuzzerAddress, false);

                if (model.Count > 0)
                {
                    if(model.Count(x => x.AlarmType.Equals(AlarmType.Error)) > 0)
                    {
                        ModBus_Service.LedBuzzerTrigger(ModBus_Service.RedAddress, true);
                    }
                    else if (model.Count(x => x.AlarmType.Equals(AlarmType.Warning)) > 0)
                    {
                        ModBus_Service.LedBuzzerTrigger(ModBus_Service.YellowAddress, true);
                    }

                    if (ConfigurationManager.AppSettings["EnableAlarm"].Equals("1"))
                    {
                        alarmMessageWindow.Show();
                    }
                }
                else
                {
                    ModBus_Service.LedBuzzerTrigger(ModBus_Service.GreenAddress, true);
                }

            }));
        }
    }
}
