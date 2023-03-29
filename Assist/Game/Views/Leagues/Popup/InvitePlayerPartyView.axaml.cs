using Assist.Game.Views.Leagues.ViewModels;
using Assist.Services.Popup;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Leagues.Popup;

public partial class InvitePlayerPartyView : UserControl
{
    private readonly InvitePlayerPartyViewModel _viewModel;

    public InvitePlayerPartyView()
    {
        DataContext = _viewModel = new InvitePlayerPartyViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void CloseBtn_Click(object? sender, RoutedEventArgs e)
    {
        PopupSystem.KillPopups();
    }

    private async void SendInviteBtn_Click(object? sender, RoutedEventArgs e)
    {
        var textInput = this.FindControl<TextBox>("TextInputBox");
        var btn = sender as Button;
        btn.IsEnabled = false;
        await _viewModel.InvitePlayer(textInput.Text);
        btn.IsEnabled = true;
    }
}