using Assist.MVVM.ViewModel;

using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using ValNet.Objects.Store;

namespace Assist.Controls.Store.ViewModels
{
    internal class NightMarketItemViewModel : ViewModelBase
    {

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        private string _price;
        public string Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        private string _discount;
        public string Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }

        // todo: UpdateItem
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

        private int _fontSize = 12;
        public int FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        private async Task UpdateItem()
        {
            var skin = await AssistApplication.ApiService.GetWeaponSkinAsync(Offer.Offer.OfferID);

            Name = skin.DisplayName.ToUpper();
            DetermineFontSize();

            Discount = $"-{Offer.DiscountPercent}%";
            Price = $"{Offer.DiscountCosts.ValorantPointCost:n0}";
            Image = App.LoadImageUrl(skin.Levels[0].DisplayIcon);
        }

        private void DetermineFontSize()
        {
            var l = Name.Length;
            FontSize = 12;

            if (l >= 20)
                FontSize = 11;

            if (l >= 30)
                FontSize = 9;
        }

    }
}
