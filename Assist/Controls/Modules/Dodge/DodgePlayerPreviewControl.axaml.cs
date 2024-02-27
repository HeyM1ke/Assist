using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Modules.Dodge;

public class DodgePlayerPreviewControl : TemplatedControl
{
    public string PlayerId;
    public static readonly StyledProperty<string?> PlayerNameProperty = AvaloniaProperty.Register<DodgePlayerPreviewControl, string?>("PlayerName");
    public static readonly StyledProperty<string?> PlayerCategoryProperty = AvaloniaProperty.Register<DodgePlayerPreviewControl, string?>("PlayerCategory");
    public static readonly StyledProperty<string?> PlayerNoteProperty = AvaloniaProperty.Register<DodgePlayerPreviewControl, string?>("PlayerNote");
    public static readonly StyledProperty<bool> NoteEnabledProperty = AvaloniaProperty.Register<DodgePlayerPreviewControl, bool>("NoteEnabled", false);
    public static readonly StyledProperty<string?> DateAddedProperty = AvaloniaProperty.Register<DodgePlayerPreviewControl, string?>("DateAdded");

    public string? PlayerName
    {
        get { return (string?)GetValue(PlayerNameProperty); }
        set { SetValue(PlayerNameProperty, value); }
    }

    public string? PlayerCategory
    {
        get { return (string?)GetValue(PlayerCategoryProperty); }
        set { SetValue(PlayerCategoryProperty, value); }
    }

    public string? PlayerNote
    {
        get { return (string?)GetValue(PlayerNoteProperty); }
        set { SetValue(PlayerNoteProperty, value); }
    }

    public bool NoteEnabled
    {
        get { return (bool)GetValue(NoteEnabledProperty); }
        set { SetValue(NoteEnabledProperty, value); }
    }

    public string? DateAdded
    {
        get { return (string?)GetValue(DateAddedProperty); }
        set { SetValue(DateAddedProperty, value); }
    }
}