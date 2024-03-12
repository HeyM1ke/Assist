using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.ProfileSwap;

public class AccProfilePreviewControl : TemplatedControl
{
    public static readonly StyledProperty<string?> PlayerNameProperty = AvaloniaProperty.Register<AccProfilePreviewControl, string?>("PlayerName");
    public static readonly StyledProperty<string?> RegionTextProperty = AvaloniaProperty.Register<AccProfilePreviewControl, string?>("RegionText");
    public static readonly StyledProperty<bool?> AssistEnabledProperty = AvaloniaProperty.Register<AccProfilePreviewControl, bool?>("AssistEnabled");
    public static readonly StyledProperty<bool?> GameLaunchEnabledProperty = AvaloniaProperty.Register<AccProfilePreviewControl, bool?>("GameLaunchEnabled");
    public static readonly StyledProperty<bool?> IsExpiredProperty = AvaloniaProperty.Register<AccProfilePreviewControl, bool?>("IsExpired");
    public static readonly StyledProperty<string?> PlayerIconImageProperty = AvaloniaProperty.Register<AccProfilePreviewControl, string?>("PlayerIconImage");
    public static readonly StyledProperty<bool> IsCurrentProperty = AvaloniaProperty.Register<AccProfilePreviewControl, bool>("IsCurrent");
    public static readonly StyledProperty<ICommand?> SwitchCommandProperty = AvaloniaProperty.Register<AccProfilePreviewControl, ICommand?>("SwitchCommand");
    public static readonly StyledProperty<string?> AccountIdProperty = AvaloniaProperty.Register<AccProfilePreviewControl, string?>("AccountId");
    public static readonly StyledProperty<ICommand?> ManageCommandProperty = AvaloniaProperty.Register<AccProfilePreviewControl, ICommand?>("ManageCommand");
    public static readonly StyledProperty<string?> PlayerRankImageProperty = AvaloniaProperty.Register<AccProfilePreviewControl, string?>("PlayerRankImage");

    public string? PlayerName
    {
        get { return (string?)GetValue(PlayerNameProperty); }
        set { SetValue(PlayerNameProperty, value); }
    }

    public string? RegionText
    {
        get { return (string?)GetValue(RegionTextProperty); }
        set { SetValue(RegionTextProperty, value); }
    }

    public bool? AssistEnabled
    {
        get { return (bool?)GetValue(AssistEnabledProperty); }
        set { SetValue(AssistEnabledProperty, value); }
    }

    public bool? GameLaunchEnabled
    {
        get { return (bool?)GetValue(GameLaunchEnabledProperty); }
        set { SetValue(GameLaunchEnabledProperty, value); }
    }

    public bool? IsExpired
    {
        get { return (bool?)GetValue(IsExpiredProperty); }
        set { SetValue(IsExpiredProperty, value); }
    }

    public string? PlayerIconImage
    {
        get { return (string?)GetValue(PlayerIconImageProperty); }
        set { SetValue(PlayerIconImageProperty, value); }
    }

    public bool IsCurrent
    {
        get { return !(bool)!GetValue(IsCurrentProperty); }
        set { SetValue(IsCurrentProperty, !value); }
    }

    public ICommand? SwitchCommand
    {
        get { return (ICommand?)GetValue(SwitchCommandProperty); }
        set { SetValue(SwitchCommandProperty, value); }
    }

    public string? AccountId
    {
        get { return (string?)GetValue(AccountIdProperty); }
        set { SetValue(AccountIdProperty, value); }
    }

    public ICommand? ManageCommand
    {
        get { return (ICommand?)GetValue(ManageCommandProperty); }
        set { SetValue(ManageCommandProperty, value); }
    }

    public string? PlayerRankImage
    {
        get { return (string?)GetValue(PlayerRankImageProperty); }
        set { SetValue(PlayerRankImageProperty, value); }
    }
}