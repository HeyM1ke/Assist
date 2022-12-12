using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assist.Controls.Settings;
using Assist.Services;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ValNet.Enums;

namespace Assist.Views.Settings.Pages
{
    public partial class AccountsSettingsPage : UserControl
    {
        public AccountsSettingsPage()
        {
            SettingsViewNavigationController.CurrentPage = SettingsPages.ACCOUNTS;
            InitializeComponent();
        }

        private async void UniformGrid_Init(object? sender, EventArgs e)
        {
            var l = await CreateControls();

            var obj = sender as UniformGrid;

            if (obj != null)
            {
                obj.Children.AddRange(l);
            }
        }


        //TEMP
        private async Task<List<ProfileControl>> CreateControls()
        {
            var controls = new List<ProfileControl>();

            foreach (var profile in AssistSettings.Current.Profiles)
            {
                controls.Add(new ProfileControl(profile)
                {
                   
                });
            }

            return controls;
        }
    }
}
