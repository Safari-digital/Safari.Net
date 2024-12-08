namespace Digital.Net.Database.Options;

public class DigitalDatabaseOptions
{
    public string ConnectionString { get; } = string.Empty;
    public string MigrationAssembly { get; } = string.Empty;
    public bool LazyLoadingProxies { get; } = true;
    public DatabaseEngine DatabaseEngine { get; set; } = DatabaseEngine.PostgreSql;
}