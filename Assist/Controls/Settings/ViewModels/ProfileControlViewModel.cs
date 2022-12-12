using Assist.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.ViewModels;
using ReactiveUI;
using Avalonia.Controls;
using ValNet.Enums;
using Assist.Services.Popup;
using Assist.Services;
using Assist.Views.Authentication;

namespace Assist.Controls.Settings.ViewModels
{
    internal class ProfileControlViewModel : ViewModelBase
    {
        private ProfileSettings? _profileLinked;
        public ProfileSettings? ProfileLinked
        {
            get { return _profileLinked; }
            set { this.RaiseAndSetIfChanged(ref _profileLinked, value); }
        }

        private string _accountName;
        public string? AccountName
        {
            get { return _accountName; }
            set { this.RaiseAndSetIfChanged(ref _accountName, value); }
        }

        public string? _accountRegion;
        public string? AccountRegion
        {
            get { return _accountRegion; }
            set { this.RaiseAndSetIfChanged(ref _accountRegion, value); }
        }

        private string? _accountPlayercard;
        public string? AccountPlayercard
        {
            get { return _accountPlayercard; }
            set { this.RaiseAndSetIfChanged(ref _accountPlayercard, value); }
        }

        private bool? _isSwitchEnabled;
        public bool? isSwitchEnabled
        {
            get { return _isSwitchEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isSwitchEnabled, value); }
        }

        public async void SwitchProfile()
        {
            AssistApplication.Current.SwapCurrentProfile(ProfileLinked);
        }

        public async void SetupProfile()
        {
            AccountName = ProfileLinked.RiotId;
            AccountRegion = Enum.GetName(typeof(RiotRegion), ProfileLinked.Region);
            AccountPlayercard = $"https://content.assistapp.dev/playercards/{ProfileLinked.PlayerCardId}_DisplayIcon.png";
            isSwitchEnabled = ProfileLinked.ProfileUuid == AssistApplication.Current.CurrentProfile.ProfileUuid
                ? false
                : true;
        }

        public void RemoveProfile()
        {
            var r = AssistSettings.Current.Profiles.Remove(ProfileLinked);

            if (AssistSettings.Current.Profiles.Count == 0)
            {
                MainWindowContentController.Change(new AuthenticationView());
                PopupSystem.KillPopups();
                return;
            }

            if (AssistApplication.Current.CurrentProfile.ProfileUuid != ProfileLinked.ProfileUuid)
                return;

            AssistApplication.Current.SwapCurrentProfile(AssistSettings.Current.Profiles[0], true);
        }
    }
}
