using HMI_CoolingEquipment.ViewModels;
using System.Windows.Controls;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for MachineLayout.xaml
    /// </summary>
    public partial class MachineLayout : UserControl
    {
        public MachineLayout_VM ViewModel
        {
            get;
        }

        public MachineLayout(MachineLayout_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
