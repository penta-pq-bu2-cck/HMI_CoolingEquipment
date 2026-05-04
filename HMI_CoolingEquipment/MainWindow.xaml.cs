using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.ViewModels;
using HMI_CoolingEquipment.Views;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace HMI_CoolingEquipment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //MachineLayout_VM machineLayout_VM = new MachineLayout_VM();

        public MainWindow()
        {
            InitializeComponent();
            //Test.Content = new MachineLayout(machineLayout_VM);
        }

        private void Init_Button_Click(object sender, RoutedEventArgs e)
        {
            //SecsGemService.initializeSecsGem();
            //machineLayout_VM.MachineLayoutModels.MachineLayoutTray.ForEach(x =>
            //{
            //    if (x.Key.Equals("9"))
            //    {
            //        x.MachineLayoutItemsList.ForEach(y =>
            //        {
            //            if(y.BackgroundColour.Equals(Brushes.Black))
            //            y.BackgroundColour = Brushes.Wheat;
            //        });
            //    }
            //});
            //machineLayout_VM.MachineLayoutModels.MachineLayoutTray.SelectMany(x => x.MachineLayoutItemsList).ToList().Find(x => x.AddressModel.Tag.Equals("A12")).BackgroundColour = Brushes.DeepPink;
        }

        private void Open_Button_Click(object sender, RoutedEventArgs e)
        {
            SecsGemService.OpenSettingsSecsGem();
        }

        private void Disconnect_Button_Click(object sender, RoutedEventArgs e)
        {
            SecsGemService.DisconnectSecsGem();
        }

        private void MachineStatus_Button_Click(object sender, RoutedEventArgs e)
        {
            //SecsGemService.SendEventSecsGem(152, "Production", 154);
        }
    }
}
