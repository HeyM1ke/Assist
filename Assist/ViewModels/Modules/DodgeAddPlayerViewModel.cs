using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Assist.ViewModels.Modules;

public partial class DodgeAddPlayerViewModel : ViewModelBase
{
    
    [ObservableProperty] private ICommand? _closeViewCommand;
}