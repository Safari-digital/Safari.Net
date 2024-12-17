using InternalTestProgram.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalTestProgram;

public class TestContext(DbContextOptions<TestContext> options) : DbContext(options)
{
    public DbSet<TestUser> Users { get; set; }
    public DbSet<FakeUser> FakeUsers { get; set; }
    public DbSet<TestRole> Roles { get; set; }
    public DbSet<TestIdEntity> IdUsers { get; set; }
    public DbSet<TestGuidEntity> GuidUsers { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<ApiToken> ApiTokens { get; set; }
    public DbSet<AuthEvent> AuthEvents { get; set; }
}