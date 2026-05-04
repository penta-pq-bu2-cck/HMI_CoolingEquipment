using HMI_CoolingEquipment.ViewModels;
using System.Windows;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for SecsGemCusEventWindow.xaml
    /// </summary>
    public partial class SecsGemCusEventWindow : Window
    {
        public SecsGemCusEvent_VM ViewModel
        {
            get;
        }

        public SecsGemCusEventWindow(SecsGemCusEvent_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
            Loaded += ViewModel.SecsGemCusEventWindow_Loaded;
        }
    }
}
