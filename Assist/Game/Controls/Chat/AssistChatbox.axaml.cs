using System;
using Assist.Game.Controls.Chat.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Controls.Chat;

public partial class AssistChatbox : UserControl
{
    private readonly AssistChatboxViewModel _viewModel;

    public AssistChatbox()
    {
        DataContext = _viewModel = new AssistChatboxViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public async void AddChat(string Name, string GroupId)
    {
        await _viewModel.AddChat(Name, GroupId);
    }

    private void StyledElement_OnInitialized(object? sender, EventArgs e)
    {
        AddChat("Test", "testChat");
    }
}