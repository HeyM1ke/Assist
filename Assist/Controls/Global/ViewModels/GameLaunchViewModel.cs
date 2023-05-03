using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Services.Riot;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Platform;
using ReactiveUI;
using ValNet.Objects.Inventory;

namespace Assist.Controls.Global.ViewModels
{
    internal class GameLaunchViewModel : ViewModelBase
    {
        static Dictionary<string, PlayerInventory> _inventory = new Dictionary<string, PlayerInventory>();
        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

        
        public ProfileSettings Profile
        {
            get => AssistApplication.Current.CurrentProfile;
        }

        private string _profilePlayercard;
        public string ProfilePlayercard
        {
            get => _profilePlayercard;
            set => this.RaiseAndSetIfChanged(ref _profilePlayercard, value);
        }
        
        public string SelectedPatchLine
        {
            get => AssistApplication.Current.ClientLaunchSettings.Patchline.ToUpper();
        }

        public string ProfileUsername
        {
            get => Profile.Gamename;
        }
        
        public string ProfileTagline
        {
            get => Profile.Tagline;
        }

        public string ProfileGamename
        {
            get => Profile.RiotId;
        }
        public void CheckEnable()
        {
            if (Design.IsDesignMode)
            {
                IsEnabled = true;
                return;
            }
                
            var p = AssistApplication.Current.Platform;
            
            IsEnabled = p.OperatingSystem == OperatingSystemType.WinNT && !RiotClientService.ClientOpened;
        }

        public async Task<PlayerInventory> SetPlayercard()
        {
            if (Design.IsDesignMode) return null;

            if (_inventory.ContainsKey(AssistApplication.Current.CurrentUser.UserData.sub))
            {
                return _inventory[AssistApplication.Current.CurrentUser.UserData.sub];
            }


            try
            {
                var inv = await AssistApplication.Current.CurrentUser.Inventory.GetPlayerInventory();
                _inventory.Add(AssistApplication.Current.CurrentUser.UserData.sub, inv);
                return inv;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
