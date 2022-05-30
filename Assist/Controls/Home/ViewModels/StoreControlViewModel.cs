using Assist.MVVM.ViewModel;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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

        private PlayerStore _store;

        public async Task GetShop()
        {
            _store = await AssistApplication.AppInstance.CurrentUser.Store.GetPlayerStore();
            StoreItemOffers = _store.SkinsPanelLayout.SingleItemOffers;
        }
        public async Task SetupControl()
        {
            await SetupBundle(_store.FeaturedBundle.Bundle);
        }

        private async Task SetupBundle(Bundle bundle)
        {
            var temp = await AssistApplication.AppInstance.AssistApiController.GetBundleObj(bundle.DataAssetID);
            BundleImage = App.LoadImageUrl(temp.DisplayIcon,705 , 344);
            BundleName = temp.BundleName.ToUpper();
            BundlePrice = GetBundlePrice(bundle);
        }

        public string GetBundlePrice(Bundle bundle)
        {
            var price = bundle.Items.Sum(x => x.DiscountedPrice);
            return $"{price:n0}";
        }

    }
}
