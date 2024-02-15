using System.Threading.Tasks;
using Assist.Shared.Settings;
using Assist.Models.Enums;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;

namespace Assist.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    
    [ObservableProperty]
    private UserControl _currentView;
    
    [ObservableProperty]
    private UserControl _currentPopup;

    [ObservableProperty]
    private double _scaleRate = 1.0;

    [ObservableProperty]
    private int _height = 720;

    [ObservableProperty]
    private int _width = 1190;
    
    public void DetermineScaleRate()
    {
        ChangeResolution(AssistSettings.Default.SelectedResolution);
    }
    
    public void ChangeResolution(EResolution Res)
    {
        if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Window mainRef = desktop.MainWindow;

            if (mainRef.Screens.Primary.WorkingArea.Height <= 1080 && (int)Res >= 2 )
            {
                AssistSettings.Default.SelectedResolution = EResolution.R900;
                Res = AssistSettings.Default.SelectedResolution;
            }

            switch (Res)
            {
                case EResolution.R360:
                    Width = 595;
                    Height = 375;
                    ScaleRate = .5;
                    break;
                case EResolution.R540:
                    Width = 893;
                    Height = 550;
                    ScaleRate = .75;
                    break;
                case EResolution.R900:
                    Width = 1488;
                    Height = 890;
                    ScaleRate = 1.25;
                    break;
                case EResolution.R1080:
                    Width = 1785;
                    Height = 1060;
                    ScaleRate = 1.5;
                    break;
                case EResolution.R1260:
                    Width = 2083;
                    Height = 1240;
                    ScaleRate = 1.75;
                    break;
                default:
                    Width = 1190;
                    Height = 720;
                    ScaleRate = 1.0;
                    break;
            }

        }
    }

    public void ChangeMainView(UserControl newView)
    {
        CurrentView = newView;
    }
    
    public void ChangePopupView(UserControl newView)
    {
        CurrentPopup = newView;
    }

    public async Task Startup()
    {
        // Allows only 1 instance of Assist to run at the same time.
        AssistApplication.CreateInstance();
        
        Log.Information("Main Window Loaded.");
        Log.Information("Switching to StartupPage");
        AssistApplication.ChangeMainWindowView(new Views.Startup.StartupView());
    }
}