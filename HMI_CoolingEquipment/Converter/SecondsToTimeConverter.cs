using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class SecondsToTimeConverter : IValueConverter
    {
        #region PublicInterfaceMethods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string time = string.Empty;

            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                double seconds = System.Convert.ToDouble(value);

                TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
                if (timeSpan.Days > 0)
                {
                    time = $"{timeSpan.Days} Days {timeSpan.Hours} Hours {timeSpan.Minutes} Minutes {timeSpan.Seconds} Seconds";
                }
                else if (timeSpan.Hours > 0)
                {
                    time = $"{timeSpan.Hours} Hours {timeSpan.Minutes} Minutes {timeSpan.Seconds} Seconds";
                }
                else if (timeSpan.Minutes > 0)
                {
                    time = $"{timeSpan.Minutes} Minutes {timeSpan.Seconds} Seconds";
                }
                else
                {
                    time = $"{timeSpan.Seconds} Seconds";
                }
            }

            return time;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion PublicInterfaceMethods
    }
}