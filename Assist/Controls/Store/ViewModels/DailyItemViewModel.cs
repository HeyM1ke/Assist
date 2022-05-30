using Assist.MVVM.Model;
using Assist.MVVM.ViewModel;

using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace Assist.Controls.Store.ViewModels
{
    internal class DailyItemViewModel : ViewModelBase
    {

        private AssistSkin _itemObj = new();
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
            ItemObj = await AssistApplication.AppInstance.AssistApiController.GetSkinObj(skinId);
            SkinPrice = await AssistApplication.AppInstance.AssistApiController.GetSkinPricing(skinId);
            SkinImage = App.LoadImageUrl(ItemObj.Levels[0].DisplayIcon);
            SkinName = ItemObj.DisplayName;
        }

    }
}
