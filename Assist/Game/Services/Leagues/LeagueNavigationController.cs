using Avalonia.Controls;

namespace Assist.Game.Services.Leagues;

public class LeagueNavigationController
{
    
    public static TransitioningContentControl ContentControl = new TransitioningContentControl();

    public static void Change(UserControl c) => ContentControl.Content = c;
}