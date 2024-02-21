using System;
using Assist.Controls.Assist.ViewModels;
using Assist.Game.Views.Profile;
using Assist.Game.Views.Profile.Pages;
using Assist.Services.Popup;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;

namespace Assist.Controls.Assist
{
    public partial class UserPreview : UserControl
    {
        private readonly UserPreviewViewModel _viewModel;

        public UserPreview()
        {
            DataContext = _viewModel = new UserPreviewViewModel();
            InitializeComponent();
        }


        private async void UserPreview_Initialized(object? sender, EventArgs e)
        {
            if(Design.IsDesignMode)
                return;

            // Get User Profile Data
            await _viewModel.SetupProfile();
        }

        private void GridClicked(object? sender, PointerReleasedEventArgs e)
        {
            if(Design.IsDesignMode)
                return;
            
            PopupSystem.SpawnCustomPopup(new ProfileView());
        }
    }
}
