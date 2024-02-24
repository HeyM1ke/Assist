using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Setup;

public class ModeSelectionControl : TemplatedControl
{
    public static readonly StyledProperty<ICommand?> ModeSelectionCommandProperty = AvaloniaProperty.Register<ModeSelectionControl, ICommand?>("ModeSelectionCommand");

    public ICommand? ModeSelectionCommand
    {
        get { return (ICommand?)GetValue(ModeSelectionCommandProperty); }
        set { SetValue(ModeSelectionCommandProperty, value); }
    }
}