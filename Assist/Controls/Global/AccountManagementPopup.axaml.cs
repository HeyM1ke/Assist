using System;
using Assist.Controls.Global.ViewModels;
using Assist.Services.Popup;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Global;

public partial class AccountManagementPopup : UserControl
{
    private readonly AccountManagementViewModel _viewModel;

    public AccountManagementPopup()
    {
        DataContext = _viewModel = new AccountManagementViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void AccountManagementPopup_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)
            return;

        await _viewModel.Setup();
    }

    private void BackBtn_Click(object? sender, RoutedEventArgs e)
    {
        PopupSystem.KillPopups();
        
    }
}