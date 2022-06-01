using AutoUpdaterDotNET;

namespace Assist.Update;

public class UpdateCheckResult
{

    public bool IsUpdated { get; set; }
    public UpdateInfoEventArgs EventArgs { get; set; }

}