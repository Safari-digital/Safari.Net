using InternalTestUtilities.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalTestUtilities;

public class TestContext(DbContextOptions<TestContext> options) : DbContext(options)
{
    public DbSet<TestIdEntity> IdUsers { get; set; }
    public DbSet<TestGuidEntity> GuidUsers { get; set; }
}