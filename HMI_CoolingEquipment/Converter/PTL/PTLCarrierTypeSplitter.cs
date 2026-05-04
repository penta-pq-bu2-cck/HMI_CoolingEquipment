using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class PTLCarrierTypeSplitter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<LoadPortModel> models = value as List<LoadPortModel>;

            switch (parameter.ToString())
            {
                case "Tube":
                    models = models.Where(x => x.LoadPortType.Equals("Tube")).ToList();
                    break;
                case "Tray":
                    models = models.Where(x => x.LoadPortType.Equals("Tray")).ToList();
                    break;
            }

            return models;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
