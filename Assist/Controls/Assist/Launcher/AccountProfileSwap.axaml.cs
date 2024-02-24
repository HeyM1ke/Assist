using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;

namespace Assist.Controls.Assist.Launcher;

public class AccountProfileSwap : TemplatedControl
{
    public static readonly StyledProperty<string?> ProfileIconProperty = AvaloniaProperty.Register<AccountProfileSwap, string?>("ProfileIcon");
    public static readonly StyledProperty<string?> ProfileNameProperty = AvaloniaProperty.Register<AccountProfileSwap, string?>("ProfileName");
    public static readonly StyledProperty<ICommand?> SwapButtonCommandProperty = AvaloniaProperty.Register<AccountProfileSwap, ICommand?>("SwapButtonCommand");
    public static readonly StyledProperty<bool?> SwapEnabledProperty = AvaloniaProperty.Register<AccountProfileSwap, bool?>("SwapEnabled", true);

    public string? ProfileIcon
    {
        get { return (string?)GetValue(ProfileIconProperty); }
        set { SetValue(ProfileIconProperty, value); }
    }

    public string? ProfileName
    {
        get { return (string?)GetValue(ProfileNameProperty); }
        set { SetValue(ProfileNameProperty, value); }
    }

    public ICommand? SwapButtonCommand
    {
        get { return (ICommand?)GetValue(SwapButtonCommandProperty); }
        set { SetValue(SwapButtonCommandProperty, value); }
    }

    public bool? SwapEnabled
    {
        get { return (bool?)GetValue(SwapEnabledProperty); }
        set { SetValue(SwapEnabledProperty, value); }
    }
}