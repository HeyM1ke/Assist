using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using ValNet.Objects;

namespace Assist.Controls.Selector.ViewModel
{
    class ProfileCardViewModel : ViewModelBase
    {
        private ProfileSetting _profile = new ProfileSetting()
        {
            Gamename = "Username",
            Tagline = "00000",
            PCID = "612cd02d-4294-ee2a-644c-a3ba3ddf8805",
            Region = 0
        };

        public ProfileSetting Profile
        {
            get => _profile;
            set => SetProperty(ref _profile, value);
        }

        private string _profileName = "Username";
        public string ProfileName
        {
            get => _profile.RiotId;
            set => SetProperty(ref _profileName, value);
        }

        private string _profileRegion = "LATAM";
        public string ProfileRegion
        {
            get => Enum.GetName(typeof(RiotRegion), _profile.Region);
            set => SetProperty(ref _profileRegion, value);
        }

        private BitmapImage _profileImage = App.LoadImageUrl("https://cdn.rumblemike.com/PlayerCards/9ee85a55-4b94-e382-b8a8-f985a33c1cc2_DisplayIcon.png", 64, 64);
        public BitmapImage ProfileImage
        {
            get => _profileImage;
            set => SetProperty(ref _profileImage, value);
        }

        private BitmapImage _playerRankIcon;

        public BitmapImage PlayerRankIcon
        {
            get => _playerRankIcon;
            set => SetProperty(ref _playerRankIcon, value);
        }

        

    }
}
