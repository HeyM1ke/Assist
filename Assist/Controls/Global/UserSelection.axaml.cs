using System;
using Assist.Controls.Global.ViewModels;
using Assist.ViewModels;
using AsyncImageLoader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Controls.Global
{
    public partial class UserSelection : UserControl
    {
        private readonly UserSelectionViewModel _viewModel;
        private ItemsControl _SelectionItems;
        public UserSelection()
        {
            DataContext = _viewModel = new UserSelectionViewModel();
            
            if (AssistApplication.Current.CurrentProfile != null)
                _viewModel.Profile = AssistApplication.Current.CurrentProfile;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void UserSelection_OnInitialized(object? sender, EventArgs e)
        {
            _viewModel.LoadProfiles();

            var img = this.FindControl<AdvancedImage>("UserProfilePic");

            if (img != null)
            {
                img.Source = _viewModel.ProfilePicture;
            }
        }

        private void MainBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            _viewModel.PopupOpen = !_viewModel.PopupOpen;
        }

        private void UserSelectionPopup_OnPointerLeave(object? sender, PointerEventArgs e)
        {
            Log.Fatal("This");
            _viewModel.PopupOpen = false;
        }
    }
}
