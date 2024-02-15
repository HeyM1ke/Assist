using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.General;

public class TwoTextButton : Button
{
    public static readonly StyledProperty<string?> HeaderTextProperty = AvaloniaProperty.Register<TwoTextButton, string?>("HeaderText");
    public static readonly StyledProperty<string?> BodyTextProperty = AvaloniaProperty.Register<TwoTextButton, string?>("BodyText");

    public string? HeaderText
    {
        get { return (string?)GetValue(HeaderTextProperty); }
        set { SetValue(HeaderTextProperty, value); }
    }

    public string? BodyText
    {
        get { return (string?)GetValue(BodyTextProperty); }
        set { SetValue(BodyTextProperty, value); }
    }
}