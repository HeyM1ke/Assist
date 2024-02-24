using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Startup;

public class BasicStartupControl : TemplatedControl
{
    public static readonly StyledProperty<string?> MessageProperty = AvaloniaProperty.Register<BasicStartupControl, string?>("Message", "Loading...");

    public string? Message
    {
        get { return (string?)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }
}