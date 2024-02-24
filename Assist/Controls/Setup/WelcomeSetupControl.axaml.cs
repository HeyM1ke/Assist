using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Setup;

public class WelcomeSetupControl : TemplatedControl
{
    public static readonly StyledProperty<ICommand?> NextButtonCommandProperty = AvaloniaProperty.Register<WelcomeSetupControl, ICommand?>("NextButtonCommand");

    public ICommand? NextButtonCommand
    {
        get { return (ICommand?)GetValue(NextButtonCommandProperty); }
        set { SetValue(NextButtonCommandProperty, value); }
    }
}