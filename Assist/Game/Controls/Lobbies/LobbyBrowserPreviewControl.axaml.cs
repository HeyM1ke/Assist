using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.Lobbies;

public class LobbyBrowserPreviewControl : TemplatedControl
{
    public static readonly StyledProperty<string?> LobbyNameProperty = AvaloniaProperty.Register<LobbyBrowserPreviewControl, string?>("LobbyName", "ASSIST LOBBY");
    public static readonly StyledProperty<string?> CurrentSizeProperty = AvaloniaProperty.Register<LobbyBrowserPreviewControl, string?>("CurrentSize", "0");
    public static readonly StyledProperty<string?> MaxSizeProperty = AvaloniaProperty.Register<LobbyBrowserPreviewControl, string?>("MaxSize", "12");
    public static readonly StyledProperty<bool?> IsPasswordProtectedProperty = AvaloniaProperty.Register<LobbyBrowserPreviewControl, bool?>("IsPasswordProtected", false);
    public static readonly StyledProperty<string?> LobbyCodeProperty = AvaloniaProperty.Register<LobbyBrowserPreviewControl, string?>("LobbyCode");

    public string? LobbyName
    {
        get { return (string?)GetValue(LobbyNameProperty); }
        set { SetValue(LobbyNameProperty, value); }
    }

    public string? CurrentSize
    {
        get { return (string?)GetValue(CurrentSizeProperty); }
        set { SetValue(CurrentSizeProperty, value); }
    }

    public string? MaxSize
    {
        get { return (string?)GetValue(MaxSizeProperty); }
        set { SetValue(MaxSizeProperty, value); }
    }

    public bool? IsPasswordProtected
    {
        get { return (bool?)GetValue(IsPasswordProtectedProperty); }
        set { SetValue(IsPasswordProtectedProperty, value); }
    }

    public string? LobbyCode
    {
        get { return (string?)GetValue(LobbyCodeProperty); }
        set { SetValue(LobbyCodeProperty, value); }
    }
}