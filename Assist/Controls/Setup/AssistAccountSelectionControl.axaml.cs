using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Setup;

public class AssistAccountSelectionControl : TemplatedControl
{
    public static readonly StyledProperty<ICommand?> AccountSelectionCommandProperty = AvaloniaProperty.Register<AssistAccountSelectionControl, ICommand?>("AccountSelectionCommand");

    public ICommand? AccountSelectionCommand
    {
        get { return (ICommand?)GetValue(AccountSelectionCommandProperty); }
        set { SetValue(AccountSelectionCommandProperty, value); }
    }
}