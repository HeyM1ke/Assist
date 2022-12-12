using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace Assist.Controls.Global.Navigation
{
    public class NavigationButton : TemplatedControl
    {
        public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<NavigationButton, bool>("IsSelected", false);
        public static readonly StyledProperty<bool> IsHoveredProperty = AvaloniaProperty.Register<NavigationButton, bool>("IsHovered", false);
        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<NavigationButton, string>("Title", "Navigation");
        public static readonly StyledProperty<IImage> SourceProperty = AvaloniaProperty.Register<Image, IImage>(nameof(Source));
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public bool IsHovered
        {
            get { return (bool)GetValue(IsHoveredProperty); }
            set { SetValue(IsHoveredProperty, value); }
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
        public NavigationButton()
        {
            SetupHandlers();
        }

        private void SetupHandlers()
        {
            AddHandler(PointerPressedEvent, PointerPressed);
            AddHandler(PointerEnteredEvent, PointerHoverEntered);
            AddHandler(PointerExitedEvent, PointerHoverExited);
        }

        private void PointerHoverExited(object? sender, PointerEventArgs e)
        {
            IsHovered = false;
        }

        private void PointerHoverEntered(object? sender, PointerEventArgs e)
        {
            if(IsSelected) // prevents gray from showing if selected
                return;

            IsHovered = true;
        }

        private void PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            IsSelected = true;
        }
    }
}