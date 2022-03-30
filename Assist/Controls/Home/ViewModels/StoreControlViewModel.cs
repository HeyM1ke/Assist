using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Assist.MVVM.ViewModel;
using AssistWPFTest.MVVM.ViewModel;
using ValNet.Objects.Store;

namespace Assist.Controls.Home.ViewModels
{
    internal class StoreControlViewModel : ViewModelBase
    {
        private string _bundleName;

        public string BundleName
        {
            get => _bundleName;
            set => SetProperty(ref _bundleName, value);
        }

        private string _bundlePrice;

        public string BundlePrice
        {
            get => _bundlePrice;
            set => SetProperty(ref _bundlePrice, value);
        }

        private BitmapImage _bundleImage;

        public BitmapImage BundleImage
        {
            get => _bundleImage;
            set => SetProperty(ref _bundleImage, value);
        }

        public List<string> StoreItemOffers;

        private ValNet.Objects.Store.PlayerStore StoreResp;

        public async Task GetShop()
        {
            StoreResp = await AssistApplication.AppInstance.CurrentUser.Store.GetPlayerStore();
            StoreItemOffers = StoreResp.SkinsPanelLayout.SingleItemOffers;
        }
        public async Task SetupControl()
        {
            SetupBundle(StoreResp.FeaturedBundle.Bundle);
        }

        private async Task SetupBundle(Bundle bundle)
        {
            var temp = await AssistApplication.AppInstance.AssistApiController.GetBundleObj(bundle.DataAssetID);
            BundleImage = await App.LoadImageUrl(temp.bundleDisplayIcon,705 , 344);
            BundleName = temp.bundleDisplayName.ToUpper();
            BundlePrice = await GetBundlePrice(bundle);
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

        private async Task SetupItems()
        {

        }
    }
}
