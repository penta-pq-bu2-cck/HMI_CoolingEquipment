using HMI_CoolingEquipment.ViewModels;
using PQ.HMI;
using System.Windows;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for Frame.xaml
    /// </summary>
    public partial class Frame : Window
    {
        public Frame_VM ViewModel
        {
            get;
        }

        public Frame(Frame_VM viewmodel)
        {
            ViewModel = viewmodel;
            DataContext = this;
            InitializeComponent();
            Closing += ViewModel.OnWindowClosing;
        }
    }
}
