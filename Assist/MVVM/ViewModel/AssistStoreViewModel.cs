using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValNet.Objects.Store;
using System.Text.Json;
using System.Windows.Media.Imaging;
using Assist.MVVM.Model;
using RestSharp;

namespace Assist.MVVM.ViewModel
{
    internal class AssistStoreViewModel
    {
        public PlayerStore playerStore;

        private const string bundleUrl = "https://assist.rumblemike.com/Bundle/";
        private AssistBundleObj currentBundle;
        public AssistBundleObj CurrentBundle => currentBundle;

        public async Task GetUserStore()
        {
            playerStore = await AssistApplication.AppInstance.CurrentUser.Store.GetPlayerStore();
        }

        public async Task<BitmapImage> SetupImage()
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.DecodePixelWidth = 974;
            image.DecodePixelHeight = 474;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(currentBundle.DisplayIcon, UriKind.Absolute);
            image.EndInit();

            return image;
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
    }
}
