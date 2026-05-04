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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for AlarmView.xaml
    /// </summary>
    public partial class AlarmView : UserControl
    {
        public Alarm_VM ViewModel
        {
            get;
        }

        public AlarmView()
        {
            ViewModel = new Alarm_VM();
            DataContext = this;
            InitializeComponent();
        }
    }
}
