// <copyright file="MainWindow.xaml.cs" company="Mercedes-Benz Grand Prix Limited">
// Copyright (c) Mercedes-Benz Grand Prix Limited. All rights reserved.
// </copyright>

namespace PlottingTestApp
{
    using ModernThemables.Charting.Interfaces;
    using ModernThemables.Charting.Models;
    using ModernThemables.Charting.Models.Brushes;
    using ModernThemables.Charting.Models.CartesianChart;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow ()
        {
            var cts = new CancellationTokenSource();
            Application.Current.Dispatcher.ShutdownStarted += ( sender, e ) => cts.Cancel();

            _ = Task.Run(() =>
            {
                var random = new Random();
                var series = new Series()
                {
                    Stroke = new SolidBrush(Colors.Black)
                };
                var data = new ObservableCollection<ISeries>() { series };
                Application.Current.Dispatcher.Invoke(() => this.LineChart.Series = data);
                while (!cts.Token.IsCancellationRequested)
                {
                    series.Values.Add(new DateTimePoint(DateTime.Now, random.NextDouble()));
                    if (series.Values.Count > 200)
                    {
                        series.Values.RemoveAt(0);
                    }
                    Thread.Sleep(16);
                }
            });
        }
    }
}
