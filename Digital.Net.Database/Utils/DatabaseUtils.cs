using Microsoft.Data.Sqlite;

namespace Digital.Net.Database.Utils;

public class DatabaseUtils
{
    public const string InMemoryConnectionString = "Filename=:memory:";

    public static SqliteConnection InMemorySqliteConnection => new SqliteConnection(InMemoryConnectionString);
}