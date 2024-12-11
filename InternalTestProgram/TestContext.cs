using InternalTestProgram.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalTestProgram;

public class TestContext(DbContextOptions<TestContext> options) : DbContext(options)
{
    public DbSet<FakeUser> Users { get; set; }
    public DbSet<FakeRole> Roles { get; set; }
    public DbSet<TestIdEntity> IdUsers { get; set; }
    public DbSet<TestGuidEntity> GuidUsers { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
}