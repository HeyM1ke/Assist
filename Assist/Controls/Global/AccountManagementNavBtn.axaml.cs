using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Global;

public class AccountManagementNavBtn : Button
{
    public static readonly StyledProperty<string?> PlayercardImageProperty = AvaloniaProperty.Register<AccountManagementNavBtn, string?>("PlayercardImage");

    public string? PlayercardImage
    {
        get { return (string?)GetValue(PlayercardImageProperty); }
        set { SetValue(PlayercardImageProperty, value); }
    }
}