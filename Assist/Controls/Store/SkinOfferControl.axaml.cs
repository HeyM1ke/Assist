using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Store;

public class SkinOfferControl : Button
{
    public static readonly StyledProperty<bool> HoveredOverProperty = AvaloniaProperty.Register<SkinOfferControl, bool>("HoveredOver", false);
    public static readonly StyledProperty<IBrush?> TierGradientProperty = AvaloniaProperty.Register<SkinOfferControl, IBrush?>("TierGradient");
    public static readonly StyledProperty<string?> SkinNameProperty = AvaloniaProperty.Register<SkinOfferControl, string?>("SkinName", "Skin");
    public static readonly StyledProperty<string?> SkinImageProperty = AvaloniaProperty.Register<SkinOfferControl, string?>("SkinImage");
    public static readonly StyledProperty<string?> SkinPriceProperty = AvaloniaProperty.Register<SkinOfferControl, string?>("SkinPrice");

    public bool HoveredOver
    {
        get { return (bool)GetValue(HoveredOverProperty); }
        set { SetValue(HoveredOverProperty, value); }
    }

    public IBrush? TierGradient
    {
        get { return (IBrush?)GetValue(TierGradientProperty); }
        set { SetValue(TierGradientProperty, value); }
    }

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

    public string? SkinPrice
    {
        get { return (string?)GetValue(SkinPriceProperty); }
        set { SetValue(SkinPriceProperty, value); }
    }

    public string SkinId;
}