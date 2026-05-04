using HMI_CoolingEquipment.ViewModels;
using System.Windows;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for UserAccountEditorView.xaml
    /// </summary>
    public partial class UserAccountEditorView : Window
    {
        public UserAccountEditor_VM ViewModel
        {
            get;
        }

        public UserAccountEditorView(UserAccountEditor_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
