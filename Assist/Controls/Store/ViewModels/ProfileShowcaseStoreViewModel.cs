using Assist.MVVM.ViewModel;
using Assist.Settings;

using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using ValNet.Objects;

namespace Assist.Controls.Store.ViewModels
{
    internal class ProfileShowcaseStoreViewModel : ViewModelBase
    {

        private ProfileSetting _profile = new()
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

        private BitmapImage _profileImage = App.LoadImageUrl("https://cdn.rumblemike.com/PlayerCards/9ee85a55-4b94-e382-b8a8-f985a33c1cc2_DisplayIcon.png", 79, 79);
        public BitmapImage ProfileImage
        {
            get => _profileImage;
            set => SetProperty(ref _profileImage, value);
        }

        private BitmapImage _backingImage = App.LoadImageUrl("https://cdn.rumblemike.com/Maps/2FB9A4FD-47B8-4E7D-A969-74B4046EBD53_splash.png", 217, 89);
        public BitmapImage BackingImage
        {
            get => _backingImage;
            set => SetProperty(ref _backingImage, value);
        }

        private string _accountVpBalance = "0";
        public string AccountVpBalance
        {
            get => _accountVpBalance;
            set => SetProperty(ref _accountVpBalance, value);
        }

        private string _accountRpBalance = "0";
        public string AccountRpBalance
        {
            get => _accountRpBalance;
            set => SetProperty(ref _accountRpBalance, value);
        }

        public async Task GetPlayerBalance()
        {
            var wallet = await AssistApplication.AppInstance.CurrentUser.Store.GetPlayerBalance();
            if (wallet == null)
                return;

            AccountVpBalance = $"{wallet.Balances.VP:n0}";
            AccountRpBalance = $"{wallet.Balances.RP:n0}";
        }

    }
}
