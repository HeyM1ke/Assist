using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Assist.Authentication;

public class OAuthMethodButton : Button
{
    public static readonly StyledProperty<IImage?> IconProperty = AvaloniaProperty.Register<OAuthMethodButton, IImage?>("Icon");
    public static readonly StyledProperty<IBrush?> HoverColorProperty = AvaloniaProperty.Register<OAuthMethodButton, IBrush?>("HoverColor");

    public IImage? Icon
    {
        get { return (IImage?)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public IBrush? HoverColor
    {
        get { return (IBrush?)GetValue(HoverColorProperty); }
        set { SetValue(HoverColorProperty, value); }
    }
}