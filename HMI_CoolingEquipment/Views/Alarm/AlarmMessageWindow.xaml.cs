using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.ViewModels;
using System;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for AlarmMessageWindow.xaml
    /// </summary>
    public partial class AlarmMessageWindow : Window
    {
        public AlarmMessage_VM ViewModel
        {
            get;
        }

        public AlarmMessageWindow(AlarmMessage_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
            IsVisibleChanged += AlarmMessageWindow_IsVisibleChanged;
        }

        private void AlarmMessageWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                AlarmStatusTabControl.SelectedIndex = 0;
            }
        }

        private void SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ViewModel.FindAlarmHistoryCommandEvent();
        }
    }
}
