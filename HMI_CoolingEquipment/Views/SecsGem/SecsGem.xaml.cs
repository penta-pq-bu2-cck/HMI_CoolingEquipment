using GEMPro300.Model;
using HMI_CoolingEquipment.Communication;
using HMI_CoolingEquipment.Models;
using HMI_CoolingEquipment.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for SecsGem.xaml
    /// </summary>
    public partial class SecsGem : UserControl
    {
        string answer = string.Empty;
        string jobID = "FATJOB0001";
        string lotID = "ZA840406G06";
        string carrierID = "TRID0005483";
        string loadportID = "AA-04-04";

        public SecsGem_VM ViewModel
        {
            get;
        }

        public SecsGem(SecsGem_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        private void InitJob_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SecsGemService.SendJobTracker(jobID, lotID, "Queued", carrierID).Wait(200);
            SecsGemService.SendJobTracker(jobID, lotID, "Selected", carrierID).Wait(200);
            SecsGemService.SendJobTracker(jobID, lotID, "WaitingForStart", carrierID).Wait(200);
        }

        private void LoadPortRTL_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SecsGemService.SendLoadPortReadyToLoad(loadportID).Wait(200);
        }

        private void LoadPortB_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SecsGemService.SendLoadPortToBlocked(lotID, carrierID, loadportID).Wait(200);
        }

        private void LoadPortRTU_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SecsGemService.SendLoadPortReadyToUnload(lotID, carrierID, loadportID).Wait(200);
            SecsGemService.SendLoadPortReadyToUnload(lotID, "1", "AA-01-01").Wait(200);
            SecsGemService.SendLoadPortReadyToUnload(lotID, "2", "AA-01-02").Wait(200);
            SecsGemService.SendLoadPortReadyToUnload(lotID, "3", "AA-01-03").Wait(200);
            SecsGemService.SendLoadPortReadyToUnload(lotID, "4", "AA-01-04").Wait(200);
            SecsGemService.SendLoadPortReadyToUnload(lotID, "5", "AA-01-05").Wait(200);
            SecsGemService.SendLoadPortReadyToUnload(lotID, "6", "AA-01-06").Wait(200);
            SecsGemService.SendLoadPortReadyToUnload(lotID, "7", "AA-01-07").Wait(200);
            SecsGemService.SendLoadPortReadyToUnload(lotID, "8", "AA-01-08").Wait(200);
            SecsGemService.SendLoadPortReadyToUnload(lotID, "9", "AA-01-09").Wait(200);
            SecsGemService.SendLoadPortReadyToUnload(lotID, "10", "AA-01-10").Wait(200);
        }
    }
}
