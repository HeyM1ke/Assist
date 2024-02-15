using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Setup;

public class LangSetupControl : TemplatedControl
{
    public static readonly StyledProperty<ICommand?> LangButtonCommandProperty = AvaloniaProperty.Register<LangSetupControl, ICommand?>("LangButtonCommand");
    public ICommand? LangButtonCommand
    {
        get { return (ICommand?)GetValue(LangButtonCommandProperty); }
        set { SetValue(LangButtonCommandProperty, value); }
    }
}