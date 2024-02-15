using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.RAccount;

public class RAccountMethodSelectionControl : TemplatedControl
{
    public static readonly StyledProperty<ICommand?> UsernameButtonCommandProperty = AvaloniaProperty.Register<RAccountMethodSelectionControl, ICommand?>("UsernameButtonCommand");
    public static readonly StyledProperty<ICommand?> ClientButtonCommandProperty = AvaloniaProperty.Register<RAccountMethodSelectionControl, ICommand?>("ClientButtonCommand");
    public static readonly StyledProperty<ICommand?> CloudExperimentCommandProperty = AvaloniaProperty.Register<RAccountMethodSelectionControl, ICommand?>("CloudExperimentCommand");

    public ICommand? UsernameButtonCommand
    {
        get { return (ICommand?)GetValue(UsernameButtonCommandProperty); }
        set { SetValue(UsernameButtonCommandProperty, value); }
    }

    public ICommand? ClientButtonCommand
    {
        get { return (ICommand?)GetValue(ClientButtonCommandProperty); }
        set { SetValue(ClientButtonCommandProperty, value); }
    }

    public ICommand? CloudExperimentCommand
    {
        get { return (ICommand?)GetValue(CloudExperimentCommandProperty); }
        set { SetValue(CloudExperimentCommandProperty, value); }
    }
}