using Assist.Models.Enums;
using Assist.ViewModels;
using Assist.ViewModels.Modules;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Modules;

public partial class ModulesView : UserControl
{
    private readonly ModulesViewModel _viewModel;

    public ModulesView()
    {
        DataContext = _viewModel = new ModulesViewModel();
        InitializeComponent();
    }

    private void ModulePage_Loaded(object? sender, RoutedEventArgs e)
    {
        _viewModel.IsAssistLoggedIn = !string.IsNullOrEmpty(AssistApplication.AssistUser.userTokens.AccessToken);
        _viewModel.IsGameMode = AssistApplication.CurrentMode == EAssistMode.GAME; 
    }
}