using Assist.Objects;
using Assist.Services;

using AutoUpdaterDotNET;

using Serilog;

using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Assist.Update;

public class ApplicationUpdateChecker
{

    private const string UpdateUrl = $"{AssistApiService.BaseUrl}/data/update";

    private readonly CancellationTokenSource _cts;

    private UpdateInfoEventArgs _eventArgs;
    private bool _isUpdated = true;

    public ApplicationUpdateChecker(TimeSpan timeout)
    {
        _cts = new CancellationTokenSource(timeout);
    }

    public async Task<UpdateCheckResult> ShouldUpdateAsync()
    {
        AutoUpdater.CheckForUpdateEvent += CheckForUpdate;
        AutoUpdater.ParseUpdateInfoEvent += ParseUpdateData;
        AutoUpdater.Start(UpdateUrl);

        await WaitForCheckAsync();

        return new UpdateCheckResult
        {
            IsUpdated = _isUpdated,
            EventArgs = _eventArgs
        };
    }

    private async Task WaitForCheckAsync()
    {
        try
        {
            await Task.Delay(Timeout.Infinite, _cts.Token);
        }
        catch (Exception e)
        {
            if (e is OperationCanceledException)
                return;

            Log.Error(e, "Exception while checking for application updated.");
        }
    }

    private void CheckForUpdate(UpdateInfoEventArgs args)
    {
        _eventArgs = args;

        try
        {
            var exception = args.Error;
            if (exception is WebException) // todo: can other exceptions occur?
                throw exception;

            var installedVersion = args.InstalledVersion;
            var currentVersion = new Version(args.CurrentVersion);
            Log.Information("Current version: {CurrentVersion}", currentVersion.ToString());
            Log.Information("Installed: {InstalledVersion}", installedVersion.ToString());

            var hasLatestVersion = installedVersion == currentVersion;
            var hasHigherVersion = installedVersion > currentVersion;
            if (hasLatestVersion || hasHigherVersion)
                return;

            _isUpdated = false;
        }
        catch (Exception e)
        {
            Log.Error(e, "Exception while checking for updates. {ExceptionMessage}", e.Message);
        }
        finally
        {
            _cts.Cancel();
        }
    }

    private static void ParseUpdateData(ParseUpdateInfoEventArgs args)
    {
        var content = args.RemoteData;
        var data = JsonSerializer.Deserialize<ApplicationUpdateData>(content, App.JsonSerializerOptions);

        if (data == null)
        {
            Log.Error("Failed to deserialize the update data while checking for application updates. Content: {Content}",
                content);
            return;
        }

        args.UpdateInfo = new UpdateInfoEventArgs
        {
            CurrentVersion = data.UpdateVersion,
            ChangelogURL = data.UpdateChangelog,
            DownloadURL = data.UpdateUrl,
            Mandatory =
            {
                Value = data.Mandatory.Value,
                MinimumVersion = data.Mandatory.MinVersion,
                UpdateMode = Mode.Forced
            }
        };
    }

    ~ApplicationUpdateChecker()
    {
        if (!_cts.IsCancellationRequested)
            _cts.Cancel();

        _cts.Dispose();
    }

}