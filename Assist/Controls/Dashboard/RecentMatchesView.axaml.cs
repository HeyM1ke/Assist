using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Dashboard;

public class RecentMatchesView : TemplatedControl
{
    public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<PlayerStatisticsView, object?>("Content");
    public static readonly StyledProperty<bool?> isLoadingProperty = AvaloniaProperty.Register<PlayerStatisticsView, bool?>("isLoading", false);

    
    public object? Content
    {
        get { return (object?)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }
        
    public bool? isLoading
    {
        get { return (bool?)GetValue(isLoadingProperty); }
        set { SetValue(isLoadingProperty, value); }
    }
}