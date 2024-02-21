using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Game.Controls.Navigation
{
    public class NavButton : Button
    {
        public static readonly StyledProperty<bool?> IsSelectedProperty = AvaloniaProperty.Register<NavButton, bool?>("IsSelected", false);
        public static readonly StyledProperty<IImage?> IconProperty = AvaloniaProperty.Register<NavButton, IImage?>("Icon");
        public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<NavButton, string?>("Text", "Navigation");
        public static readonly StyledProperty<bool?> IsSpecialProperty = AvaloniaProperty.Register<NavButton, bool?>("IsSpecial", false);
        public static readonly StyledProperty<IBrush?> HighlightColorProperty = AvaloniaProperty.Register<NavButton, IBrush?>("HighlightColor", new SolidColorBrush(new Color(255,19,21,24)));
        public static readonly StyledProperty<bool?> IsCompressedProperty = AvaloniaProperty.Register<NavButton, bool?>("IsCompressed", false);
        public static readonly StyledProperty<bool?> IsFullSizeProperty = AvaloniaProperty.Register<NavButton, bool?>("IsFullSize");

        public bool? IsSelected
        {
            get { return (bool?)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public IImage? Icon
        {
            get { return (IImage?)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public string? Text
        {
            get { return (string?)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool? IsSpecial
        {
            get { return (bool?)GetValue(IsSpecialProperty); }
            set { SetValue(IsSpecialProperty, value); }
        }

        public IBrush? HighlightColor
        {
            get { return (IBrush?)GetValue(HighlightColorProperty); }
            set { SetValue(HighlightColorProperty, value); }
        }

        public bool? IsCompressed
        {
            get { return (bool?)GetValue(IsCompressedProperty); }
            set { SetValue(IsCompressedProperty, value); }
        }

        public bool? IsFullSize
        {
            get { return (bool?)GetValue(IsFullSizeProperty); }
            set { SetValue(IsFullSizeProperty, value); }
        }
    }
}
