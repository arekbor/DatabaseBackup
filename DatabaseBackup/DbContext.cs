using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using DatabaseBackup.Data;

namespace DatabaseBackup;

public class DbContext
{
    private static List<string> _listOfDatabases = null!;
    public static async Task CreateDbContext(string? dataSource, string? userId, string? password, string? pathToBak, string? pathToLog)
    {
        var connectionString = new SqlConnectionStringBuilder();
        try
        {
            connectionString.DataSource = dataSource;
            connectionString.UserID = userId;
            connectionString.Password = password;
            using (var conn = new SqlConnection(connectionString.ConnectionString)){
                await conn.OpenAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{Message.Prefix}" +
                    $"{MethodBase.GetCurrentMethod()?.DeclaringType} connected successfully");
                Console.ResetColor();

                const string cmdText = "SELECT name FROM sys.databases;";
                
                using (var sqlCmdDatabases = new SqlCommand(cmdText, conn)){
                    using (var sqlReader = await sqlCmdDatabases.ExecuteReaderAsync()){

                        _listOfDatabases = new List<string>();
                        var stringBuilder = new StringBuilder();
                        while (await sqlReader.ReadAsync())
                        {

                            if(sqlReader[0].ToString()!.Equals("tempdb"))
                                continue;

                            _listOfDatabases.Add(sqlReader[0].ToString() ?? string.Empty);

                            Console.WriteLine($"{Message.Prefix}" +
                                $"{sqlReader[0]} starting backup");

                            stringBuilder.Append($"BACKUP DATABASE {sqlReader[0]} ");
                            stringBuilder.Append(@$"TO DISK = '{pathToBak}{DateTime.Now.ToString("MMddyyyyHHmm")}/{sqlReader[0]}.BAK';");
                        }
                        await conn.CloseAsync();
                        await conn.OpenAsync();

                        var sqlCmdBackup = new SqlCommand(stringBuilder.ToString(), conn);
                        await sqlCmdBackup.ExecuteReaderAsync();
                        await conn.CloseAsync();

                        Console.WriteLine($"{Message.Prefix}" +
                                $"Backup successfully created {DateTime.Now.ToString("MM/dd/yyyy HH:mm")}");
                        }

                        if(!Directory.Exists($"{pathToLog}"))
                            Directory.CreateDirectory($"{pathToLog}");

                        string text = $"{Message.Prefix}" +
                            $"({DateTime.Now.ToString("MM/dd/yyyy HH:mm")}) " +
                            $"Backup successfully created" + System.Environment.NewLine +
                            $"created backups:" + System.Environment.NewLine +
                            $"{String.Join(", ", _listOfDatabases.ToArray())}";

                        await File.WriteAllTextAsync($"{pathToLog}{DateTime.Now.ToString("MMddyyyyHHmm")}.txt",text);
                    }
                }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Message.Prefix}" +
                $"{MethodBase.GetCurrentMethod()?.DeclaringType} cannot connect to database server");
            Console.ResetColor();
            Console.WriteLine(e.Message);
            throw;
        }
    }
}