using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.Leagues;

public class LeagueAnnouncementMemberControl : TemplatedControl
{
    public static readonly StyledProperty<string?> MessageProperty = AvaloniaProperty.Register<LeagueAnnouncementMemberControl, string?>("Message", "Announcement Message");
    public static readonly StyledProperty<string?> TitleProperty = AvaloniaProperty.Register<LeagueAnnouncementMemberControl, string?>("Title", "Announcement Title");

    public string? Message
    {
        get { return (string?)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }

    public string? Title
    {
        get { return (string?)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }
}