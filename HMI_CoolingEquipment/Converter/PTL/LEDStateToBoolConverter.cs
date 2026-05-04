using System;
using System.Globalization;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class LEDStateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LEDState val = (LEDState)value;

            bool retVal;
            switch (val)
            {
                case LEDState.Blinking:
                    retVal = true;
                    break;
                default:
                    retVal = false;
                    break;
            }

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            LEDState retVal;
            if (val)
            {
                retVal = LEDState.Blinking;
            }
            else
            {
                retVal = LEDState.NoBlinking;
            }

            return retVal;
        }
    }
}
