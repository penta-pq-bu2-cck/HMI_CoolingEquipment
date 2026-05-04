using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HMI_CoolingEquipment.Models;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Input;
using System.Windows;
using LiveCharts.Helpers;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace HMI_CoolingEquipment.ViewModels
{
    public class Report_VM : ViewModelBase
    {
        public ICommand LoadPortCartesianChartMouseMoveCommand { get; set; }

        private double oEEHourlyUphXPointer = 0;
        public double OEEHourlyUphXPointer
        {
            get { return oEEHourlyUphXPointer; }
            set
            {
                if (oEEHourlyUphXPointer != value)
                {
                    oEEHourlyUphXPointer = value;
                    RaisePropertyChanged(nameof(OEEHourlyUphXPointer));
                }
            }
        }

        private double oEEHourlyUphYPointer = 0;
        public double OEEHourlyUphYPointer
        {
            get { return oEEHourlyUphYPointer; }
            set
            {
                if (oEEHourlyUphYPointer != value)
                {
                    oEEHourlyUphYPointer = value;
                    RaisePropertyChanged(nameof(OEEHourlyUphYPointer));
                }
            }
        }

        private string[] _labels;
        public string[] Labels
        {
            get { return _labels; }
            set
            {
                if(value != _labels)
                {
                    _labels = value;
                    RaisePropertyChanged(nameof(Labels));
                }
            }
        }

        private SeriesCollection _series = new SeriesCollection();
        public SeriesCollection Series
        {
            get { return _series; }
            set
            {
                if(value != _series)
                {
                    _series = value;
                    RaisePropertyChanged(nameof(Series));
                }
            }
        }

        private ObservableCollection<SetupJobHistoryModel> _setupJobModelHistory = new ObservableCollection<SetupJobHistoryModel>();
        public ObservableCollection<SetupJobHistoryModel> SetupJobModelHistory
        {
            get { return _setupJobModelHistory; }
            set
            {
                if (value != _setupJobModelHistory)
                {
                    _setupJobModelHistory = value;
                    RaisePropertyChanged(nameof(SetupJobModelHistory));
                }
            }
        }

        private ObservableCollection<LoadPortModel> _loadPortModels = new ObservableCollection<LoadPortModel>();
        public ObservableCollection<LoadPortModel> LoadPortModels
        {
            get { return _loadPortModels; }
            set
            {
                if (_loadPortModels != value)
                {
                    _loadPortModels = value;
                    RaisePropertyChanged(nameof(LoadPortModels));
                }
            }
        }

        public Func<double, string> OEEHourlyUphLabelFormatter
        {
            get { return x => x.ToString("N0"); }
        }


        public Report_VM()
        {
            Messenger.Default.Register<List<SetupJobHistoryModel>>(this, UpdateSetupJobHistoryModelList);
            Messenger.Default.Register<List<LoadPortModel>>(this, UpdateLoadPortModels);
            LoadPortCartesianChartMouseMoveCommand = new RelayCommand<EventArgsModel>(LoadPortCartesianChartMouseMoveCommandEvent);
        }

        private void UpdateSetupJobHistoryModelList(List<SetupJobHistoryModel> list)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SetupJobModelHistory.Clear();
                list.ForEach(x =>
                {
                    SetupJobModelHistory.Add(x);
                });
            }));
        }

        private void UpdateLoadPortModels(List<LoadPortModel> loadportmodels)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                LoadPortModels.Clear();
                Labels = new string[203];
                Series = new SeriesCollection();
                ChartValues<int> values = new ChartValues<int>();
                int i = 0;
                loadportmodels.ForEach(x =>
                {
                    LoadPortModels.Add(x);
                    Labels[i] = x.LoadPortID.ToString();
                    values.Add(int.Parse(x.LoadPortColumn));
                    i += 1;
                });

                Series.Add(new ColumnSeries
                {
                    Title = "Value",
                    Values = values,
                });
            }));
        }

        private void LoadPortCartesianChartMouseMoveCommandEvent(EventArgsModel eventArgsModel)
        {
            MouseEventArgs mouseEventArgs = eventArgsModel.EventArgs as MouseEventArgs;
            CartesianChart cartesianChart = eventArgsModel.Parameter as CartesianChart;

            Point mouseCoordinates = mouseEventArgs.GetPosition(cartesianChart);
            Point chartValues = cartesianChart.ConvertToChartValues(mouseCoordinates);

            OEEHourlyUphYPointer = chartValues.Y;

            ISeriesView series = cartesianChart.Series[0];
            if (series.ActualValues.Count <= 0)
            {
                return;
            }
            ChartPoint closestPoint = series.ClosestPointTo(chartValues.X, AxisOrientation.X);

            if (closestPoint != null)
            {
                OEEHourlyUphXPointer = closestPoint.X;
            }
        }
    }
}
