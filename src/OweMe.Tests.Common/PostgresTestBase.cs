using Testcontainers.PostgreSql;

namespace OweMe.Tests.Common;

public abstract class PostgresTestBase : IAsyncDisposable
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithDatabase("oweme_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithPortBinding(5452)
        .Build();

    protected Task SetupAsync()
    {
        return _postgresContainer.StartAsync();
    }

    protected string ConnectionString => _postgresContainer.GetConnectionString();

    public async ValueTask DisposeAsync()
    {
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}