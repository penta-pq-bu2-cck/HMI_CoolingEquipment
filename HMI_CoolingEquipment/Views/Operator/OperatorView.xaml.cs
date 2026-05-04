using HMI_CoolingEquipment.ViewModels;
using System.Windows.Controls;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for OperatorView.xaml
    /// </summary>
    public partial class OperatorView : UserControl
    {
        public Operator_VM ViewModel
        {
            get;
        }

        public OperatorView(Operator_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
