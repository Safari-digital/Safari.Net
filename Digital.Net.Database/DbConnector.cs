using Digital.Net.Core.Environment;
using Digital.Net.Database.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Digital.Net.Database;

public static class DbConnector
{
    public static WebApplicationBuilder AddDbConnector<TContext>(
        this WebApplicationBuilder builder,
        Func<DigitalDatabaseOptions, DigitalDatabaseOptions> buildOptions
    )
        where TContext : DbContext
    {
        var options = buildOptions(new DigitalDatabaseOptions());
        builder.Services.AddDbContext<TContext>(opts =>
        {
            if (options.LazyLoadingProxies)
                opts.UseLazyLoadingProxies();
            if (AspNetEnv.IsTest || options.DatabaseEngine is DatabaseEngine.SqLiteInMemory)
                opts.UseSqlite(new SqliteConnection("Filename=:memory:"));
            else if (options.DatabaseEngine is DatabaseEngine.PostgreSql)
                opts.UseNpgsql(options.ConnectionString, b => b.MigrationsAssembly(options.MigrationAssembly));
            else
                throw new NotImplementedException("Database engine is not supported");
        }, ServiceLifetime.Transient);
        return builder;
    }
}