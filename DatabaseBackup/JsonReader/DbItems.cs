using Newtonsoft.Json;

namespace DatabaseBackup.JsonReader;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class DbItems
{
    [JsonProperty("DataSource")]
    public static string? DataSource { get; set; }
    [JsonProperty("UserId")]
    public static string? UserId { get; set; }
    [JsonProperty("Password")]
    public static string? Password { get; set; }
    [JsonProperty("PathToBak")]
    public static string? PathToBak { get; set; }
    [JsonProperty("PathToLog")]
    public static string? PathToLog { get; set; }
}