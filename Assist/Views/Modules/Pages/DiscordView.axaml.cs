using Assist.ViewModels.Modules;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Modules.Pages;

public partial class DiscordView : UserControl
{
    private readonly DiscordRPViewModel _viewModel;

    public DiscordView()
    {
        DataContext = _viewModel = new DiscordRPViewModel();
        InitializeComponent();
    }
}