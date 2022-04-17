using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ValNet.Objects.Store;
using Assist.MVVM.ViewModel;
using System.Windows.Media.Imaging;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistStoreNightMarketItem.xaml
    /// </summary>
    public partial class AssistStoreNightMarketItem : UserControl
    {
        AssistNightMarketViewModel _viewModel { get; set; }
        NightMarket.NightMarketOffer Offer { get; set; }
        public AssistStoreNightMarketItem()
        {
            InitializeComponent();
        }

        public AssistStoreNightMarketItem(NightMarket.NightMarketOffer pOffer)
        {
            Offer = pOffer;
            DataContext = _viewModel = new AssistNightMarketViewModel();
            InitializeComponent();
        }

        private async void NightMarketItem_Initialized(object sender, EventArgs e)
        {
            await _viewModel.LoadSkin(Offer);
            skinName.Text = _viewModel.SkinName;
            discountPercentage.Content = _viewModel.DiscountPercentage;
            priceLabel.Content = _viewModel.ValorantPointCost;
            loadImage(_viewModel.skinData.Levels[0].DisplayIcon);
            
        }

        private void loadImage(string url)
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(url, UriKind.Absolute);
            image.EndInit();
            skinImage.Source = image;



        }
    }
}
