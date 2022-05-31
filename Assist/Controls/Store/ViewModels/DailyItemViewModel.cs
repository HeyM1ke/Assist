using Assist.MVVM.ViewModel;
using Assist.Objects.Valorant.Skin;

using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace Assist.Controls.Store.ViewModels
{
    internal class DailyItemViewModel : ViewModelBase
    {

        private WeaponSkin _skin = new();
        public WeaponSkin Skin
        {
            get => _skin;
            set => SetProperty(ref _skin, value);
        }

        private string _skinName = "Loading..";
        public string Name
        {
            get => _skinName.ToUpper();
            set => SetProperty(ref _skinName, value);
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

        private BitmapImage _tierIcon;
        public BitmapImage TierIcon
        {
            get => _tierIcon;
            set => SetProperty(ref _tierIcon, value);
        }

        public async Task SetupSkinAsync(string id)
        {
            Skin = await AssistApplication.ApiService.GetWeaponSkinAsync(id);
            Price = await AssistApplication.ApiService.GetWeaponSkinPriceAsync(id);

            Image = App.LoadImageUrl(Skin.Levels[0].DisplayIcon);
            Name = Skin.DisplayName;
        }

    }
}
