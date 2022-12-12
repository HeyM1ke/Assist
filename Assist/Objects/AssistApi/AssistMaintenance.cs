namespace Assist.Objects.AssistApi
{
    public class AssistMaintenance
    {
        public string Id { get; set; }
        public bool DownForMaintenance { get; set; }
        public string DownForMaintenanceMessage { get; set; }
    }
}