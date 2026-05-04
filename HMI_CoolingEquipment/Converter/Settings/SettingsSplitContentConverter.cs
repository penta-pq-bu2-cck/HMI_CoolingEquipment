using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class SettingsSplitContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<SettingsModel> tmp_settings = value as List<SettingsModel>;

            if (tmp_settings != null)
            {
                if (parameter.Equals("Numeric"))
                {
                    tmp_settings = tmp_settings.Where(x => x.Type.Equals(SettingsType.NumericUpDown)).ToList();
                }
                else if (parameter.Equals("Toggle"))
                {
                    tmp_settings = tmp_settings.Where(x => x.Type.Equals(SettingsType.ToggleButton)).ToList();
                }
                else
                {
                    tmp_settings = tmp_settings.Where(x => x.Type.Equals(SettingsType.ComboBox)).ToList();
                }
            }

            return tmp_settings;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
