using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using DatabaseBackup.Data;

namespace DatabaseBackup;

public class DbContext
{
    private static List<string> _listDatabases = null!;
    public static async Task CreateDbContext(string? dataSource, string? userId, string? password)
    {
        var connectionString = new SqlConnectionStringBuilder();
        try
        {
            connectionString.DataSource = dataSource;
            connectionString.UserID = userId;
            connectionString.Password = password;
            await using var conn = new SqlConnection(connectionString.ConnectionString);
            await conn.OpenAsync();
            Console.WriteLine($"{Message.Prefix}" +
                $"{MethodBase.GetCurrentMethod()?.DeclaringType} connected successfully");

            const string cmdText = "SELECT name FROM sys.databases;";
            
            await using var sqlCmdDatabases = new SqlCommand(cmdText, conn);
            await using var sqlReader = await sqlCmdDatabases.ExecuteReaderAsync();
            _listDatabases = new List<string>();
            while (await sqlReader.ReadAsync())
            {
                if(!sqlReader[0].ToString()!.Equals("tempdb"))
                    _listDatabases.Add(sqlReader[0].ToString() ?? string.Empty);
            }

            await conn.CloseAsync();

            Console.WriteLine($"{Message.Prefix}" +
                $"total databases count: {_listDatabases.Count}");

            var stringBuilder = new StringBuilder();
            await conn.OpenAsync();
            
            for (int i = 0; i < _listDatabases.Count; i++)
            {
                Console.WriteLine($"{Message.Prefix}" +
                    $"starting backup database {_listDatabases[i]}");
                
                stringBuilder.Append($"BACKUP DATABASE {_listDatabases[i]} ");
                stringBuilder.Append(@$"TO DISK = 'F:\dbbackup\{_listDatabases[i]}.BAK'; ");
                var sqlCmdBackup = new SqlCommand(stringBuilder.ToString(), conn);
                await sqlCmdBackup.ExecuteReaderAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{Message.Prefix}" +
                $"{MethodBase.GetCurrentMethod()?.DeclaringType} cannot connect to database server");
            Console.WriteLine(e.Message);
            throw;
        }
    }
}