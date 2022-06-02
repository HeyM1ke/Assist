using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.MVVM.ViewModel;
using Assist.Settings;

namespace Assist.MVVM.View.Startup.ViewModels
{
    internal class StartupViewModel : ViewModelBase
    {
        private string _timerText = "TimerText";

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

            await AssistApplication.AppInstance.AuthenticateWithProfileSetting(profile);
        }
    }
}
