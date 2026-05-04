using HMI_CoolingEquipment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HMI_CoolingEquipment.Views.LotStatus
{
    /// <summary>
    /// Interaction logic for LotStatus.xaml
    /// </summary>
    public partial class LotStatus : UserControl
    {
        public LotStatus_VM ViewModel
        {
            get;
        }

        public LotStatus(LotStatus_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
            Loaded += LotStatus_Loaded;
        }

        private void LotStatus_Loaded(object sender, RoutedEventArgs e)
        {
            LotStatusTabControl.SelectedIndex = 0;
        }

        private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.FindJobHistoryCommandEvent();
        }
    }
}
