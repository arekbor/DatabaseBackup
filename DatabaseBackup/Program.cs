namespace DatabaseBackup;
using JsonReader;

public static class Program
{
    public static void Main()
    {
        DbInit().GetAwaiter().GetResult();
    }

    private static async Task DbInit()
    {
        await ReadJson.InitJson();
        await DbContext.CreateDbContext(DbItems.DataSource, DbItems.UserId, DbItems.Password, DbItems.PathToBak, DbItems.PathToLog);
    }
}