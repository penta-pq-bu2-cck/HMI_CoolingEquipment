using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for WaitingForHostWindow.xaml
    /// </summary>
    public partial class WaitingForHostWindow : Window
    {
        public WaitingForHostWindow(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }
    }
}
