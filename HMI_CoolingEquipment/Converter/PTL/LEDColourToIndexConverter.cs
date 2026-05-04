using System;
using System.Globalization;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class LEDColourToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (LEDColour)value;
        }
    }
}
