using Digital.Net.Database;
using Digital.Net.Database.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InternalTestUtilities;

public sealed class TestProgram
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.AddDbConnector<TestContext>(options => options.SetDatabaseEngine(DatabaseEngine.SqLiteInMemory));

        var app = builder.Build();
        app.MapControllers();
        await app.RunAsync();
    }
}