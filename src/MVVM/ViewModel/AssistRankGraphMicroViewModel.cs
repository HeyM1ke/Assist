using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ValNet.Objects.Player;

namespace Assist.MVVM.ViewModel
{
    internal class AssistRankGraphMicroViewModel
    {
        public SeriesCollection SeriesCollection { get; set; }
        public CompetitiveUpdateObj CompetitiveUpdates { get; set; }
        public double[] rrDiff { get; set; }
        public async Task SetupGraph()
        {
            CompetitiveUpdates = await AssistApplication.AppInstance.CurrentUser.Player.GetCompetitiveUpdates();

             rrDiff = new double[] {0,0,0};

             if(CompetitiveUpdates is not null && CompetitiveUpdates.Matches is not null){
                for (int i = 0; i < CompetitiveUpdates.Matches.Count && i < 3; i++)
                {
                    if (CompetitiveUpdates.Matches[i] != null)
                        rrDiff[i] = CompetitiveUpdates.Matches[i].RankedRatingEarned;
                    else
                        rrDiff[i] = 0;
                }
             }

             rrDiff.Reverse();

            SeriesCollection = new SeriesCollection()
            {
                new LineSeries
                {
                    Title = "RR Gain/Loss",
                    Values = new ChartValues<double>(rrDiff),
                    Fill = new SolidColorBrush(Color.FromArgb(128, 255, 71, 87)),
                    PointForeground = Brushes.Red,
                    Stroke = Brushes.White
                }
            };


        }
    }
}
