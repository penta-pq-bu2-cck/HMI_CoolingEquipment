using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HMI_CoolingEquipment.Models
{
    public class MenuModel : ViewModelBase
    {
        public ICommand MenuCommand { get; set; }

        private object menuSelectedItem = string.Empty;
        public object MenuSelectedItem
        {
            get { return menuSelectedItem; }
            set
            {
                if(menuSelectedItem != value)
                {
                    menuSelectedItem = value;
                    RaisePropertyChanged(nameof(MenuSelectedItem));
                }
            }
        }

        private object menuSelectedIndex = string.Empty;
        public object MenuSelectedIndex
        {
            get { return menuSelectedIndex; }
            set
            {
                if (menuSelectedIndex != value)
                {
                    menuSelectedIndex = value;
                    RaisePropertyChanged(nameof(MenuSelectedIndex));
                }
            }
        }

        private ObservableCollection<MenuItemModel> _menuModels = new ObservableCollection<MenuItemModel>();
        public ObservableCollection<MenuItemModel> MenuModels
        {
            get { return _menuModels; }
            set
            {
                if (value != _menuModels)
                {
                    _menuModels = value;
                    RaisePropertyChanged(nameof(MenuModels));
                }
            }
        }
    }
}
