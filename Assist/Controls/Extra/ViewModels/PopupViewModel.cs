using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Modules.Popup;
using Assist.MVVM.ViewModel;

namespace Assist.Controls.Extra.ViewModels
{
    public class PopupViewModel : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get => _popupSettings.PopupTitle;
            set => SetProperty(ref _title,value);
        }

        private string _description;
        public string Description
        {
            get => _popupSettings.PopupDescription;
            set => SetProperty(ref _description, value);
        }

        private PopupSettings _popupSettings = new()
        {
            PopupTitle = "POPUP",
            PopupDescription = "Default Description",
            PopupType = PopupType.LOADING
        };

        public PopupSettings PopupSettings
        {
            get => _popupSettings;
            set
            {
                SetProperty(ref _popupSettings, value);
                Description = _popupSettings.PopupDescription;
                Title = _popupSettings.PopupTitle;
            }
        }

        public async void UpdateSettings(PopupSettings s)
        {
            Title = s.PopupTitle;
            Description = s.PopupDescription;
        }
    }
}
