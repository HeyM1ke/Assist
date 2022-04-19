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
    internal class StorePopupViewModel : ViewModelBase
    {
        private AssistSkin _skin;
        public AssistSkin Skin
        {
            get => _skin;
            set => SetProperty(ref _skin, value);
        }

        private Uri _currentVideo;

        public Uri CurrentVideo
        {
            get => _currentVideo;
            set => SetProperty(ref _currentVideo, value);
        }

        private string _currentSkinName = "Arcade Phantom"; //sadge

        public string CurrentSkinName
        {
            get => _currentSkinName;
            set => SetProperty(ref _currentSkinName, value);
        }


        private BitmapImage _skinPreviewImage = App.LoadImageUrl("https://cdn.rumblemike.com/Skin/97af88e4-4176-9fa3-4a26-57919443dab7_DisplayIcon.png").Result;

        public BitmapImage SkinPreviewImage
        {
            get => _skinPreviewImage;
            set => SetProperty(ref _skinPreviewImage, value);
        }

        public void ChangeLevelVideo(int skinIndex)
        {
            if(Skin.Levels[skinIndex].StreamedVideoUrl is null) return;
            CurrentSkinName = Skin.Levels[skinIndex].DisplayName;
            SkinPreviewImage = App.LoadImageUrl(Skin.Levels[skinIndex].DisplayName).Result;
            CurrentVideo = new Uri(Skin.Levels[skinIndex].StreamedVideoUrl);
        }

        public void ChangeChromaVideo(int skinIndex)
        {
            if (Skin.Chromas[skinIndex].StreamedVideoUrl is null) return;

            CurrentSkinName = Skin.Chromas[skinIndex].DisplayName;
            SkinPreviewImage = App.LoadImageUrl(Skin.Chromas[skinIndex].DisplayIcon).Result;
            CurrentVideo = new Uri(Skin.Chromas[skinIndex].StreamedVideoUrl);
        }


    }
}
