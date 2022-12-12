using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using Serilog;
using ValNet.Enums;

namespace Assist.Controls.Global.ViewModels
{
    internal class UserSelectionViewModel : ViewModelBase
    {
        public string ProfilePicture
        {
            get => $"https://content.assistapp.dev/playercards/{Profile.PlayerCardId}_DisplayIcon.png";
        }

        private bool popupOpen = false;
        public bool PopupOpen
        {
            get => popupOpen;
            set => this.RaiseAndSetIfChanged(ref popupOpen, value);
        }
        
        private List<UserControl> _profileControls = new List<UserControl>();
        public List<UserControl> ProfileControls
        {
            get => _profileControls;
            set => this.RaiseAndSetIfChanged(ref _profileControls, value);
        }

        public bool isExpired => _profile.isExpired;
        public string Username => _profile.RiotId;
#pragma warning disable CS8603 // Possible null reference return.
        public string Region => Enum.GetName(typeof(RiotRegion), _profile.Region);
#pragma warning restore CS8603 // Possible null reference return.

        private ProfileSettings _profile;

        public ProfileSettings Profile
        {
            get => _profile;
            set => this.RaiseAndSetIfChanged(ref _profile, value);
        }

        public async Task LoadProfiles()
        {
            Log.Information("Loading Profiles..");
            foreach (var profile in AssistSettings.Current.Profiles)
            {
                if(profile.ProfileUuid != AssistApplication.Current.CurrentProfile.ProfileUuid)
                    ProfileControls.Add(new UserSelectionBtn(profile)
                    {
                        Width = 240,
                        Height = 60
                    });
            }
            

            ProfileControls.Add(new UserAddBtn()
            {
                Width = 240,
                Height = 30,
                Margin = new Thickness(0,20,0,10)
            });
        }
    }
}
