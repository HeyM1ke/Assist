using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace Assist.Controls.Settings
{
    public class SettingsNavigationButton : TemplatedControl
    {
        public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<SettingsNavigationButton, bool>("IsSelected", false);
        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<SettingsNavigationButton, string>("Title", "Title");
        public static readonly StyledProperty<IImage> SourceProperty = AvaloniaProperty.Register<Image, IImage>(nameof(Source));
        public static readonly StyledProperty<bool> IsHoveredProperty = AvaloniaProperty.Register<SettingsNavigationButton, bool>("IsHovered");

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public IImage Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public bool IsHovered
        {
            get { return (bool)GetValue(IsHoveredProperty); }
            set { SetValue(IsHoveredProperty, value); }
        }

        public SettingsNavigationButton()
        {
            AddHandler(PointerPressedEvent, PointerPressed);
        }
        
        private void PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            IsSelected = true;
        }
    }
}