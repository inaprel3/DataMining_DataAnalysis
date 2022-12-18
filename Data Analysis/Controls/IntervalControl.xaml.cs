using Data_Analysis.Class;
using LiveCharts;
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
    public partial class IntervalControl : UserControl
    {
        IntervalRow intervalRow;
        public List<IntervalGrid> IntervalGrid = new List<IntervalGrid>();
        public string[] Labels { get; set; }
        public bool Open = false;
        public IntervalControl()
        {
            InitializeComponent();
        }

        private double[] x;
        private double[] y;

        private void BuildChart()
        {
            barChart.DataContext = null;
            y = IntervalGrid.Select(p => p.frequency).ToArray();
            x = IntervalGrid.Select(p => p.leftBorder).ToArray();
            Labels = new string[x.Length];
            barChart.Series = new SeriesCollection
            {
                new ColumnSeries { Values = new ChartValues<double>(y) }
            };
            for (int i = 0; i < x.Length; i++)
            {
                Labels[i] = x[i].ToString();
            }
            barChart.DataContext = this;
        }

        private void Calculate()
        {
            tbAverageValue.Text = "Середня величина: " + intervalRow.CalculateAverageValue();
            tbMode.Text = "Мода: " + intervalRow.CalculateMode();
            tbMedian.Text = "Медіана: " + intervalRow.CalculateMedia();
            tbRangeOfVariation.Text = "Розмах варіації: " + intervalRow.CalculateRangeOfVariation(); ;
            tbMeanLinearDeviation.Text = "Середнє лінійне відхилення: " + intervalRow.CalculateMeanLinearDeviation();
            tbDispersion.Text = "Дисперсія: " + intervalRow.CalculateDispersion();
            tbStandardDeviation.Text = "Середнє квадратичне відхилення: " + intervalRow.CalculateStandardDeviation();
            tbCoefficientVariation.Text = "Коефіцієнт варіації: " + intervalRow.CalculateCoefficientVariation();
            tbNormalCoefficientAsymmetry.Text = "Нормований моментний коефіцієнт асиметрії: " + intervalRow.CalculateNormalCoefficientAsymmetry();
            tbEstimationCoefficientAsymmetry.Text = "Оцінка коефіцієнта асиметрії: " + intervalRow.EstimationCoefficientAsymmetry;
            tbDegreeAsymmetry.Text = "Ступінь суттєвості асиметрії: " + intervalRow.CalculateDegreeAsymmetry();
            tbMaterialityAsymmetry.Text = "Оцінка суттєвості асиметрії: " + intervalRow.MaterialityAsymmetry;
            tbExcess.Text = "Ексцес: " + intervalRow.CalculateExcess();
            tbExcessError.Text = "Середня квадратична помилка ексцесу: " + intervalRow.CalculateExcessError();
        }

        private void btYes_Click(object sender, RoutedEventArgs e)
        {
            intervalRow = new IntervalRow();
            intervalRow.LoadFromCSV();
            string flName = intervalRow.FLName;
            int error = intervalRow.Error;
            if (flName != "")
            {
                if (error == 0)
                {
                    discreteGrid.Visibility = Visibility.Visible;
                    calculated.Visibility = Visibility.Visible;
                    bgStart2.Visibility = Visibility.Collapsed;
                    IntervalGrid = intervalRow.FillDataGridTwo();
                    discreteGrid.ItemsSource = IntervalGrid;
                    Calculate();
                    BuildChart();
                }
                else
                {
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

        private void btCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (Open) { dialogReInit.IsOpen = true; }
            else
            {
                intervalRow = new IntervalRow();
                intervalRow.LoadFromCSV();
                string flName = intervalRow.FLName;
                int error = intervalRow.Error;
                if (flName != "")
                {
                    if (error == 0)
                    {
                        discreteGrid.Visibility = Visibility.Visible;
                        calculated.Visibility = Visibility.Visible;
                        bgStart2.Visibility = Visibility.Collapsed;
                        IntervalGrid = intervalRow.FillDataGridTwo();
                        btR.Visibility = Visibility.Visible;
                        discreteGrid.ItemsSource = IntervalGrid;
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
                    engine.SetSymbol("x", engine.CreateNumericVector(x));
                    engine.SetSymbol("y", engine.CreateNumericVector(y));
                    engine.Evaluate("plot(x, y, type = 'h', main = 'Інтервальний варіаційний ряд')");
                }
                catch
                {
                    MessageBox.Show("RGUI.exe не знайдено :(", "Помилка");
                }
            }
        }
    }
}