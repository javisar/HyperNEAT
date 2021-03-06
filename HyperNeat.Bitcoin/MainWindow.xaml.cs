﻿using System;
using System.Threading;
using System.Windows;

namespace HyperNeat.Bitcoin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Calc calc;

        private Thread calcthread;

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            calc = new Calc();
            calc.ProgressUpdate += (s, ee) => {
                Dispatcher.Invoke(
                    delegate()
                        {
                            var progress = (ProgressEventArgs)ee;
                            CurrentGeneration.Content = progress.CurrentGeneration;
                            MaxFitness.Content = progress.BestGenomeFitness;
                            AvgFitness.Content = progress.AverageFitness;
                            Money.Content = progress.BestGenomeMoney;
                            Start.Content = progress.StartPrice;
                            End.Content = progress.EndPrice;
                            WeeklyProfit.Content = progress.WeeklyProfit;
                            Buys.Content = progress.BestGenomeBuys;
                            Sells.Content = progress.BestGenomeSells;
                            Species.Content = progress.SpecieCount;
                            TradeHistory.Content = progress.TradeHistory;
                            Fingerprint.Content = progress.NetworkFingerprint;
                            SpeciesOverview.Content = progress.SpeciesOverview;

                            History.AppendText(
                                string.Format(
                                    "{15:HH:mm:ss} Gen: {4}, Max Fit: {0:0.00000}, AVG Fit: {9:0.00000}, Money: {5:0.00}, Best Money: {12:0.00}, Buy: {1}, Sells: {2}, Counts: {11} YP: {3:0.00000}, Start: {6}, End: {7}, ST: {13:yyyy-MM-dd}, ET: {14:yyyy-MM-dd}, FP: {10}, Spec: {8} \n",
                                    progress.BestGenomeFitness,
                                    progress.BestGenomeBuys,
                                    progress.BestGenomeSells,
                                    progress.WeeklyProfit,
                                    progress.CurrentGeneration,
                                    progress.BestGenomeMoney,
                                    progress.StartPrice,
                                    progress.EndPrice,
                                    progress.SpecieCount,
                                    progress.AverageFitness,
                                    progress.NetworkFingerprint,
                                    progress.TradeCounts,
                                    progress.BestGenomeTotalMoney,
                                    progress.StartTime,
                                    progress.EndTime,
                                    DateTime.Now));
                            
                            History.ScrollToEnd();
                        });
            };

            calcthread = new Thread(new ParameterizedThreadStart(calc.StartTrading));
            calcthread.IsBackground = true;
            calcthread.Priority = ThreadPriority.BelowNormal;
            calcthread.Start(null);
        }

        private void SerializeButton_OnClick(object sender, RoutedEventArgs e)
        {
            calc.DoSerialize = true;
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            if (calcthread != null && (calcthread.ThreadState == ThreadState.Running || calcthread.ThreadState == ThreadState.Background))
            {
                calc.Stop = true;

                calcthread.Join();

                calcthread = null;
                calc = null;

                History.Clear();
            }
        }
    }
}
