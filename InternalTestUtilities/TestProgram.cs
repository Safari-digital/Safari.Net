using Digital.Net.Authentication;
using Digital.Net.Database;
using Digital.Net.Database.Options;
using InternalTestUtilities.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InternalTestUtilities;

public sealed class TestProgram
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.AddDbConnector<TestContext>(
            options => options.SetDatabaseEngine(DatabaseEngine.SqLiteInMemory)
        );
        builder.Services.AddDigitalApiKeyAuthorization<ApiKey>(
            options => options.SetHeaderAccessor("X-API-Key")
        );
        var app = builder.Build();
        app.MapControllers();
        await app.RunAsync();
    }
}