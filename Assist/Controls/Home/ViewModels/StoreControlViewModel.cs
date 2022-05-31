using Assist.MVVM.ViewModel;
using Assist.Utils;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using ValNet.Objects.Store;

using ValNetBundle = ValNet.Objects.Store.Bundle;

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

        public List<string> StoreItemOffers { get; set; }

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

        private async Task SetupBundle(ValNetBundle valNetBundle)
        {
            var bundle = await AssistApplication.ApiService.GetBundleAsync(valNetBundle.DataAssetID);

            BundleImage = App.LoadImageUrl(bundle.DisplayIcon,705 , 344);
            BundleName = bundle.Name.ToUpper();
            BundlePrice = valNetBundle.GetFormattedPrice();
        }

    }
}
