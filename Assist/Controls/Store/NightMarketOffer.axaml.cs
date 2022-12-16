using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Store
{
    public class NightMarketOffer : TemplatedControl
    {
        public static readonly StyledProperty<IBrush?> BackgroundBrushProperty = AvaloniaProperty.Register<NightMarketOffer, IBrush?>("BackgroundBrush");
        public static readonly StyledProperty<string?> SkinNameProperty = AvaloniaProperty.Register<NightMarketOffer, string?>("SkinName");
        public static readonly StyledProperty<string?> SkinDiscountProperty = AvaloniaProperty.Register<NightMarketOffer, string?>("SkinDiscount");
        public static readonly StyledProperty<string?> OriginalPriceProperty = AvaloniaProperty.Register<NightMarketOffer, string?>("OriginalPrice");
        public static readonly StyledProperty<string?> DiscountedPriceProperty = AvaloniaProperty.Register<NightMarketOffer, string?>("DiscountedPrice");
        public static readonly StyledProperty<string?> SkinImageProperty = AvaloniaProperty.Register<NightMarketOffer, string?>("SkinImage");

        public IBrush? BackgroundBrush
        {
            get { return (IBrush?)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        public string? SkinName
        {
            get { return (string?)GetValue(SkinNameProperty); }
            set { SetValue(SkinNameProperty, value.ToUpper()); }
        }

        public string? SkinDiscount
        {
            get { return (string?)GetValue(SkinDiscountProperty); }
            set { SetValue(SkinDiscountProperty, value); }
        }

        public string? OriginalPrice
        {
            get { return (string?)GetValue(OriginalPriceProperty); }
            set { SetValue(OriginalPriceProperty, value); }
        }

        public string? DiscountedPrice
        {
            get { return (string?)GetValue(DiscountedPriceProperty); }
            set { SetValue(DiscountedPriceProperty, value); }
        }

        public string? SkinImage
        {
            get { return (string?)GetValue(SkinImageProperty); }
            set { SetValue(SkinImageProperty, value); }
        }
    }
}
