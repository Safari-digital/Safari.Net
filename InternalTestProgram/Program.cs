using Digital.Net.Authentication;
using Digital.Net.Authentication.Options.Jwt;
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
        builder.Services.AddDigitalJwtOptions(options =>
            options.SetJwtTokenOptions(new JwtTokenOptions
            {
                Issuer = "Issuer",
                Audience = "Audience",
                CookieName = "Cookie",
                Secret = "ThisIsA32ByteSecretKeyForTestingPurposes"
            })
        );
        builder.Services.AddDigitalJwtAuthentication<TestUser, ApiToken, AuthEvent>();
        builder.Services.AddDigitalApiKeyAuthentication<TestUser, ApiKey>();
        builder.Services.AddDigitalApiKeyAuthentication<FakeUser, ApiKey>();


        var app = builder.Build();
        app.MapControllers();

        await app.RunAsync();
    }
}