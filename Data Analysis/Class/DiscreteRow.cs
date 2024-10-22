﻿//Дискретний варіаційний ряд
using Data_Analysis.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Data_Analysis
{
    interface IRowDiscreate
    {
        double CalculateAverageValue();
        int CalculateMode();
        double CalculateMedia();
        double CalculateRangeOfVariation();
        double CalculateMeanLinearDeviation();
        double CalculateDispersion();
        double CalculateStandardDeviation();
        double CalculateCoefficientVariation();
        double CalculateNormalCoefficientAsymmetry();
        double CalculateDegreeAsymmetry();
        double CalculateExcess();
        string CalculateExcessError();
    }

    class DiscreteRow : IRowDiscreate
    {
        public List<string> RowList = new List<string>();
        public List<DiscreteGrid> DiscreteGrids = new List<DiscreteGrid>();

        public string EstimationCoefficientAsymmetry = "",
            MaterialityAsymmetry = "",
            ExcessErrorString = "",
            FLName = "";

        double AverageValue = 0,
            Dispersion = 0,
            StandardDeviation = 0,
            CoefficientVariation = 0,
            NormalCoefficientAsymmetry = 0,
            Excess = 0;

        public int Error = 0;

        /*Метод для заповнення даними з CSV файлу списку RowList*/
        public List<string> LoadFromCSV()
        {
            Error = 0;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV файл (*.csv)|*.csv";
            ofd.FileName = "";
            ofd.Title = "Відкрити";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string str;
                using (var R = new StreamReader(ofd.FileName))
                {
                    FLName = ofd.FileName;
                    while ((str = R.ReadLine()) != null)
                    {
                        String[] array = str.Split(new char[] { '.' }); ///роздільник 
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i] != "")
                            {
                                try
                                {
                                    Convert.ToDouble(array[i]);
                                    RowList.Add(array[i]);
                                }
                                catch { Error++; }
                            }
                        }
                    }
                    if (Error != 0) { }
                    else
                    {
                        for (int i = 0; i < RowList.Count; i++)
                        {
                            for (int j = 0; j < RowList.Count - 1; j++)
                            {
                                if (Convert.ToDouble(RowList[j]) > Convert.ToDouble(RowList[j + 1]))
                                {
                                    string t = RowList[j];
                                    (RowList[j]) = (RowList[j + 1]);
                                    RowList[j + 1] = t;
                                }
                            }
                        }
                    }
                }
            }
            return RowList;
        }

        /*Метод для заповнення списку DiscreteGrid, для подальшого заповнення DataGrid в DiscreteControl*/
        public List<DiscreteGrid> FillDataGrid()
        {
            try
            {
                int accumulatedFrequency = 0;
                for (int i = 0; i < RowList.Count; i++)
                {
                    double number = Convert.ToDouble(RowList[i]);
                    int frequency = 1;
                    for (int j = i + 1; j < RowList.Count; j++)
                    {
                        if (number == Convert.ToDouble(RowList[j]))
                        {
                            frequency++;
                            i++;
                        }
                        else { break; }
                    }
                    accumulatedFrequency += frequency;
                    DiscreteGrids.Add(new DiscreteGrid { 
                        number = number, 
                        frequency = frequency, 
                        accumulatedFrequency = accumulatedFrequency 
                    });
                }
                return DiscreteGrids;
            }
            catch { return DiscreteGrids; }
        }

        /*Метод для пошуку середньої величини*/
        public virtual double CalculateAverageValue()
        {
            try
            {
                AverageValue = 0;
                double[] num = DiscreteGrids.Select(p => p.number).ToArray();
                double[] freq = DiscreteGrids.Select(p => p.frequency).ToArray();
                for (int i = 0; i < DiscreteGrids.Count - 1; i++)
                {
                    AverageValue += Convert.ToDouble(num[i]) * Convert.ToDouble(freq[i]);
                }
                AverageValue = AverageValue / RowList.Count;
                return Math.Round(AverageValue, 2);
            }
            catch
            {
                return Math.Round(AverageValue, 2);
            }
        }

        /*Метод для пошуку моди*/
        public int CalculateMode()
        {
            int frequency = 0, Mode = 0;
            try
            {
                double[] num = DiscreteGrids.Select(p => p.number).ToArray();
                double[] freq = DiscreteGrids.Select(p => p.frequency).ToArray();
                for (int i = 0; i < DiscreteGrids.Count - 1; i++)
                {
                    if (frequency < Convert.ToDouble(freq[i]))
                    {
                        frequency = Convert.ToInt32(freq[i]);
                        Mode = Convert.ToInt32(num[i]);
                    }
                }
                return Mode;
            }
            catch { return Mode; }
        }

        /*Метод для пошуку медіани*/
        public virtual double CalculateMedia()
        {
            double Median = 0;
            try
            {
                int N = 0;
                for (int i = 0; i < DiscreteGrids.Count - 1; i++)
                {
                    if (RowList.Count % 2 == 0) { N = (RowList.Count) / 2; }
                    else { N = (RowList.Count + 1) / 2; }
                    Median = Convert.ToDouble(RowList[N]);
                }
                return Math.Round(Median, 2);
            }
            catch { return Math.Round(Median, 2); }
        }

        /*Метод для пошуку размаху варіації*/
        public double CalculateRangeOfVariation()
        {
            double RangeOfVariation = 0;
            try
            {
                Double Xmax = Convert.ToDouble(RowList[0]), Xmin = Convert.ToDouble(RowList[0]);
                for (int i = 0; i < RowList.Count; i++)
                {
                    if (Xmax < Convert.ToDouble(RowList[i]))
                    {
                        Xmax = Convert.ToDouble(RowList[i]);
                    }
                    if (Xmin > Convert.ToDouble(RowList[i]))
                    {
                        Xmin = Convert.ToDouble(RowList[i]);
                    }
                }
                RangeOfVariation = Xmax - Xmin;
                return Math.Round(RangeOfVariation, 2);
            }
            catch { return Math.Round(RangeOfVariation, 2); }
        }

        /*Метод для пошуку середнього лінійного відхилення*/
        public double CalculateMeanLinearDeviation()
        {
            double MeanLinearDeviation = 0;
            try
            {
                for (int i = 0; i < RowList.Count; i++)
                {
                    MeanLinearDeviation = Math.Abs(Convert.ToDouble(RowList[i]) - AverageValue);
                }
                MeanLinearDeviation = MeanLinearDeviation / RowList.Count;
                return Math.Round(MeanLinearDeviation, 2);
            }
            catch { return Math.Round(MeanLinearDeviation, 2); }
        }

        /*Метод для пошуку дисперсії*/
        public double CalculateDispersion()
        {
            try
            {
                Dispersion = 0;
                for (int i = 0; i < RowList.Count; i++)
                {
                    Dispersion += Math.Pow((Convert.ToDouble(RowList[i]) - AverageValue), 2);
                }
                Dispersion = Dispersion / RowList.Count;
                return Math.Round(Dispersion, 2);
            }
            catch { return Math.Round(Dispersion, 2); }
        }

        /*Метод для пошуку середнього квадратичного відхилення*/
        public double CalculateStandardDeviation()
        {
            try
            {
                StandardDeviation = 0;
                StandardDeviation = Math.Sqrt(Dispersion);
                return Math.Round(StandardDeviation, 2);
            }
            catch { return Math.Round(StandardDeviation, 2); }
        }

        /*Метод для пошуку коефіцієнта варіації*/
        public double CalculateCoefficientVariation()
        {
            try
            {
                CoefficientVariation = 0;
                CoefficientVariation = StandardDeviation / AverageValue;
                return Math.Round(CoefficientVariation, 2);
            }
            catch { return Math.Round(CoefficientVariation, 2); }
        }

        /*Метод для пошуку нормованого моментного коефіцієнта асиметрії і його оцінки*/
        public double CalculateNormalCoefficientAsymmetry()
        {
            try
            {
                EstimationCoefficientAsymmetry = "";
                NormalCoefficientAsymmetry = 0;
                double Mom = 0;
                for (int i = 0; i < RowList.Count - 1; i++)
                {
                    Mom += Math.Abs(Math.Pow((Convert.ToDouble(RowList[i]) - AverageValue), 3));
                }
                NormalCoefficientAsymmetry = Mom / (Math.Pow(StandardDeviation, 3));
                if (NormalCoefficientAsymmetry == 0)
                {
                    EstimationCoefficientAsymmetry = "Ряд симетричний";
                }
                else if (NormalCoefficientAsymmetry > 0)
                {
                    EstimationCoefficientAsymmetry = "Правостороння скошенність ряду";
                }
                else { EstimationCoefficientAsymmetry = "Лівостороння скошенність ряду"; }
                return Math.Round(NormalCoefficientAsymmetry, 2);
            }
            catch { return Math.Round(NormalCoefficientAsymmetry, 2); }
        }

        /*Метод для пошуку ступеня суттєвості асиметрії*/
        public double CalculateDegreeAsymmetry()
        {
            MaterialityAsymmetry = "";
            double DegreeAsymmetry = 0;
            try
            {
                DegreeAsymmetry = Math.Sqrt((6 * ((RowList.Count - 1) - 1)) / 
                    ((RowList.Count - 1) + 1) * ((RowList.Count - 1) + 3));
                if (((Math.Abs(NormalCoefficientAsymmetry) / DegreeAsymmetry)) > 3)
                {
                    MaterialityAsymmetry = "Суттєва асиметрія";
                }
                else { MaterialityAsymmetry = "Несуттєва асиметрія"; }
                return Math.Round(DegreeAsymmetry, 2);
            }
            catch { return Math.Round(DegreeAsymmetry, 2); }
        }

        /*Метод для пошуку ексцесу*/
        public double CalculateExcess()
        {
            try
            {
                Excess = 0;
                double mom = 0;
                for (int i = 0; i < RowList.Count - 1; i++)
                {
                    mom += (Math.Pow((Convert.ToDouble(RowList[i]) - AverageValue), 4)) 
                        / (RowList.Count - 1);
                }
                Excess = mom / (Math.Pow(StandardDeviation, 4) - 3);
                return Math.Round(Excess, 2);
            }
            catch { return Math.Round(Excess, 2); }
        }

        /*Метод для пошуку помилки ексцесу*/
        public string CalculateExcessError()
        {
            ExcessErrorString = "";
            double ExcessError = 0;
            try
            {
                ExcessError = Math.Sqrt(((24 * RowList.Count) * (RowList.Count - 2) * (RowList.Count - 3)) 
                    / ((Math.Pow((RowList.Count - 1), 2)) * (RowList.Count + 3) * (RowList.Count + 5)));
                if ((Math.Abs(Excess) / ExcessError) > 3)
                {
                    ExcessErrorString = "Відхилення суттєво";
                }
                else { ExcessErrorString = "Відхилення несуттєво"; }
                return ExcessErrorString;
            }
            catch { return ExcessErrorString; }
        }
    }
}