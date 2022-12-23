using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.Modules.Dodge
{
    public class DodgeUserButton : Button
    {
        public static readonly StyledProperty<string?> GameNameProperty = AvaloniaProperty.Register<DodgeUserButton, string?>("GameName");
        public static readonly StyledProperty<string?> NoteProperty = AvaloniaProperty.Register<DodgeUserButton, string?>("Note");
        public static readonly StyledProperty<string?> DateAddedProperty = AvaloniaProperty.Register<DodgeUserButton, string?>("DateAdded");

        public string? GameName
        {
            get { return (string?)GetValue(GameNameProperty); }
            set { SetValue(GameNameProperty, value); }
        }

        public string? Note
        {
            get { return (string?)GetValue(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }

        public string? DateAdded
        {
            get { return (string?)GetValue(DateAddedProperty); }
            set { SetValue(DateAddedProperty, value); }
        }
    }
}
