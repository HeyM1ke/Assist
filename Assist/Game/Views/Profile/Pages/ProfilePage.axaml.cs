using System;
using Assist.Game.Views.Profile.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Profile.Pages;

public partial class ProfilePage : UserControl
{
    private readonly ProfilePageViewModel _viewModel;

    public ProfilePage()
    {
        DataContext = _viewModel = new ProfilePageViewModel();
        InitializeComponent();
    }



    private async void ProfilePage_Init(object? sender, EventArgs e)
    {
        _viewModel.Setup();
    }
}