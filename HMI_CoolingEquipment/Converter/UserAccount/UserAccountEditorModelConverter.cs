using HMI_CoolingEquipment.Models;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class UserAccountEditorModelConverter : IMultiValueConverter
    {
        #region PublicInterfaceMethods

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            UserAccountEditorModel userAccountEditorModel = new UserAccountEditorModel();

            userAccountEditorModel = new UserAccountEditorModel
            {
                UserAccountUserNameTextBox = values[0] as TextBox,
                UserAccountFirstNameTextBox = values[1] as TextBox,
                UserAccountLastNameTextBox = values[2] as TextBox,
                UserAccountPasswordTextBox = values[3] as TextBox,
                UserAccountGroupKeyComboBox = values[4] as ComboBox,
                UserAccountListView = values[5] as ListView
            };

            return userAccountEditorModel;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion PublicInterfaceMethods
    }
}