using GalaSoft.MvvmLight;
using System.Windows.Controls;

namespace HMI_CoolingEquipment.Models
{
    public class UserAccountEditorModel : ViewModelBase
    {
        public TextBox UserAccountUserNameTextBox { get; set; }

        public TextBox UserAccountFirstNameTextBox { get; set; }

        public TextBox UserAccountLastNameTextBox { get; set; }

        public TextBox UserAccountPasswordTextBox { get; set; }

        public ComboBox UserAccountGroupKeyComboBox { get; set; }

        public ListView UserAccountListView { get; set; }
    }
}
