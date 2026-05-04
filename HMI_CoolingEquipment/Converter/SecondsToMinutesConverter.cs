using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class SecondsToMinutesConverter : IValueConverter
    {
        #region PublicInterfaceMethods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int minutes = 0;
            double seconds = System.Convert.ToDouble(value);
            minutes = TimeSpan.FromSeconds(seconds).Minutes;
            return minutes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion PublicInterfaceMethods
    }
}