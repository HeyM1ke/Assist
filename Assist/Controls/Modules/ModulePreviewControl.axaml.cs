using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Modules;

public class ModulePreviewControl : TemplatedControl
{
    public static readonly StyledProperty<string?> ModuleNameProperty = AvaloniaProperty.Register<ModulePreviewControl, string?>("ModuleName", "Module");
    public static readonly StyledProperty<string?> ModuleDescProperty = AvaloniaProperty.Register<ModulePreviewControl, string?>("ModuleDesc");
    public static readonly StyledProperty<ICommand?> ManageButtonCommandProperty = AvaloniaProperty.Register<ModulePreviewControl, ICommand?>("ManageButtonCommand");
    public static readonly StyledProperty<IImage?> ModuleIconProperty = AvaloniaProperty.Register<ModulePreviewControl, IImage?>("ModuleIcon");

    public string? ModuleName
    {
        get { return (string?)GetValue(ModuleNameProperty); }
        set { SetValue(ModuleNameProperty, value); }
    }

    public string? ModuleDesc
    {
        get { return (string?)GetValue(ModuleDescProperty); }
        set { SetValue(ModuleDescProperty, value); }
    }

    public ICommand? ManageButtonCommand
    {
        get { return (ICommand?)GetValue(ManageButtonCommandProperty); }
        set { SetValue(ManageButtonCommandProperty, value); }
    }

    public IImage? ModuleIcon
    {
        get { return (IImage?)GetValue(ModuleIconProperty); }
        set { SetValue(ModuleIconProperty, value); }
    }
}