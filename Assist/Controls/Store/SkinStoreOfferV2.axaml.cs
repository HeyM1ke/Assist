using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Store;

public class SkinStoreOfferV2 : Button
{
    public static readonly StyledProperty<string?> SkinCostProperty = AvaloniaProperty.Register<SkinStoreOfferV2, string?>("SkinCost");
    public static readonly StyledProperty<string?> SkinNameProperty = AvaloniaProperty.Register<SkinStoreOfferV2, string?>("SkinName");
    public static readonly StyledProperty<string?> SkinImageProperty = AvaloniaProperty.Register<SkinStoreOfferV2, string?>("SkinImage");

    public string? SkinCost
    {
        get { return (string?)GetValue(SkinCostProperty); }
        set { SetValue(SkinCostProperty, value); }
    }

    public string? SkinId { get; set; }

    public string? SkinName
    {
        get { return (string?)GetValue(SkinNameProperty); }
        set { SetValue(SkinNameProperty, value); }
    }

    public string? SkinImage
    {
        get { return (string?)GetValue(SkinImageProperty); }
        set { SetValue(SkinImageProperty, value); }
    }
}