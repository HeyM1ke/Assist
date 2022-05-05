using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Assist.MVVM.Model;
using Assist.MVVM.ViewModel;


namespace Assist.Controls.Store.ViewModels
{
    internal class DailyItemViewModel : ViewModelBase
    {
        private AssistSkin _itemObj = new AssistSkin()
        {

        };
        public AssistSkin ItemObj
        {
            get => _itemObj;
            set => SetProperty(ref _itemObj, value);
        }

        private string _skinName = "Loading..";
        public string SkinName
        {
            get => _skinName.ToUpper();
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

        private BitmapImage _tierIcon;
        public BitmapImage TierIcon
        {
            get => _tierIcon;
            set => SetProperty(ref _tierIcon, value);
        }


        public async Task SetupSkin(string skinId)
        {
            this.ItemObj = await AssistApplication.AppInstance.AssistApiController.GetSkinObj(skinId);
            this.SkinPrice = await AssistApplication.AppInstance.AssistApiController.GetSkinPricing(skinId);
            this.SkinImage = await App.LoadImageUrl(ItemObj.Levels[0].DisplayIcon);
            this.SkinName = ItemObj.DisplayName;
        }
    }
}
