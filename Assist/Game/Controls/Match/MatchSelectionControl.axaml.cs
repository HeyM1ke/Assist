using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.Match;

public class MatchSelectionControl : ToggleButton
{
    public static readonly StyledProperty<bool> IsBannedProperty = AvaloniaProperty.Register<MatchSelectionControl, bool>("IsBanned", false);
    public static readonly StyledProperty<string?> MapNameProperty = AvaloniaProperty.Register<MatchSelectionControl, string?>("MapName", "Default");
    public static readonly StyledProperty<string?> MapImageProperty = AvaloniaProperty.Register<MatchSelectionControl, string?>("MapImage");

    public bool IsBanned
    {
        get { return (bool)GetValue(IsBannedProperty); }
        set { SetValue(IsBannedProperty, value); }
    }

    public string? MapName
    {
        get { return (string?)GetValue(MapNameProperty); }
        set { SetValue(MapNameProperty, value); }
    }

    public string? MapImage
    {
        get { return (string?)GetValue(MapImageProperty); }
        set { SetValue(MapImageProperty, value); }
    }
}