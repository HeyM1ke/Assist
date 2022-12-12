using Avalonia.Controls;

namespace Assist.Services
{
    public class AuthenticationNavigationController
    {
        
        public static TransitioningContentControl ContentControl = new TransitioningContentControl();

        public static void Change(UserControl c) => ContentControl.Content = c;
    }
}