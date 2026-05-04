using HMI_CoolingEquipment.ViewModels;
using System.Windows;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for SecsGemCusVarWindow.xaml
    /// </summary>
    public partial class SecsGemCusVarWindow : Window
    {
        public SecsGemCusVar_VM ViewModel
        {
            get;
        }

        public SecsGemCusVarWindow(SecsGemCusVar_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            Loaded += ViewModel.SecsGemCusVarWindow_Loaded;
            InitializeComponent();
        }
    }
}
