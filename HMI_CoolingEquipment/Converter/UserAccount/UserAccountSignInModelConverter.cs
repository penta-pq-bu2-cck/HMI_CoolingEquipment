using HMI_CoolingEquipment.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class UserAccountSignInModelConverter : IMultiValueConverter
    {
        #region PublicInterfaceMethods

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            UserAccountSignInModel userAccountSignInModel = new UserAccountSignInModel();

            userAccountSignInModel = new UserAccountSignInModel
            {
                UserAccountSignInWindow = values[0] as Window,
                UserAccountSignInMessageTextBlock = values[1] as TextBlock,
                UserAccountUserNameTextBox = values[2] as TextBox,
                UserAccountSignInButton = values[3] as Button,
                UserAccountPasswordBox = values[4] as PasswordBox
            };

            return userAccountSignInModel;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion PublicInterfaceMethods
    }
}