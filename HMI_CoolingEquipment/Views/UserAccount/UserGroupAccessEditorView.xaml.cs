using HMI_CoolingEquipment.ViewModels;
using System.Windows;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for UserGroupAccessEditorView.xaml
    /// </summary>
    public partial class UserGroupAccessEditorView : Window
    {
        public UserGroupAccessEditor_VM ViewModel
        {
            get;
        }

        public UserGroupAccessEditorView(UserGroupAccessEditor_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
