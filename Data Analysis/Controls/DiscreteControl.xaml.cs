using Data_Analysis.Class; 
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RDotNet;

namespace Data_Analysis.Controls
{
    /// <summary>
    /// Логіка взаємодії для DiscreteControl.xaml
    /// </summary>
    public partial class DiscreteControl : UserControl
    {
        DiscreteRow discreteRow;
        public List<DiscreteGrid> DiscreteGrids = new List<DiscreteGrid>();
        public SeriesCollection SeriesCollection { get; set; }
        public bool Open = false;

        public DiscreteControl()
        {
            InitializeComponent();
        }

        private void Calculate()
        {
            tbAverageValue.Text = "Середня величина: " + discreteRow.CalculateAverageValue();
            tbMode.Text = "Мода: " + discreteRow.CalculateMode();
            tbMedian.Text = "Медіана: " + discreteRow.CalculateMedia();
            tbRangeOfVariation.Text = "Розмах варіації: " + discreteRow.CalculateRangeOfVariation(); ;
            tbMeanLinearDeviation.Text = "Середнє лінійне відхилення: " + discreteRow.CalculateMeanLinearDeviation();
            tbDispersion.Text = "Дисперсія: " + discreteRow.CalculateDispersion();
            tbStandardDeviation.Text = "Середнє квадратичне відхилення: " + discreteRow.CalculateStandardDeviation();
            tbCoefficientVariation.Text = "Коефіцієнт варіації: " + discreteRow.CalculateCoefficientVariation();
            tbNormalCoefficientAsymmetry.Text = "Нормований моментний коефіцієнт асиметрії: " + discreteRow.CalculateNormalCoefficientAsymmetry();
            tbEstimationCoefficientAsymmetry.Text = "Оцінка коефіцієнта асиметрії: " + discreteRow.EstimationCoefficientAsymmetry;
            tbDegreeAsymmetry.Text = "Ступінь суттєвості асиметрії: " + discreteRow.CalculateDegreeAsymmetry();
            tbMaterialityAsymmetry.Text = "Оцінка суттєвості асиметрії: " + discreteRow.MaterialityAsymmetry;
            tbExcess.Text = "Ексцес: " + discreteRow.CalculateExcess();
            tbExcessError.Text = "Середня квадратична помилка ексцесу: " + discreteRow.CalculateExcessError();
        }

        public double[] X { get; set; }
        public double[] Y { get; set; }

        private void BuildChart()
        {
            polygon.DataContext = null;
            SeriesCollection = new SeriesCollection
            {
                new LineSeries { Values = new ChartValues<ObservablePoint>(), }
            };
            X = DiscreteGrids.Select(p => p.number).ToArray();
            Y = DiscreteGrids.Select(p => p.frequency).ToArray();
            foreach (var series in SeriesCollection)
            {
                for (var i = 0; i < X.Length; i++)
                {
                    series.Values.Add(new ObservablePoint(X[i], Y[i]));
                }
            }
            polygon.DataContext = this;
        }

        private void btCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (Open) { dialogReInit.IsOpen = true; }
            else
            {
                discreteRow = new DiscreteRow();
                discreteRow.LoadFromCSV();
                string flName = discreteRow.FLName;
                int error = discreteRow.Error;
                if (flName != "")
                {
                    if (error == 0)
                    {
                        DiscreteGrids = discreteRow.FillDataGrid();
                        discreteGrid.Visibility = Visibility.Visible;
                        polygon.Visibility = Visibility.Visible;
                        calculated.Visibility = Visibility.Visible;
                        bgStart.Visibility = Visibility.Collapsed;
                        btR.Visibility = Visibility.Visible;
                        discreteGrid.ItemsSource = DiscreteGrids;
                        Calculate();
                        BuildChart();
                        Open = true;
                    }
                    else
                    {
                        dialogReInit.IsOpen = false;
                        dialogError.IsOpen = true;
                        tbError.Text = "В " + error + " комірках є помилки";
                    }
                }
            }
        }

        private void btYes_Click(object sender, RoutedEventArgs e)
        {
            discreteRow = new DiscreteRow();
            discreteRow.LoadFromCSV();
            string flName = discreteRow.FLName;
            int error = discreteRow.Error;
            if (flName != "")
            {
                if (error == 0)
                {
                    DiscreteGrids = discreteRow.FillDataGrid();
                    discreteGrid.Visibility = Visibility.Visible;
                    polygon.Visibility = Visibility.Visible;
                    calculated.Visibility = Visibility.Visible;
                    bgStart.Visibility = Visibility.Collapsed;
                    discreteGrid.ItemsSource = DiscreteGrids;
                    Calculate();
                    BuildChart();
                    Open = true;
                }
                else
                {
                    dialogReInit.IsOpen = false;
                    dialogError.IsOpen = true;
                    tbError.Text = "В " + error + " комірках є помилки";
                }
            }
            dialogReInit.IsOpen = false;
        }

        private void btNo_Click(object sender, RoutedEventArgs e)
        {
            dialogReInit.IsOpen = false;
        }

        private void btOkay_Click(object sender, RoutedEventArgs e)
        {
            dialogError.IsOpen = false;
        }

        private void BtR_OnClick(object sender, RoutedEventArgs e)
        {
            if (Open)
            {
                try
                {
                    REngine.SetEnvironmentVariables("E:\\R-4.2.1\\R-4.2.1\\bin", "E:\\R-4.2.1\\R-4.2.1");
                    REngine engine = REngine.GetInstance();
                    engine.Initialize();
                    engine.SetSymbol("x", engine.CreateNumericVector(X));
                    engine.SetSymbol("y", engine.CreateNumericVector(Y));
                    engine.Evaluate("plot(x, y, type = 'l', main = 'Дискретний варіаційний ряд')");
                }
                catch
                {
                    MessageBox.Show("RGUI.exe не знайдено :(", "Помилка");
                }
            }
        }
    }
}