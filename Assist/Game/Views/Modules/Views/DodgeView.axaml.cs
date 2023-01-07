using System;
using Assist.Game.Controls.Modules.Dodge.Popup;
using Assist.Game.Services;
using Assist.Game.Views.Modules.Views.ViewModels;
using Assist.Services.Popup;
using Assist.Settings;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Assist.Game.Views.Modules.Views
{
    public partial class DodgeView : UserControl
    {
        private readonly DodgeViewViewModel _viewModel;

        public DodgeView()
        {
            ModulesViewNavigationController.CurrentPage = ModulePage.DODGE;
            DataContext = _viewModel = new DodgeViewViewModel();
            InitializeComponent();
        }

        private async void DodgeViewLoaded_Init(object? sender, EventArgs e)
        {
            _viewModel.IsLoading = true;
            await _viewModel.LoadUsers();
            _viewModel.IsLoading = false;
        }

        private void DodgeAdd_Click(object? sender, RoutedEventArgs e)
        {
            PopupSystem.SpawnCustomPopup(new DodgeAddPopup());
        }

        private void GlobalCheckbox_OnUnchecked(object? sender, RoutedEventArgs e)
        {
            _viewModel.IsGlobalEnabled = GameSettings.Current.GlobalListEnabled = false;
            GameSettings.Save();
        }

        private void GlobalCheckbox_OnChecked(object? sender, RoutedEventArgs e)
        {
            _viewModel.IsGlobalEnabled = GameSettings.Current.GlobalListEnabled = true;
            GameSettings.Save();
        }
    }
}
