using HMI_CoolingEquipment.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for UnloadJobDetailsWindow.xaml
    /// </summary>
    public partial class UnloadJobDetailsWindow : Window
    {
        public UnloadJobDetails_VM ViewModel
        {
            get;
        }

        public UnloadJobDetailsWindow(UnloadJobDetails_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
