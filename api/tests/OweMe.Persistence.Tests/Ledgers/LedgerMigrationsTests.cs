using Microsoft.EntityFrameworkCore;
using Moq;
using OweMe.Application;
using OweMe.Domain.Ledgers;
using OweMe.Persistence.Ledgers;
using OweMe.Tests.Common;
using Shouldly;

namespace OweMe.Persistence.Tests.Ledgers;

public class LedgerMigrationsTests() : PostgresTestBase("oweme_migrations_test"), IAsyncLifetime
{
    private readonly Mock<TimeProvider> _timeProvider = new();
    private readonly Mock<IUserContext> _userContextMock = new();

    public async Task InitializeAsync()
    {
        await SetupAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return base.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task Migrations_ShouldApplySuccessfully()
    {
        // Arrange
        await using var context = new LedgerDbContext(
            new DbContextOptionsBuilder<LedgerDbContext>()
                .UseNpgsql(ConnectionString)
                .Options,
            _timeProvider.Object,
            _userContextMock.Object
        );

        // Act
        await context.Database.MigrateAsync();

        // Assert
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        pendingMigrations.ShouldBeEmpty("There should be no pending migrations after applying them.");
    }

    [Fact]
    public async Task Migrations_ShouldBeAbleToCreateAndQuery()
    {
        // Arrange
        await using var context = new LedgerDbContext(
            new DbContextOptionsBuilder<LedgerDbContext>()
                .UseNpgsql(ConnectionString)
                .Options,
            _timeProvider.Object,
            _userContextMock.Object
        );

        await context.Database.MigrateAsync();

        var ledger = new Ledger
        {
            Name = "Test Ledger",
            Description = "This is a test ledger."
        };

        // Act
        context.Ledgers.Add(ledger);
        await context.SaveChangesAsync();

        var createdLedger = await context.Ledgers
            .FirstOrDefaultAsync(x => x.Name == "Test Ledger" && x.Description == "This is a test ledger.");

        // Assert
        createdLedger.ShouldNotBeNull("The ledger should have been created successfully.");
        createdLedger.Name.ShouldBe("Test Ledger");
        createdLedger.Description.ShouldBe("This is a test ledger.");
        createdLedger.ShouldBeCreated(
            _userContextMock.Object.Id,
            _timeProvider.Object.GetUtcNow()
        );
    }
}