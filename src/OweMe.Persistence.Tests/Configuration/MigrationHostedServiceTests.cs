using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OweMe.Application;
using OweMe.Persistence.Configuration;
using OweMe.Persistence.Ledgers;
using OweMe.Tests.Common;
using Shouldly;

namespace OweMe.Persistence.Tests.Configuration;

public class MigrationHostedServiceTests
{
    private readonly Mock<ILogger<LedgerDbContext>> _dbContextLoggerMock = new();
    private readonly Mock<ILogger<MigrationHostedService>> _loggerMock = new();
    private readonly Mock<IOptions<DatabaseOptions>> _optionsMock = new();
    private readonly IServiceCollection _serviceProvider;
    private readonly Mock<LedgerDbContext> _testContextMock = new();
    private readonly Mock<TimeProvider> _timeProviderMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();

    public MigrationHostedServiceTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddSingleton<IHostedService, MigrationHostedService>()
            .AddSingleton(_loggerMock.Object)
            .AddTransient<TimeProvider>(_ => _timeProviderMock.Object)
            .AddTransient<IUserContext>(_ => _userContextMock.Object)
            .AddTransient<ILogger<LedgerDbContext>>(_ => _dbContextLoggerMock.Object)
            .AddSingleton(_optionsMock.Object);
    }

    [Fact]
    public async Task StartAsync_ShouldRunMigrations_WhenEnabled()
    {
        // Arrange
        await using var postgresTestBase = new PostgresTestBase();
        await postgresTestBase.SetupAsync();

        _optionsMock.Setup(o => o.Value).Returns(new DatabaseOptions { RunMigrations = true });

        var connectionString = postgresTestBase.ConnectionString;
        _serviceProvider.AddDbContext<LedgerDbContext>(options => options.UseNpgsql(connectionString)
        );

        var serviceProvider = _serviceProvider.BuildServiceProvider();
        var migrationService = new MigrationHostedService(serviceProvider, _loggerMock.Object);

        (await AreTherePendingMigrations(serviceProvider))
            .ShouldBeTrue("There should be pending migrations before starting the service.");

        // Act
        await migrationService.StartAsync(CancellationToken.None);

        // Assert
        _optionsMock.Verify(options => options.Value, Times.Once,
            "The service should check the RunMigrations option to determine if migrations should run."
        );

        (await AreTherePendingMigrations(serviceProvider))
            .ShouldBeFalse("There should be no pending migrations after starting the service.");
    }

    [Fact]
    public async Task StartAsync_ShouldNotRunMigrations_WhenDisabled()
    {
        // Arrange
        _serviceProvider.AddTransient<LedgerDbContext>(_ => _testContextMock.Object);

        _optionsMock.Setup(o => o.Value).Returns(new DatabaseOptions { RunMigrations = false });
        var migrationService = new MigrationHostedService(_serviceProvider.BuildServiceProvider(), _loggerMock.Object);

        // Act
        await migrationService.StartAsync(CancellationToken.None);

        // Assert
        _testContextMock.VerifyNoOtherCalls();
        _optionsMock.Verify(options => options.Value, Times.Once,
            "The service should check the RunMigrations option to determine if migrations should run."
        );
    }

    private static async Task<bool> AreTherePendingMigrations(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<LedgerDbContext>();
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        return pendingMigrations.Any();
    }
}