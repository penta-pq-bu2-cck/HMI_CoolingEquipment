using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    internal class PTLConnectionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           bool returnBool = false;

            if (int.Parse(value.ToString()).Equals(7))
            {
                returnBool = true;
            }

            switch (parameter.ToString())
            {
                case "Normal":
                    break;
                case "Inverse":
                    returnBool = !returnBool;
                    break;
            }

            return returnBool;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
