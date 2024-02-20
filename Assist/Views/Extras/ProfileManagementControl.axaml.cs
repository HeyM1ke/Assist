using System;
using Assist.ViewModels.ProfileSwap;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;

namespace Assist.Views.Extras;

public partial class ProfileManagementControl : UserControl
{
    
    private readonly ProfileManagementViewModel _viewModel;

    public ProfileManagementControl()
    {
        DataContext = _viewModel = new ProfileManagementViewModel();
        InitializeComponent();
    }

    public ProfileManagementControl(string code, IRelayCommand closePopupCommand)
    {
        DataContext = _viewModel = new ProfileManagementViewModel();
        
        InitializeComponent();
        _viewModel.ProfileId = code;
        _viewModel.ClosePopup = closePopupCommand;
    }

    private void ProfileManage_Int(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)return;
        _viewModel.Setup();
    }
}