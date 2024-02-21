using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Profile;

public class ProfileBadgeShowcase : Button
{
    public static readonly StyledProperty<bool> IsFeaturedProperty = AvaloniaProperty.Register<ProfileBadgeShowcase, bool>("IsFeatured", false);
    public static readonly StyledProperty<string?> BadgeIdProperty = AvaloniaProperty.Register<ProfileBadgeShowcase, string?>("BadgeId", "16ca2dd3-16d4-4ee1-a273-865b94cd5162");
    public static readonly StyledProperty<string?> BadgeImageUrlProperty = AvaloniaProperty.Register<ProfileBadgeShowcase, string?>("BadgeImageUrl", "https://content.assistapp.dev/badges/16ca2dd3-16d4-4ee1-a273-865b94cd5162.png");

    public bool IsFeatured
    {
        get { return (bool)GetValue(IsFeaturedProperty); }
        set { SetValue(IsFeaturedProperty, value); }
    }

    public string? BadgeId
    {
        get { return (string?)GetValue(BadgeIdProperty); }
        set { SetValue(BadgeIdProperty, value); }
    }

    public string? BadgeImageUrl
    {
        get { return (string?)GetValue(BadgeImageUrlProperty); }
        set { SetValue(BadgeImageUrlProperty, value); }
    }
}