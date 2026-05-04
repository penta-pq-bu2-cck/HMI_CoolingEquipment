using HMI_CoolingEquipment.ViewModels;
using HMI_CoolingEquipment.Views;
using System.Diagnostics;
using System;
using System.Windows;
using System.Linq;
using GEMPro300.Model;
using GalaSoft.MvvmLight.Messaging;
using HMI_CoolingEquipment.Models;

namespace HMI_CoolingEquipment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (Process.GetProcesses().Where(x => x.ProcessName == Process.GetCurrentProcess().ProcessName).Count() > 1)
            {
                MessageBox.Show(string.Format("{0} Already an application is running...",
                       Environment.NewLine), "Warning", MessageBoxButton.OK,
                       MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                Current.Shutdown();
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                Frame frame = new Frame(new Frame_VM());
                frame.Show();
            }
        }
    }
}
