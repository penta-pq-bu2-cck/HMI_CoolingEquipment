using HMI_CoolingEquipment.ViewModels;
using System.Windows.Controls;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for UnloadingBin.xaml
    /// </summary>
    public partial class UnloadingLoadPort : UserControl
    {
        public UnloadingLoadPort_VM ViewModel
        {
            get;
        }

        public UnloadingLoadPort(UnloadingLoadPort_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
            Loaded += UnloadingBin_Loaded;
            ViewModel.txtBox = LotIDTxtBox;
        }

        private void UnloadingBin_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LotIDTxtBox.Focus();
            ViewModel.UnloadPortLocationLogs.Clear();
        }
    }
}
