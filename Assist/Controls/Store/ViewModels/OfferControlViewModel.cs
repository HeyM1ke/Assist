using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Media;
using ReactiveUI;

namespace Assist.Controls.Store.ViewModels
{
    internal class OfferControlViewModel : ViewModelBase
    {

        private string _offerId = "97af88e4-4176-9fa3-4a26-57919443dab7";

        public string OfferId
        {
            get => _offerId;
            set => this.RaiseAndSetIfChanged(ref _offerId, value);
        }

        private string _skinName = "Glitchpop Odin";

        public string SkinName
        {
            get => _skinName;
            set => this.RaiseAndSetIfChanged(ref _skinName, value);
        }

        private string _skinImage = "https://cdn.assistapp.dev/Skin/Level/549b06bb-4704-25ce-19d5-c9b70b10de19_DisplayIcon.png";

        public string SkinImage
        {
            get => _skinImage;
            set => this.RaiseAndSetIfChanged(ref _skinImage, value);
        }


        private string _offerPrice = "2,175";

        public string OfferPrice
        {
            get => _offerPrice;
            set => this.RaiseAndSetIfChanged(ref _offerPrice, value);
        }

        private LinearGradientBrush _backgroundBrush = Application.Current.Resources["DeluxeGrad"] as LinearGradientBrush;

        public LinearGradientBrush BackgroundBrush
        {
            get => _backgroundBrush;
            set => this.RaiseAndSetIfChanged(ref _backgroundBrush, value);
        }


        public async Task Setup()
        {
            var sData = await AssistApplication.ApiService.GetWeaponSkinAsync(OfferId);

            

            SkinImage = sData?.Levels[0].DisplayIcon;
            SkinName = sData?.DisplayName;
            OfferPrice = await AssistApplication.ApiService.GetWeaponSkinPriceAsync(OfferId);

            
        }
    }
}
