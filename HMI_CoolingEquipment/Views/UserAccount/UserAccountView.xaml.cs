using HMI_CoolingEquipment.ViewModels;
using System.Windows.Controls;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for UserAccountView.xaml
    /// </summary>
    public partial class UserAccountView : UserControl
    {
        public UserAccount_VM ViewModel
        {
            get;
        }

        public UserAccountView()
        {
            ViewModel = new UserAccount_VM();
            DataContext = this;
            InitializeComponent();
        }
    }
}
