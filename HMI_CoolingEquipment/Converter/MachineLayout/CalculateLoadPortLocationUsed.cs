using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class CalculateLoadPortLocationUsed : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int returnVal = 0;
            List<LoadPortModel> LoadPortModels = value as List<LoadPortModel>;
            LoadPortModels = LoadPortModels.Where(x => x.LoadPortNodeConnectionStatus.Equals(true)).ToList();

            if (parameter.ToString().Equals("PercentTray"))
            {
                LoadPortModels = LoadPortModels.Where(x => x.LoadPortType.Equals("Tray")).ToList();
                int LoadPortLoaded = LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.Loaded)) + LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)) + LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.CoolingCompleted));
                int LoadPortTotal = LoadPortModels.Count();

                if (LoadPortLoaded > 0)
                {
                    returnVal = (int)Math.Round((double)(100 * LoadPortLoaded) / LoadPortTotal);
                }
                else
                {
                    returnVal = LoadPortLoaded;
                }
            }
            else if(parameter.ToString().Equals("LoadedTray"))
            {
                LoadPortModels = LoadPortModels.Where(x => x.LoadPortType.Equals("Tray")).ToList();
                returnVal = LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.Loaded)) + LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)) + LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.CoolingCompleted));
            }
            else if(parameter.ToString().Equals("EmptyTray"))
            {
                LoadPortModels = LoadPortModels.Where(x => x.LoadPortType.Equals("Tray")).ToList();
                returnVal = LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.Empty)) + LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.LoadProcess));
            }
            else if (parameter.ToString().Equals("PercentTube"))
            {
                LoadPortModels = LoadPortModels.Where(x => x.LoadPortType.Equals("Tube")).ToList();
                int LoadPortLoaded = LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.Loaded)) + LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)) + LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.CoolingCompleted));
                int LoadPortTotal = LoadPortModels.Count();

                if (LoadPortLoaded > 0)
                {
                    returnVal = (int)Math.Round((double)(100 * LoadPortLoaded) / LoadPortTotal);
                }
                else
                {
                    returnVal = LoadPortLoaded;
                }
            }
            else if(parameter.ToString().Equals("LoadedTube"))
            {
                LoadPortModels = LoadPortModels.Where(x => x.LoadPortType.Equals("Tube")).ToList();
                returnVal = LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.Loaded)) + LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.UnloadProcess)) + LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.CoolingCompleted));
            }
            else if(parameter.ToString().Equals("EmptyTube"))
            {
                LoadPortModels = LoadPortModels.Where(x => x.LoadPortType.Equals("Tube")).ToList();
                returnVal = LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.Empty)) + LoadPortModels.Count(x => x.LoadPortState.Equals(LoadPortState.LoadProcess));
            }

            return returnVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
