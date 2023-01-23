using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Game.Controls.Lobbies;

public partial class LobbyJoinButton : UserControl
{
    
    public static readonly StyledProperty<string> AssistLobbyIdProperty = AvaloniaProperty.Register<LobbyJoinButton, string>(nameof(AssistLobbyId), defaultValue: "0-0-0-0");

    public string AssistLobbyId
    {
        get { return GetValue(AssistLobbyIdProperty); }
        set { SetValue(AssistLobbyIdProperty, value); }
    }
    public LobbyJoinButton()
    {
        InitializeComponent();
        AddHandler(PointerPressedEvent, PointerPressed_Event);
    }

    private void PointerPressed_Event(object? sender, PointerPressedEventArgs e)
    {
        Log.Information("Pointer Pressed on LobbyJoinBtn, Attempting ID of: " + AssistLobbyId);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}