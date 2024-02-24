using Assist.Core.Helpers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Assist.Controls.Startup;

public class AccountPreviewStartupControl : TemplatedControl
{
    public static readonly StyledProperty<string?> IconProperty = AvaloniaProperty.Register<AccountPreviewStartupControl, string?>("Icon");
    public static readonly StyledProperty<string?> AccountNameProperty = AvaloniaProperty.Register<AccountPreviewStartupControl, string?>("AccountName");
    public static readonly StyledProperty<string?> AccountRegionProperty = AvaloniaProperty.Register<AccountPreviewStartupControl, string?>("AccountRegion");

    public string? Icon
    {
        get { return (string?)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public string? AccountName
    {
        get { return (string?)GetValue(AccountNameProperty); }
        set { SetValue(AccountNameProperty, value); }
    }

    public string? AccountRegion
    {
        get { return (string?)GetValue(AccountRegionProperty); }
        set { SetValue(AccountRegionProperty, value); }
    }
}