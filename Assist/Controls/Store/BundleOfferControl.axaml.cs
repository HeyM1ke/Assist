using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Store;

public class BundleOfferControl : TemplatedControl
{
    public static readonly StyledProperty<string?> BundlePriceProperty = AvaloniaProperty.Register<BundleOfferControl, string?>("BundlePrice");
    public static readonly StyledProperty<string?> BundleNameProperty = AvaloniaProperty.Register<BundleOfferControl, string?>("BundleName");
    public static readonly StyledProperty<string?> BundleImageProperty = AvaloniaProperty.Register<BundleOfferControl, string?>("BundleImage");

    public string? BundlePrice
    {
        get { return (string?)GetValue(BundlePriceProperty); }
        set { SetValue(BundlePriceProperty, value); }
    }

    public string? BundleName
    {
        get { return (string?)GetValue(BundleNameProperty).ToUpper(); }
        set { SetValue(BundleNameProperty, value.ToUpper()); }
    }

    public string? BundleImage
    {
        get { return (string?)GetValue(BundleImageProperty); }
        set { SetValue(BundleImageProperty, value); }
    }

    public string BundleId;
}