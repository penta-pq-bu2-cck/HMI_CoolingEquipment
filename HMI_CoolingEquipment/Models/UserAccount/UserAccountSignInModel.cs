using System.Windows;
using System.Windows.Controls;

namespace HMI_CoolingEquipment.Models
{
    public class UserAccountSignInModel
    {
        public Window UserAccountSignInWindow { get; set; }

        public TextBlock UserAccountSignInMessageTextBlock { get; set; }

        public TextBox UserAccountUserNameTextBox { get; set; }

        public Button UserAccountSignInButton { get; set; }

        public PasswordBox UserAccountPasswordBox { get; set; }
    }
}
