using GalaSoft.MvvmLight;
using System.Windows.Controls;

namespace HMI_CoolingEquipment.Models
{
    public class MenuItemModel : ViewModelBase
    {
        private ContentControl menuContent;

        private string menuImage = string.Empty;

        private string menuName = string.Empty;

        private string menuKey = string.Empty;

        public ContentControl MenuContent
        {
            get
            {
                return menuContent;
            }
            set
            {
                if (menuContent != value)
                {
                    menuContent = value;
                    RaisePropertyChanged(nameof(MenuContent));
                }
            }
        }

        public string MenuImage
        {
            get
            {
                return menuImage;
            }
            set
            {
                if (menuImage != value)
                {
                    menuImage = value;
                    RaisePropertyChanged(nameof(MenuImage));
                }
            }
        }

        public string MenuName
        {
            get
            {
                return menuName;
            }
            set
            {
                if (menuName != value)
                {
                    menuName = value;
                    RaisePropertyChanged(nameof(MenuName));
                }
            }
        }

        public string MenuKey
        {
            get
            {
                return menuKey;
            }
            set
            {
                if (menuKey != value)
                {
                    menuKey = value;
                    RaisePropertyChanged(nameof(MenuKey));
                }
            }
        }
    }
}
