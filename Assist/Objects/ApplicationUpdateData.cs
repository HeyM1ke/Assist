namespace Assist.Objects;

public class ApplicationUpdateData
{
    public string UpdateUrl { get; set; }
    public string UpdateVersion { get; set; }
    public string UpdateChangelog { get; set; }
    public ApplicationUpdateDataMandatory Mandatory { get; set; }
}