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

namespace Assist.MVVM.View.Store
{
    /// <summary>
    /// Interaction logic for Store_NightMarket.xaml
    /// </summary>
    public partial class Store_NightMarket : Page
    {
        private long timeRemaining = AssistApplication.AppInstance.CurrentUser.Store.PlayerStore.BonusStore
            .NightMarketTimeRemainingInSeconds;
        public Store_NightMarket()
        {
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            AssistMainWindow.Current.ContentFrame.GoBack();
        }

        private async void NightMarket_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMarket();
        }

        private async Task LoadMarket()
        {
            // Store page has to have been requested to get to the night market.
            // Do not make a new call, just look at stored data.
            var storeData = AssistApplication.AppInstance.CurrentUser.Store.PlayerStore;

            foreach (var offer in storeData.BonusStore.NightMarketOffers)
            {
                OfferContainer.Children.Add(new NightMarketItemView(offer));
            }

            await EndTimer();
        }

        private Timer marketTimer;
        private async Task EndTimer()
        {
            marketTimer = new Timer();
            marketTimer.Tick += TimerTick;
            marketTimer.Interval = 1000;
            marketTimer.Start();
            
        }

        private void TimerTick(object? sender, EventArgs e)
        {

            if (timeRemaining == 0)
            {
                marketTimer.Stop();
                AssistMainWindow.Current.GoToStore();
            }
            else
            {
                timeRemaining--;
            }
            var t = TimeSpan.FromSeconds(timeRemaining);
            EndsTimer.Content = $"{Properties.Languages.Lang.NightMarket_EndsIn}: {t.ToString(@"dd\:hh\:mm\:ss")}";

        }

    }
}
