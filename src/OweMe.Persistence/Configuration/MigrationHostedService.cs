using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OweMe.Persistence.Ledgers;

namespace OweMe.Persistence.Configuration;

public class MigrationHostedService(
    IServiceProvider serviceProvider,
    ILogger<MigrationHostedService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbOptions = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        if (!dbOptions.RunMigrations)
        {
            logger.LogInformation("Database migrations are disabled");
            return;
        }

        logger.LogInformation("Running database migrations");
        var context = scope.ServiceProvider.GetRequiredService<LedgerDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("Database migrations completed successfully");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
