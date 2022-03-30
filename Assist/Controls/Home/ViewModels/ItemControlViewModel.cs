using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Assist.MVVM.ViewModel;
using AssistWPFTest.MVVM.ViewModel;

namespace Assist.Controls.Home.ViewModels
{
    internal class ItemControlViewModel : ViewModelBase
    {
        private string _skinName = "Loading..";
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


        public async Task SetupSkin(string skinId)
        {
            var data = await AssistApplication.AppInstance.AssistApiController.GetSkinObj(skinId);
            this.SkinImage = await App.LoadImageUrl(data.levels[0].displayIcon);
            
        }
    }
}
