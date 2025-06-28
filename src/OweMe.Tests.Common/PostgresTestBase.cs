using Testcontainers.PostgreSql;

namespace OweMe.Tests.Common;

public abstract class PostgresTestBase(
    string databaseName = "oweme_test",
    string username = "postgres",
    string password = "postgres",
    int? port = null) : IAsyncDisposable
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithDatabase(databaseName)
        .WithUsername(username)
        .WithPassword(password)
        .WithPortBinding(port ?? 5432, true)
        .Build();

    protected string ConnectionString => _postgresContainer.GetConnectionString();

    public virtual async ValueTask DisposeAsync()
    {
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public virtual Task SetupAsync()
    {
        return _postgresContainer.StartAsync();
    }
}