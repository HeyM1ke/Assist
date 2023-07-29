using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Services;
using Assist.ViewModels;
using Assist.Views.Store;
using Avalonia.Threading;
using ReactiveUI;
using Serilog;
using ValNet.Objects.Store;

namespace Assist.Controls.Store.ViewModels
{
    internal class BundleItemViewModel : ViewModelBase
    {
        public Bundle Bundle = new Bundle()
        {
            
        };


        private string _bundleName = "Loading..";
        public string BundleName
        {
            get => _bundleName.ToUpper();
            set => this.RaiseAndSetIfChanged(ref _bundleName, value);
        }

        private string _bundleDescription = "Loading..";
        public string BundleDescription
        {
            get => _bundleDescription;
            set => this.RaiseAndSetIfChanged(ref _bundleDescription, value);
        }

        private string _bundlePrice = "99,999";
        public string BundlePrice
        {
            get => _bundlePrice;
            set => this.RaiseAndSetIfChanged(ref _bundlePrice, value);
        }

        private string _bundleTimer = "Loading..";
        public string BundleTimer
        {
            get => _bundleTimer;
            set => this.RaiseAndSetIfChanged(ref _bundleTimer, value);
        }

        private string _bundleImage = "";
        public string BundleImage
        {
            get => _bundleImage;
            set => this.RaiseAndSetIfChanged(ref _bundleImage, value);
        }



        public async Task Setup()
        {
            if(Bundle.DataAssetID == null)
                return;

            var bData = await AssistApplication.ApiService.GetBundleAsync(Bundle.DataAssetID);

            

            BundleImage = bData.DisplayIcon;
            BundleName = bData.Name;
            BundleDescription = bData.ExtraDescription;

            // Assemble Price
            var price = Bundle.Items.Sum(x => x.DiscountedPrice);
            BundlePrice = $"{price:n0}";
            bunSec = Bundle.DurationRemainingInSeconds;
            // Start Timer
            await StartCountdown();
        }


        private async Task SetupTimer()
        {

        }

        private DispatcherTimer bundleTimer;
        private int bunSec = 0;
        private async Task StartCountdown ()
        {
            bundleTimer = new DispatcherTimer();
            bundleTimer.Tick += TimerTick;
            bundleTimer.Interval = TimeSpan.FromSeconds(1);
            bundleTimer.Start();
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            if (Bundle.DurationRemainingInSeconds == 0)
            {
                bundleTimer.Stop();
                MainViewNavigationController.Change(new StoreViewV2());
                
            }
            else
            {
                bunSec -= 1;
            }
            var t = TimeSpan.FromSeconds(bunSec);
            BundleTimer = t.ToString(@"dd\:hh\:mm\:ss");

        }
    }
}
