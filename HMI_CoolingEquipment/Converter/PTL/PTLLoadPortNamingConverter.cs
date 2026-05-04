using System;
using System.Globalization;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class PTLLoadPortNamingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string returnValue = string.Empty;

            returnValue = $"({values[0]}) - {values[1]} [{values[2]}]";

            return returnValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
