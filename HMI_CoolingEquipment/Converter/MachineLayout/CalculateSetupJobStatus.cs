using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class CalculateSetupJobStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int countingLots = 0;
            List<SetupJobModel> model = value as List<SetupJobModel>;

            switch (parameter)
            {
                case "RunningLots":
                    countingLots = model.Count(x => !x.JobStatus.Equals(JobStatus.Initialize) && !x.JobStatus.Equals(JobStatus.Complete));
                    break;
                case "OpenLots":
                    countingLots = model.Count(x => x.JobStatus.Equals(JobStatus.Initialize));
                    break;
            }

            return countingLots;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
