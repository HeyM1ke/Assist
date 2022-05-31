using Assist.MVVM.ViewModel;

using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Assist.Controls.Home.ViewModels
{
    internal class ItemControlViewModel : ViewModelBase
    {

        private BitmapImage _skinImage;
        public BitmapImage SkinImage
        {
            get => _skinImage;
            set => SetProperty(ref _skinImage, value);
        }

        public async Task SetupSkinAsync(string id)
        {
            var data = await AssistApplication.ApiService.GetWeaponSkinAsync(id);
            SkinImage = App.LoadImageUrl(data.Levels[0].DisplayIcon, BitmapCacheOption.None);
        }

    }
}
