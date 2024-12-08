using Digital.Net.Core.Environment;
using Digital.Net.Database.Options;
using Digital.Net.Database.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Digital.Net.Database;

public static class DbConnector
{
    public static WebApplicationBuilder AddDbConnector<TContext>(
        this WebApplicationBuilder builder,
        Action<DigitalDatabaseOptions> buildOptions
    )
        where TContext : DbContext
    {
        var options = new DigitalDatabaseOptions();
        buildOptions(options);

        builder.Services.AddEntityFrameworkProxies();
        builder.Services.AddDbContext<TContext>(opts =>
        {
            if (AspNetEnv.IsTest || options.DatabaseEngine is DatabaseEngine.SqLiteInMemory)
                opts.UseSqlite(DatabaseUtils.InMemorySqliteConnection);
            else if (options.DatabaseEngine is DatabaseEngine.PostgreSql)
                opts.UseNpgsql(options.ConnectionString, b => b.MigrationsAssembly(options.MigrationAssembly));
            else
                throw new NotImplementedException("Database engine is not supported");

            opts.UseLazyLoadingProxies();
        }, ServiceLifetime.Transient);
        return builder;
    }
}