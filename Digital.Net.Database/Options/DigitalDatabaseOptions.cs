namespace Digital.Net.Database.Options;

public class DigitalDatabaseOptions
{
    public string ConnectionString { get; private set; } = string.Empty;
    public string MigrationAssembly { get; private set; } = string.Empty;
    public DatabaseEngine DatabaseEngine { get; private set; } = DatabaseEngine.PostgreSql;

    public DigitalDatabaseOptions SetConnectionString(string connectionString)
    {
        ConnectionString = connectionString;
        return this;
    }

    public DigitalDatabaseOptions SetMigrationAssembly(string migrationAssembly)
    {
        MigrationAssembly = migrationAssembly;
        return this;
    }

    public DigitalDatabaseOptions SetDatabaseEngine(DatabaseEngine databaseEngine)
    {
        DatabaseEngine = databaseEngine;
        return this;
    }
}