using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class GetCarrierListForDisplay : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retval = string.Empty;

            switch (parameter.ToString())
            {
                case "Current":
                    SetupJobModel job = value as SetupJobModel;

                    using (var context = new DB_Context())
                    {
                        List<JOBDETAILS> carriers = context.JOBDETAILS.ToList().Where(x => x.JobID.Equals(job.JobID) && x.LotNo.Equals(job.LotNo)).ToList();

                        if (carriers.Count() > 0)
                        {
                            int i = 1;
                            carriers.ForEach(x =>
                            {
                                retval += $"{i}. {x.CarrierID}({x.CarrierStatus})\n";
                                i += 1;
                            });

                            int index = retval.LastIndexOf('\n');
                            retval = retval.Remove(index, 1);
                        }
                    }
                    break;
                case "History":
                    List<JobDetailsHistory> carriersList = value as List<JobDetailsHistory>;
                    if (carriersList.Count() > 0)
                    {
                        int i = 1;
                        carriersList.ForEach(x =>
                        {
                            retval += $"{i}. {x.CarrierID}\n";
                            i += 1;
                        });

                        int index = retval.LastIndexOf('\n');
                        retval = retval.Remove(index, 1);
                    }
                    break;
            }

            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
