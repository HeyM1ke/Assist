using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Services;
using Assist.ViewModels;
using Assist.Views.Store;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace Assist.Controls.Store.ViewModels
{
    internal class OfferContainerViewModel : ViewModelBase
    {
        private IEnumerable<OfferControl> _offerControls;

        public IEnumerable<OfferControl> OfferControls
        {
            get => _offerControls;
            set => this.RaiseAndSetIfChanged(ref _offerControls, value);
        }

        private string _bundleTimer = "Loading..";
        public string OfferTimer
        {
            get => _bundleTimer;
            set => this.RaiseAndSetIfChanged(ref _bundleTimer, value);
        }


        public async Task Setup()
        {

            if (AssistApplication.Current.CurrentUser == null)
            {
                OfferControls = new List<OfferControl>()
                {
                    new OfferControl()
                };

                return;
            }

            var store = AssistApplication.Current.CurrentUser.Store.PlayerStore;
            offerSec = store.SkinsPanelLayout.SingleItemOffersRemainingDurationInSeconds;
            await StartCountdown();
            int t = 0;

            OfferControls = store.SkinsPanelLayout.SingleItemOffers.Select(b => {
                OfferControl offerControl = new OfferControl(b);
                offerControl.Height = 150;

                offerControl.Margin = new Thickness(5, 0, 0, 0);

                if(t == 0)
                    offerControl.Margin = new Thickness(0, 0, 0, 0);

                t += 1;
                return offerControl;
            });

        }

        private DispatcherTimer bundleTimer;
        private int offerSec = 0;
        private async Task StartCountdown()
        {
            bundleTimer = new DispatcherTimer();
            bundleTimer.Tick += TimerTick;
            bundleTimer.Interval = TimeSpan.FromSeconds(1);
            bundleTimer.Start();
        }

        private void TimerTick(object? sender, EventArgs e)
        {

            if (offerSec == 0)
            {
                bundleTimer.Stop();
                MainViewNavigationController.Change(new StoreView());

            }
            else
            {
                offerSec -= 1;
            }
            var t = TimeSpan.FromSeconds(offerSec);
            OfferTimer = t.ToString(@"hh\:mm\:ss");

        }
    }
}
