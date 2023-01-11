using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Game.Controls.Modules
{
    public class ModuleSelectionControl : TemplatedControl
    {
        public static readonly StyledProperty<string?> ModuleNameProperty = AvaloniaProperty.Register<ModuleSelectionControl, string?>("ModuleName", "Module");
        public static readonly StyledProperty<string?> ModuleDescriptionProperty = AvaloniaProperty.Register<ModuleSelectionControl, string?>("ModuleDescription", "This is a description.");
        public static readonly StyledProperty<IImage?> ModuleIconProperty = AvaloniaProperty.Register<ModuleSelectionControl, IImage?>("ModuleIcon");

        public string? ModuleName
        {
            get { return (string?)GetValue(ModuleNameProperty); }
            set { SetValue(ModuleNameProperty, value); }
        }

        public string? ModuleDescription
        {
            get { return (string?)GetValue(ModuleDescriptionProperty); }
            set { SetValue(ModuleDescriptionProperty, value); }
        }

        public IImage? ModuleIcon
        {
            get { return (IImage?)GetValue(ModuleIconProperty); }
            set { SetValue(ModuleIconProperty, value); }
        }
    }
}
