using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DatabaseBackup.JsonReader;

public static class ReadJson
{
    public static async Task InitJson()
    {
        using var reader = new StreamReader("dbSettings.json");
        var json = await reader.ReadToEndAsync();
        var items = JsonConvert.DeserializeObject<DbItems>(json);
    }
}