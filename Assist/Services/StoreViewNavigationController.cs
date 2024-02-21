using Avalonia.Controls;

namespace Assist.Services;

public class StoreViewNavigationController
{
    public static StorePage CurrentPage;
    public static TransitioningContentControl ContentControl = new TransitioningContentControl();

    public static void Change(UserControl c) => ContentControl.Content = c;
    
    
}

public enum StorePage
{
    STORE,
    NIGHTMARKET,
    ACCESSORIES
}