using HMI_CoolingEquipment.ViewModels;
using System;
using System.Windows.Controls;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for LoadingLoadPort.xaml
    /// </summary>
    public partial class LoadingLoadPort : UserControl
    {
        public LoadingLoadPort_VM ViewModel
        {
            get;
        }

        public LoadingLoadPort(LoadingLoadPort_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
            Loaded += LoadingBin_Loaded;
        }

        private void LoadingBin_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CarrierIDTxtBox.Focus();
        }
    }
}
