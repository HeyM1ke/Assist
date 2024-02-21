using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Store;

public class BundleStoreOfferV2 : Button
{
    public static readonly StyledProperty<string?> BundlePriceProperty = AvaloniaProperty.Register<BundleStoreOfferV2, string?>("BundlePrice");
    public static readonly StyledProperty<string?> BundleNameProperty = AvaloniaProperty.Register<BundleStoreOfferV2, string?>("BundleName");
    public static readonly StyledProperty<string?> BundleImageProperty = AvaloniaProperty.Register<BundleStoreOfferV2, string?>("BundleImage");

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
}