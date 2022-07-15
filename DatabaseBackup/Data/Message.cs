using System.Reflection;

namespace DatabaseBackup.Data;

public class Message
{
    public static readonly string Prefix = $"[{Assembly.GetEntryAssembly()?.GetName().Name}] ";
}