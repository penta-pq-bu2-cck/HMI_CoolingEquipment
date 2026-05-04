using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace HMI_CoolingEquipment
{
    public class FrameBorderAlarmColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush colour = Brushes.Red;
            List<AlarmCurrentModel> alarmCurrentModels = value as List<AlarmCurrentModel>;

            if(alarmCurrentModels.Count() > 0)
            {
                if (alarmCurrentModels.Count(x => x.AlarmType.Equals(AlarmType.Error)) > 0)
                {
                    colour = Brushes.Red;
                }
                else if (alarmCurrentModels.Count(x => x.AlarmType.Equals(AlarmType.Warning)) > 0)
                {
                    colour = Brushes.Yellow;
                }
            }
            else
            {
                colour = (SolidColorBrush)new BrushConverter().ConvertFrom("#4caf50");
            }

            return colour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
