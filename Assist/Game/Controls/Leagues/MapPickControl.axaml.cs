using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Assist.Game.Controls.Leagues;

public class MapPickControl : Button
{
    public static readonly StyledProperty<string?> MapImageProperty = AvaloniaProperty.Register<MapPickControl, string?>("MapImage");
    public static readonly StyledProperty<bool> IsBannedProperty = AvaloniaProperty.Register<MapPickControl, bool>("IsBanned", false);
    public static readonly StyledProperty<string?> MapNameProperty = AvaloniaProperty.Register<MapPickControl, string?>("MapName", "MAPNAME");
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<MapPickControl, bool>("IsSelected");

    public string? MapImage
    {
        get { return (string?)GetValue(MapImageProperty); }
        set { SetValue(MapImageProperty, value); }
    }

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

    public bool IsSelected
    {
        get { return (bool)GetValue(IsSelectedProperty); }
        set { SetValue(IsSelectedProperty, value); }
    }

    public MapPickControl()
    {
        AddHandler(ClickEvent, ClickChangeDesign);
    }

    private void ClickChangeDesign(object? sender, RoutedEventArgs e)
    {
        if (IsBanned)
            return;

        IsSelected = true;
    }
}