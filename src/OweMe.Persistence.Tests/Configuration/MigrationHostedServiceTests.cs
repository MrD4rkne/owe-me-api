using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OweMe.Persistence.Configuration;
using OweMe.Persistence.Ledgers;
using OweMe.Tests.Common;
using Shouldly;

namespace OweMe.Persistence.Tests.Configuration;

public class MigrationHostedServiceTests : PostgresTestBase, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await SetupAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return base.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task StartAsync_ShouldRunMigrations_WhenEnabled()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<DatabaseOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new DatabaseOptions { RunMigrations = false });

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IHostedService, MigrationHostedService>()
            .AddSingleton(optionsMock.Object)
            .AddDbContext<LedgerDbContext>(options => options.UseNpgsql(ConnectionString))
            .BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<MigrationHostedService>>();
        var migrationService = new MigrationHostedService(serviceProvider, logger);

        // Verify that there are pending migrations before running the service
        (await AreTherePendingMigrations(serviceProvider))
            .ShouldBeTrue("There should be pending migrations before running the migration service.");

        // Act
        await migrationService.StartAsync(CancellationToken.None);

        // Assert
        (await AreTherePendingMigrations(serviceProvider))
            .ShouldBeFalse("There should be no pending migrations after running the migration service.");
        optionsMock.Verify(options => options.Value, Times.Once,
            "The service should check the RunMigrations option to determine if migrations should run."
        );
    }

    [Fact]
    public async Task StartAsync_ShouldNotRunMigrations_WhenDisabled()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<DatabaseOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new DatabaseOptions { RunMigrations = false });

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IHostedService, MigrationHostedService>()
            .AddSingleton(optionsMock.Object)
            .AddDbContext<LedgerDbContext>(options => options.UseNpgsql(ConnectionString))
            .BuildServiceProvider();

        var loggerMock = new Mock<ILogger<MigrationHostedService>>();
        var migrationService = new MigrationHostedService(serviceProvider, loggerMock.Object);

        // Act
        await migrationService.StartAsync(CancellationToken.None);

        // Assert
        optionsMock.Verify(options => options.Value, Times.Once,
            "The service should check the RunMigrations option to determine if migrations should run."
        );
    }

    private static async Task<bool> AreTherePendingMigrations(IServiceProvider serviceProvider)
    {
        await using var context = serviceProvider.GetRequiredService<LedgerDbContext>();
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        return pendingMigrations.Any();
    }
}