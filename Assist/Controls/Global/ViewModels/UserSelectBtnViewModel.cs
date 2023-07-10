using System;
using Assist.Settings;
using Assist.ViewModels;
using ReactiveUI;
using ValNet.Enums;

namespace Assist.Controls.Global.ViewModels
{
    public class UserSelectBtnViewModel : ViewModelBase
    {
        public string ProfilePicture
        {
            get => $"https://content.assistapp.dev/playercards/{Profile.PlayerCardId}_DisplayIcon.png";
        }
        public bool isExpired => _profile.isExpired;
        public bool IsDefault => false;
        public string LastUsed => $"Last Used: {_profile.LastUsed.ToShortDateString()}";
        public string Username => _profile.RiotId;
#pragma warning disable CS8603 // Possible null reference return.
        public string Region => Enum.GetName(typeof(RiotRegion), _profile.Region);
#pragma warning restore CS8603 // Possible null reference return.

        private ProfileSettings _profile = new ProfileSettings()
        {
            Gamename = "0000000000000000",
            Tagline = "00000",
            Region = RiotRegion.UNKNOWN
        };

        public ProfileSettings Profile
        {
            get => _profile;
            set => this.RaiseAndSetIfChanged(ref _profile, value);
        }
    }
}