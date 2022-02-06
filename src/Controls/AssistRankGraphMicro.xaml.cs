using Assist.MVVM.ViewModel;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistRankGraphMicro.xaml
    /// </summary>
    public partial class AssistRankGraphMicro : UserControl
    {

        AssistApplication _viewModel => AssistApplication.AppInstance;
        public SeriesCollection SeriesCollection { get; set; }

        public AssistRankGraphMicro()
        {
            InitializeComponent();
            DataContext = _viewModel.RankMicroGraphViewModel;
            
        }

        private async void RankG_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.RankMicroGraphViewModel.SetupGraph();

            PointChart.Series = _viewModel.RankMicroGraphViewModel.SeriesCollection;
            PointChart_XAxis.Labels = new string[] { "Game 1", "Game 2", "Game 3" };

            if (_viewModel.RankMicroGraphViewModel.CompetitiveUpdates.Matches.Count > 0)
                LoadRankData();
        }

        private void LoadRankData()
        {
            

            if (_viewModel.RankMicroGraphViewModel.CompetitiveUpdates.Matches[0].TierAfterUpdate != 0)
            {
                int rank = _viewModel.RankMicroGraphViewModel.CompetitiveUpdates.Matches[0].TierAfterUpdate;
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri($"pack://application:,,,/Resources/RankLogos/TX_CompetitiveTier_Large_{rank}.png");
                image.EndInit();

                RankLogo.Source = image;
                CurrentRRLabel.Content = _viewModel.RankMicroGraphViewModel.CompetitiveUpdates.Matches[0].RankedRatingAfterUpdate + " RR";
            }
            else
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri($"pack://application:,,,/Resources/RankLogos/TX_CompetitiveTier_Large_0.png", UriKind.RelativeOrAbsolute);
                image.EndInit();

                RankLogo.Source = image;
            }

            RankChangeImage1.Source = RRChangeImage(0);
            RankChangeImage2.Source = RRChangeImage(1);
            RankChangeImage3.Source = RRChangeImage(2);
        }

        private BitmapImage RRChangeImage(int index)
        {
            if (index < _viewModel.RankMicroGraphViewModel.rrDiff.Length)
            {
                Trace.WriteLine(_viewModel.RankMicroGraphViewModel.rrDiff[index]);
                if (_viewModel.RankMicroGraphViewModel.rrDiff[index] > 0)
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri($"pack://application:,,,/Resources/RankLogos/TX_CompetitiveTierMovement_Increase.png");
                    image.EndInit();

                    return image;
                }
                else if (_viewModel.RankMicroGraphViewModel.rrDiff[index] < 0)
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri($"pack://application:,,,/Resources/RankLogos/TX_CompetitiveTierMovement_Decrease.png");
                    image.EndInit();

                    return image;
                }
                else
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri($"pack://application:,,,/Resources/RankLogos/TX_CompetitiveTierMovement_Stable.png");
                    image.EndInit();

                    return image;

                }
            }
            else
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri($"pack://application:,,,/Resources/RankLogos/TX_CompetitiveTierMovement_Stable.png");
                image.EndInit();

                return image;
            }
            

        }
    }
}

