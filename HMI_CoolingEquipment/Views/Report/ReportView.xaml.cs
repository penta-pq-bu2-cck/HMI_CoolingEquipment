using HMI_CoolingEquipment.ViewModels;
using LiveCharts.Wpf;
using LiveCharts;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView : UserControl
    {
        public Report_VM ViewModel
        {
            get;
        }
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public ReportView(Report_VM viewModel)
        {
            ViewModel = viewModel;
            //Labels = viewModel.Labels;

            //SeriesCollection = new SeriesCollection
            //{
            //    new ColumnSeries
            //    {
            //        Title = "2015",
            //        Values = new ChartValues<double> { 10, 50, 39, 50 }
            //    }
            //};

            ////adding series will update and animate the chart automatically
            //SeriesCollection.Add(new ColumnSeries
            //{
            //    Title = "2016",
            //    Values = new ChartValues<double> { 11, 56, 42 }
            //});

            ////also adding values updates and animates the chart automatically
            //SeriesCollection[1].Values.Add(48d);

            ////Labels = new[] { "Maria", "Susan", "Charles", "Frida" };
            //Formatter = value => value.ToString("N");

            //DataContext = this;

            DataContext = this;
            InitializeComponent();
        }
    }
}
