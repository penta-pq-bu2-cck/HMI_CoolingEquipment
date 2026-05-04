using HMI_CoolingEquipment.ViewModels;
using System.Windows.Controls;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for PTL.xaml
    /// </summary>
    public partial class PTL : UserControl
    {
        public PTL_VM ViewModel
        {
            get;
        }

        public PTL(PTL_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
