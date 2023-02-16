using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.GDashboard
{
    public class GlobalChatMessageControl : TemplatedControl
    {
        public static readonly StyledProperty<string?> MessageProperty = AvaloniaProperty.Register<GlobalChatMessageControl, string?>("Message");
        public static readonly StyledProperty<string?> TimeStampProperty = AvaloniaProperty.Register<GlobalChatMessageControl, string?>("TimeStamp");
        public static readonly StyledProperty<string?> UsernameProperty = AvaloniaProperty.Register<GlobalChatMessageControl, string?>("Username");
        public static readonly StyledProperty<object> BadgesProperty = AvaloniaProperty.Register<GlobalChatMessageControl, object>("Badges");

        public string? Message
        {
            get { return (string?)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string? TimeStamp
        {
            get { return (string?)GetValue(TimeStampProperty); }
            set { SetValue(TimeStampProperty, value); }
        }

        public string? Username
        {
            get { return (string?)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public object Badges
        {
            get { return (object)GetValue(BadgesProperty); }
            set { SetValue(BadgesProperty, value); }
        }
    }
}
