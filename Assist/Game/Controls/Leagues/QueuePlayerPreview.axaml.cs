using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.Leagues;

public class QueuePlayerPreview : TemplatedControl
{
    public static readonly StyledProperty<string?> ImageUrlProperty = AvaloniaProperty.Register<QueuePlayerPreview, string?>("ImageUrl");
    public static readonly StyledProperty<string?> PlayerNameProperty = AvaloniaProperty.Register<QueuePlayerPreview, string?>("PlayerName");

    public string? ImageUrl
    {
        get { return (string?)GetValue(ImageUrlProperty); }
        set { SetValue(ImageUrlProperty, value); }
    }

    public string? PlayerName
    {
        get { return (string?)GetValue(PlayerNameProperty); }
        set { SetValue(PlayerNameProperty, value); }
    }
}