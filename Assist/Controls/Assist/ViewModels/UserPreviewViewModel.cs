using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.ViewModels;
using AssistUser.Lib.Profiles.Models;
using ReactiveUI;
using Serilog;

namespace Assist.Controls.Assist.ViewModels
{
    internal class UserPreviewViewModel : ViewModelBase
    {
        private string _assistDisplayImage;

        public string AssistDisplayImage
        {
            get => _assistDisplayImage;
            set => this.RaiseAndSetIfChanged(ref _assistDisplayImage, value);
        }

        public async Task SetupProfile()
        {
            // Set Profile Picture, and Online Status
            
            // Get Profile
            try
            {
                var p = await AssistApplication.Current.AssistUser.Profile.GetProfile();

                if (p.Code != 200)
                {
                    return;
                }

                var data = JsonSerializer.Deserialize<AssistProfile>(p.Data.ToString());
                // Set Display Image
                AssistDisplayImage = data.ProfileImage;
            }
            catch (Exception e)
            {
                AssistDisplayImage =
                    "https://static.wikia.nocookie.net/valorant/images/0/06/POLYfrog_Card_Large.png/revision/latest?cb=20210520175355";
            }
        }
    }
}
