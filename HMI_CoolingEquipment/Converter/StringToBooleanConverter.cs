using System;
using System.Globalization;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class StringToBooleanConverter : IValueConverter
    {
        #region PublicInterfaceMethods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolean = false;
            if (value == null)
            { boolean = false; }
            boolean = GlobalClassFunction.ConvertBool(value.ToString());
            return boolean;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;
            result = value.ToString();
            return result;
        }

        #endregion PublicInterfaceMethods
    }

    public class InverseStringToBooleanConverter : IValueConverter
    {
        #region PublicInterfaceMethods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolean = false;
            if (value == null)
            { boolean = true; }
            boolean = GlobalClassFunction.ConvertBool(value.ToString());
            return !boolean;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;
            bool bValue = (bool)value;
            result = (!bValue).ToString();
            return result;
        }

        #endregion PublicInterfaceMethods
    }
}