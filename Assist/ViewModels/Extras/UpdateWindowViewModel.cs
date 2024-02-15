using System;
using System.Threading.Tasks;
using Serilog;
using Velopack;

namespace Assist.ViewModels.Extras;

public partial class UpdateWindowViewModel : ViewModelBase
{
    public async Task Setup()
    {
        try
        {
            var mgr = new UpdateManager("https://cdn.assistval.com/releases/beta/win/");
            var newVer = await mgr.CheckForUpdatesAsync();
            if (newVer == null)
                return;
            await mgr.DownloadUpdatesAsync(newVer);

            // install new version and restart app
            mgr.ApplyUpdatesAndRestart(newVer);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }
}