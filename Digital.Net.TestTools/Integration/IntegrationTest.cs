using Digital.Net.Core.Environment;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Digital.Net.TestTools.Integration;

public abstract class IntegrationTest<T, TContext>
    : UnitTest,
        IClassFixture<AppFactory<T, TContext>>
    where T : class
    where TContext : DbContext
{
    protected readonly List<HttpClient> Clients = [];
    protected readonly IConfiguration Configuration;
    protected readonly WebApplicationFactory<T> Factory;

    protected HttpClient BaseClient => Clients.First();

    protected IntegrationTest(AppFactory<T, TContext> fixture)
    {
        Factory = fixture;
        Clients.Add(Factory.CreateClient());
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile($"appsettings.{AspNetEnv.Test}.json", true)
            .AddEnvironmentVariables()
            .Build();
    }

    protected TContext GetContext() => Factory.Services.GetRequiredService<TContext>();

    protected void CreateClient(int? amount = 1)
    {
        for (var i = 0; i < amount; i++)
        {
            var client = Factory.CreateClient();
            Clients.Add(client);
        }
    }

    protected void DisposeClients()
    {
        foreach (var client in Clients)
            client.Dispose();
        Clients.Clear();
    }
}
