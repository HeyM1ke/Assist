using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Controls.Extra;
using Assist.Modules.Popup;
using Assist.MVVM.ViewModel;
using Assist.Settings;

namespace Assist.MVVM.View.Selector.ViewModels
{
    internal class StartupViewModel : ViewModelBase
    {
        private string _timerText;

        public string TimerText
        {
            get => _timerText;
            set => SetProperty(ref _timerText, value);
        }

        public async Task DefaultAccountLogin()
        {
            if (string.IsNullOrEmpty(AssistSettings.Current.DefaultAccount))
                return;

            var profile = AssistSettings.Current.FindProfileById(AssistSettings.Current.DefaultAccount);

            if (profile == null)
                return;

            PopupSystem.SpawnPopup(new PopupSettings()
            {
                PopupTitle = "Logging in...",
                PopupDescription = $"Logging into {profile.Gamename}",
                PopupType = PopupType.LOADING
            });

            try
            {
                await AssistApplication.AppInstance.AuthenticateWithProfileSetting(profile);
            }
            catch (Exception exception)
            {
                PopupSystem.KillPopups();
                AssistApplication.AppInstance.OpenStartupWindow();

            }
        }
    }
}
