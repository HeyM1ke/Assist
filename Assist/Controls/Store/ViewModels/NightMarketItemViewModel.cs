using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Assist.MVVM.ViewModel;
using ValNet.Objects.Store;

namespace Assist.Controls.Store.ViewModels
{
    internal class NightMarketItemViewModel : ViewModelBase
    {
        private string _skinName;

        public string SkinName
        {
            get => _skinName;
            set => SetProperty(ref _skinName, value);
        }

        private BitmapImage _skinImage;

        public BitmapImage SkinImage
        {
            get => _skinImage;
            set => SetProperty(ref _skinImage, value);

            
        }

        private string _skinPrice;

        public string SkinPrice
        {
            get => _skinPrice;
            set => SetProperty(ref _skinPrice, value);
        }

        private string _skinDiscount;

        public string SkinDiscount
        {
            get => _skinDiscount;
            set => SetProperty(ref _skinDiscount, value);
        }

        private NightMarket.NightMarketOffer _offer;

        public NightMarket.NightMarketOffer Offer
        {
            get => _offer;
            set
            {
                SetProperty(ref _offer, value);
                UpdateItem();
            }
        }

        private int _fontSize;

        public int FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        private async Task UpdateItem()
        {

            var skinData = await AssistApplication.AppInstance.AssistApiController.GetSkinObj(Offer.Offer.OfferID);

            SkinName = skinData.DisplayName.ToUpper();
            await DetermineFontSize();
            SkinDiscount = $"-{Offer.DiscountPercent}%";
            SkinPrice = $"{string.Format("{0:n0}", Offer.DiscountCosts.ValorantPointCost)}";
            SkinImage = await App.LoadImageUrl(skinData.Levels[0].DisplayIcon);
        }

        private async Task DetermineFontSize()
        {
            var l = SkinName.Length;
            FontSize = 12;

            if (l >= 20)
                FontSize = 11;

            if (l >= 30)
                FontSize = 9;
        }

    }
}
