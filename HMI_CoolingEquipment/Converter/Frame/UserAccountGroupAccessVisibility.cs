using HMI_CoolingEquipment.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class UserAccountGroupAccessVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UserAccountModel model = value as UserAccountModel;
            Visibility visibility = Visibility.Collapsed;

            if(model != null)
            {
                if (model.UserGroup.Equals("Operator"))
                {
                    if (parameter.ToString().Equals("Normal"))
                    {
                        visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (parameter.ToString().Equals("Normal"))
                    {
                        visibility = Visibility.Visible;
                    }
                    else
                    {
                        visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                if (parameter.ToString().Equals("Normal"))
                {
                    visibility = Visibility.Visible;
                }
                else
                {
                    visibility = Visibility.Collapsed;
                }
            }

            
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
