using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using System;
using System.IO;
using System.Text.Json;
using Assist.Settings;
using Assist.ViewModels;
using Serilog;

namespace Assist
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                var settingsContent = File.ReadAllText(AssistSettings.SettingsFilePath);
                AssistSettings.Current = JsonSerializer.Deserialize<AssistSettings>(settingsContent);
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                Log.Fatal("Fatal Error");
                Log.Fatal(e.Message);
                Log.Fatal("Fatal Error STACK == ");
                Log.Fatal(e.StackTrace);
                Log.CloseAndFlush();
            }
            finally
            {
#if (!DEBUG)
                Log.CloseAndFlush();
                AssistSettings.Save();
                GameSettings.Save();
                AssistApplication.CurrentApplication.Shutdown();
#endif

            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect().With(new Win32PlatformOptions()
                {
                }).With(new MacOSPlatformOptions()
                {
                    DisableDefaultApplicationMenuItems = true,
                    DisableNativeMenus = true,
                    ShowInDock = true
                })
                .UseReactiveUI();

    }
}
