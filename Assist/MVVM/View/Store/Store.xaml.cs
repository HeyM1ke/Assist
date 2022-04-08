using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assist.Controls.Store;
using Assist.MVVM.ViewModel;
using ValNet.Objects.Store;

namespace Assist.MVVM.View.Store
{
    /// <summary>
    /// Interaction logic for Store.xaml
    /// </summary>
    public partial class Store : Page
    {
        public static Store CurrentStore;
        private PlayerStore _playerStore;
        public Store()
        {
            InitializeComponent();
        }

        private async void Store_Loaded(object sender, EventArgs e)
        {
            // Load Daily Items (Loaded by Storepage for now, need to make custom control for future performance)
            await GetUserStore();
            Load_Bundle();
            Load_DailyStore();
            await NightMarketCheck();

        }

        

        private async void Load_Bundle()
        {
            BundleView.Children.Add(new BundleView());
        }
        private async void Load_DailyStore()
        {
            for (int i = 0; i < _playerStore.SkinsPanelLayout.SingleItemOffers.Count; i++)
            {
                if (i == 0)
                {
                    var skinControl = new DailyItemView(_playerStore.SkinsPanelLayout.SingleItemOffers[i])
                    {
                        Margin = new Thickness(0, 0, 10, 0),
                        Width = 220
                    };

                    ItemContainer.Children.Add(skinControl);
                }
                else
                {
                    var skinOffer = new DailyItemView(_playerStore.SkinsPanelLayout.SingleItemOffers[i])
                    {
                        Margin = new Thickness(10, 0, 10, 0),
                        Width = 220
                    };

                    ItemContainer.Children.Add(skinOffer);
                }

                
            }

            StartCountdown();
        }
        private async Task NightMarketCheck()
        {
            if (await isNightMarketActive())
            {
                NightMarketBtn.Visibility = Visibility.Visible;
            }
            else
            {
                NightMarketBtn.Visibility = Visibility.Collapsed;
            }
        }

        private Timer dailyTimer;
        private async Task StartCountdown()
        {
            dailyTimer = new Timer();
            dailyTimer.Tick += TimerTick;
            dailyTimer.Interval = 1000;
            dailyTimer.Start();
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            if (_playerStore.SkinsPanelLayout.SingleItemOffersRemainingDurationInSeconds == 0)
            {
                dailyTimer.Stop();
                AssistMainWindow.Current.ContentFrame.Refresh();
            }
            else
            {
                _playerStore.SkinsPanelLayout.SingleItemOffersRemainingDurationInSeconds--;
            }
            var t = TimeSpan.FromSeconds(_playerStore.SkinsPanelLayout.SingleItemOffersRemainingDurationInSeconds);
            DailyTimeRemaining.Content = t.ToString(@"dd\:hh\:mm\:ss");

        }

        public async Task GetUserStore()
        {
            _playerStore = await AssistApplication.AppInstance.CurrentUser.Store.GetPlayerStore();
        }

        private async Task<bool> isNightMarketActive()
        {
            return AssistApplication.AppInstance.CurrentUser.Store.PlayerStore.BonusStore is null ? false : true;
        }

        private void NightMarketBtn_Click(object sender, RoutedEventArgs e)
        {
            AssistMainWindow.Current.ContentFrame.Navigate(new Uri("MVVM/View/Store/Store_NightMarket.xaml",
                UriKind.RelativeOrAbsolute));
        }
    }
}
