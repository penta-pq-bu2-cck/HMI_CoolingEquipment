using HMI_CoolingEquipment.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class UnloadingSetupJobModelConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            UnloadingSetupJobModel model = new UnloadingSetupJobModel();

            model.UnloadingLotDetailsWindow = values[0] as Window;
            model.setupJobModel = values[1] as SetupJobModel;

            return model;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
