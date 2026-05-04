using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HMI_CoolingEquipment
{
    public class LoadPortStateToInterfaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Tuple<LoadPortState, bool, bool, bool> tuple = value as Tuple<LoadPortState, bool, bool, bool>;
            object returnVal = null;

            if (parameter.ToString().Equals("Colour"))
            {
                returnVal = Brushes.LightGray;

                if (!tuple.Item2)
                {
                    //switch (tuple.Item1)
                    //{
                    //    case LoadPortState.Empty:
                    //        returnVal = Brushes.Transparent;
                    //        break;
                    //    case LoadPortState.CoolingCompleted:
                    //        returnVal = Brushes.Blue;
                    //        break;
                    //    case LoadPortState.Loaded:
                    //        returnVal = Brushes.Green;
                    //        break;
                    //    case LoadPortState.LoadProcess:
                    //        returnVal = Brushes.GreenYellow;
                    //        break;
                    //    case LoadPortState.UnloadProcess:
                    //        returnVal = Brushes.LightGreen;
                    //        break;
                    //    case LoadPortState.Disabled:
                    //        returnVal = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFBF00");
                    //        break;
                    //}

                    returnVal = Brushes.DarkGray;
                }
            }
            else
            {
                returnVal = tuple.Item2;
            }
            return returnVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
