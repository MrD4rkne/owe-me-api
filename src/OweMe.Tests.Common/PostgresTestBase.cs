using Testcontainers.PostgreSql;

namespace OweMe.Tests.Common;

public abstract class PostgresTestBase : IAsyncDisposable
{
    private readonly PostgreSqlContainer _postgresContainer;
    
    protected PostgresTestBase()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("oweme_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5452)
            .Build();
        _postgresContainer.StartAsync().GetAwaiter().GetResult();
    }

    protected string ConnectionString => _postgresContainer.GetConnectionString();

    public async ValueTask DisposeAsync()
    {
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}