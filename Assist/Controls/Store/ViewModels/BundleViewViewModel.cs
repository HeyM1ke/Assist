using Assist.MVVM.ViewModel;

using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using ValNet.Objects.Store;

namespace Assist.Controls.Store.ViewModels
{
    internal class BundleViewViewModel : ViewModelBase
    {

        private BitmapImage _bundleImage = App.LoadImageUrl("https://cdn.rumblemike.com/AssistHome.png", 673, 328);
        public BitmapImage BundleImage
        {
            get => _bundleImage;
            set => SetProperty(ref _bundleImage, value);
        }

        private string _bundleName = "Loading....";

        public string BundleName
        {
            get => _bundleName;
            set => SetProperty(ref _bundleName, value);
        }

        private string _bundlePrice = "0 VP";

        public string BundlePrice
        {
            get => _bundlePrice;
            set => SetProperty(ref _bundlePrice, value);
        }

        private Bundle _bundle;

        public Bundle Bundle
        {
            get => _bundle;
            set => SetProperty(ref _bundle, value);
        }

        private string _timeRemaining = "Loading..";

        public string TimeRemaining
        {
            get => _timeRemaining;
            set => SetProperty(ref _timeRemaining, value);
        }

        public async Task SetupBundle()
        {
            Bundle.DurationRemainingInSeconds += 10;
            var temp = await AssistApplication.AppInstance.AssistApiController.GetBundleObj(Bundle.DataAssetID);
            await StartCountdown();

            BundleImage = App.LoadImageUrl(temp.DisplayIcon, 673, 328);
            BundleName = temp.BundleName.ToUpper();
            BundlePrice = await GetBundlePrice(Bundle);
        }

        public async Task<string> GetBundlePrice(Bundle bundle)
        {
            int price = 0;

            foreach (var item in bundle.Items)
            {
                price += item.DiscountedPrice;
            }

            return string.Format("{0:n0}", price);
        }

        private Timer bundleTimer;
        private async Task StartCountdown()
        {
            bundleTimer = new Timer();
            bundleTimer.Tick += TimerTick;
            bundleTimer.Interval = 1000;
            bundleTimer.Start();
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            if (Bundle.DurationRemainingInSeconds == 0)
            {
                bundleTimer.Stop();
                AssistMainWindow.Current.ContentFrame.Refresh();
            }
            else
            {
                Bundle.DurationRemainingInSeconds--;
            }
            var t = TimeSpan.FromSeconds(Bundle.DurationRemainingInSeconds);
            TimeRemaining = t.ToString(@"dd\:hh\:mm\:ss");

        }


    }
}
