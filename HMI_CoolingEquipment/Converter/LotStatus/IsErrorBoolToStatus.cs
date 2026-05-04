using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class IsErrorBoolToStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object status = string.Empty;

            switch (parameter.ToString())
            {
                case "Normal":
                    switch ((bool)value)
                    {
                        case false:
                            status = "OK";
                            break;
                        case true:
                            status = "Error";
                            break;
                    }
                    break;
                case "Number":
                    switch ((bool)value)
                    {
                        case false:
                            status = 1;
                            break;
                        case true:
                            status = 0;
                            break;
                    }
                    break;
            }

            return status;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
