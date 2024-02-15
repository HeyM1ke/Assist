using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.RAccount;

public class RAccountSecondarySelectionControl : TemplatedControl
{
    public static readonly StyledProperty<ICommand?> YesSelectionCommandProperty = AvaloniaProperty.Register<RAccountSecondarySelectionControl, ICommand?>("YesSelectionCommand");
    public static readonly StyledProperty<ICommand?> NoSelectionCommandProperty = AvaloniaProperty.Register<RAccountSecondarySelectionControl, ICommand?>("NoSelectionCommand");

    public ICommand? YesSelectionCommand
    {
        get { return (ICommand?)GetValue(YesSelectionCommandProperty); }
        set { SetValue(YesSelectionCommandProperty, value); }
    }

    public ICommand? NoSelectionCommand
    {
        get { return (ICommand?)GetValue(NoSelectionCommandProperty); }
        set { SetValue(NoSelectionCommandProperty, value); }
    }
}