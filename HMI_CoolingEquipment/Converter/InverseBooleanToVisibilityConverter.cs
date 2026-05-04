using NLog;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI_CoolingEquipment
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        #region PrivateFields

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion PrivateFields

        #region PublicInterfaceMethods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Visible;

            try
            {
                visibility = System.Convert.ToBoolean(value) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex) { logger.Error(ex); }

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion PublicInterfaceMethods
    }
}