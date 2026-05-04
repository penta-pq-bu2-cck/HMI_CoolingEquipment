using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class SecondsToDaysConverter : IValueConverter
    {
        #region PublicInterfaceMethods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int days = 0;
            double seconds = System.Convert.ToDouble(value);
            days = TimeSpan.FromSeconds(seconds).Days;
            return days;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion PublicInterfaceMethods
    }
}