using GalaSoft.MvvmLight.Command;
using HMI_CoolingEquipment.Models;

namespace HMI_CoolingEquipment
{
    public class EventArgsConverter : IEventArgsConverter
    {
        #region PublicInterfaceMethods

        public object Convert(object value, object parameter)
        {
            EventArgsModel eventArgsModel = new EventArgsModel();

            eventArgsModel.EventArgs = value;
            eventArgsModel.Parameter = parameter;

            return eventArgsModel;
        }

        #endregion PublicInterfaceMethods
    }
}