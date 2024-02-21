using System;

namespace Assist.Game.Models.Live;

public class AssistPlayerStorage
{
    private const int EXPIRETIME_MINS = 10;
    private DateTime LastUpdated { get; set; }
    public string PlayerId { get; set; } = String.Empty;
    
    
    
    public bool IsOld()
    {
        return LastUpdated.ToUniversalTime().AddMinutes(EXPIRETIME_MINS) < DateTime.UtcNow;
    }
}