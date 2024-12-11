using Digital.Net.Authentication;
using Digital.Net.Database;
using Digital.Net.Database.Options;
using Digital.Net.Entities;
using InternalTestProgram.Models;

namespace InternalTestProgram;

public sealed class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddDbConnector<TestContext>(options => options.SetDatabaseEngine(DatabaseEngine.SqLiteInMemory));
        builder.Services.AddControllers();
        builder.Services.AddDigitalEntities<TestContext>();
        builder.Services.AddDigitalApiKeyAuthorization<ApiKey>();

        var app = builder.Build();
        app.MapControllers();

        await app.RunAsync();
    }
}