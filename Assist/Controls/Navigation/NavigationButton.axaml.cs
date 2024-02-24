using Assist.Services.Navigation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Navigation;

public class NavigationButton : RadioButton
{
    public static readonly StyledProperty<IImage?> IconProperty = AvaloniaProperty.Register<NavigationButton, IImage?>("Icon");
    public static readonly StyledProperty<bool> TextVisibleProperty = AvaloniaProperty.Register<NavigationButton, bool>("TextVisible", false);
    public static readonly StyledProperty<string?> NavigationTextProperty = AvaloniaProperty.Register<NavigationButton, string?>("NavigationText", "Test");
    public static readonly StyledProperty<bool?> DisabledProperty = AvaloniaProperty.Register<NavigationButton, bool?>("Disabled");

    public IImage? Icon
    {
        get { return (IImage?)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public bool TextVisible
    {
        get { return (bool)GetValue(TextVisibleProperty); }
        set { SetValue(TextVisibleProperty, value); }
    }

    public string? NavigationText
    {
        get { return (string?)GetValue(NavigationTextProperty); }
        set { SetValue(NavigationTextProperty, value); }
    }

    public AssistPage Page { get; set; }

    public bool? Disabled
    {
        get { return (bool?)GetValue(DisabledProperty); }
        set { SetValue(DisabledProperty, value); }
    }
}