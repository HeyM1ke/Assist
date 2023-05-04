using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace Assist.Controls.Global
{
    public class SocialButton : Button
    {
        public static readonly StyledProperty<IImage?> SourceProperty = AvaloniaProperty.Register<SocialButton, IImage?>("Source");
        public static readonly StyledProperty<string?> LinkToProperty = AvaloniaProperty.Register<SocialButton, string?>("LinkTo", "https://assistapp.dev/");

        public SocialButton()
        {
            AddHandler(PointerPressedEvent, PressedEvent);
        }

        public IImage? Source
        {
            get { return (IImage?)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public string? LinkTo
        {
            get { return (string)GetValue(LinkToProperty); }
            set { SetValue(LinkToProperty, value); }
        }

        private void PressedEvent(object? sender, PointerPressedEventArgs e)
        {
            if (LinkTo == null)
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = LinkTo,
                UseShellExecute = true
            });
        }
    }
}
