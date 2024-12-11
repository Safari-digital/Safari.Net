using Digital.Net.Database.Options;
using Digital.Net.Database.Services;
using Digital.Net.TestTools;
using Microsoft.Extensions.Options;
using Moq;

namespace Digital.Net.Database.Test.Services;

public class DataAccessorTest : UnitTest
{
    private readonly Mock<IOptions<DigitalDatabaseOptions>> _optionsMock = new();
    private readonly DigitalDatabaseOptions _options = new();
    private readonly DataAccessor _dataAccessor;

    public DataAccessorTest()
    {
        _optionsMock.SetupGet(x => x.Value).Returns(_options);
        _dataAccessor = new DataAccessor(_optionsMock.Object);
    }

    [Fact]
    public void ExecuteSql_WhenDatabaseEngineIsPostgreSql_ShouldExecutePsqlFunction()
    {
        _options.SetConnectionString("Host=localhost;Port=5432;Database=None;Username=postgres;Password=postgres");
        _options.SetDatabaseEngine(DatabaseEngine.PostgreSql);
        var result = _dataAccessor.ExecuteSql("");
        Assert.True(result.Errors[0].Reference is "NPGSQL_POSTGRESEXCEPTION" or "NPGSQL_NPGSQLEXCEPTION");
    }

    [Fact]
    public void ExecuteSql_ShouldExecuteProvidedSql()
    {
        _options.SetConnectionString("Data Source=:memory:");
        _options.SetDatabaseEngine(DatabaseEngine.SqLiteInMemory);
        var result = _dataAccessor.ExecuteSql(
            "CREATE TABLE test (id INT PRIMARY KEY); INSERT INTO test (id) VALUES (1);"
        );
        Assert.Equal("Affected rows: 1", result.Infos[0].Message);
    }
}