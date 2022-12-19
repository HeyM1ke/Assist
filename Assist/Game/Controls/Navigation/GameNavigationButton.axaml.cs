using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Assist.Game.Controls.Navigation
{
    public class GameNavigationButton : TemplatedControl
    {
        public static readonly StyledProperty<string?> TitleProperty = AvaloniaProperty.Register<GameNavigationButton, string?>("Title", "Page");
        public static readonly StyledProperty<bool?> IsSelectedProperty = AvaloniaProperty.Register<GameNavigationButton, bool?>("IsSelected", false);

        public string? Title
        {
            get { return (string?)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public bool? IsSelected
        {
            get { return (bool?)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public GameNavigationButton()
        {
            AddHandler(PointerPressedEvent, PointerPressed_Event);
        }

        private void PointerPressed_Event(object? sender, PointerPressedEventArgs e)
        {
            IsSelected = true;
        }
    }
}
