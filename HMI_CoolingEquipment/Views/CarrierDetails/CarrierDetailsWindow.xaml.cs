using HMI_CoolingEquipment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for CarrierDetailsWindow.xaml
    /// </summary>
    public partial class CarrierDetailsWindow : Window
    {
        public CarrierDetails_VM ViewModel
        {
            get;
        }

        public CarrierDetailsWindow(CarrierDetails_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
