using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Assist;

public class LargeImageButton : Button
{
    public static readonly StyledProperty<string?> ImageLinkProperty = AvaloniaProperty.Register<LargeImageButton, string?>("ImageLink");

    public string? ImageLink
    {
        get { return (string?)GetValue(ImageLinkProperty); }
        set { SetValue(ImageLinkProperty, value); }
    }
}