using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Assist.Game.Controls.Leagues.Settings;

public class AssistSettingsNavigationButton : Button
{
    public static readonly StyledProperty<string?> TitleTextProperty = AvaloniaProperty.Register<AssistSettingsNavigationButton, string?>("TitleText", "Setting Name");
    public static readonly StyledProperty<string?> MessageTextProperty = AvaloniaProperty.Register<AssistSettingsNavigationButton, string?>("MessageText", "Setting Description");
    public static readonly StyledProperty<IImage?> IconProperty = AvaloniaProperty.Register<AssistSettingsNavigationButton, IImage?>("Icon");

    public string? TitleText
    {
        get { return (string?)GetValue(TitleTextProperty); }
        set { SetValue(TitleTextProperty, value); }
    }

    public string? MessageText
    {
        get { return (string?)GetValue(MessageTextProperty); }
        set { SetValue(MessageTextProperty, value); }
    }

    public IImage? Icon
    {
        get { return (Bitmap?)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
}