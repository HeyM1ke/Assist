using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Progression;

public class DailyTicketDiamond : TemplatedControl
{
    public static readonly StyledProperty<bool> IsCompletedProperty = AvaloniaProperty.Register<DailyTicketDiamond, bool>("IsCompleted");
    public static readonly StyledProperty<bool> IsCurrentProperty = AvaloniaProperty.Register<DailyTicketDiamond, bool>("IsCurrent");
    public static readonly StyledProperty<string?> ProgressTextProperty = AvaloniaProperty.Register<DailyTicketDiamond, string?>("ProgressText");
    public static readonly StyledProperty<string?> DiamondNumberProperty = AvaloniaProperty.Register<DailyTicketDiamond, string?>("DiamondNumber");

    public bool IsCompleted
    {
        get { return (bool)GetValue(IsCompletedProperty); }
        set { SetValue(IsCompletedProperty, value); }
    }

    public bool IsCurrent
    {
        get { return (bool)GetValue(IsCurrentProperty); }
        set { SetValue(IsCurrentProperty, value); }
    }

    public string? ProgressText
    {
        get { return (string?)GetValue(ProgressTextProperty); }
        set { SetValue(ProgressTextProperty, value); }
    }

    public string? DiamondNumber
    {
        get { return (string?)GetValue(DiamondNumberProperty); }
        set { SetValue(DiamondNumberProperty, value); }
    }
}