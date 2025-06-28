using Moq;
using OweMe.Application.UnitTests.Ledgers;
using OweMe.Domain.Ledgers;

namespace OweMe.Application.UnitTests;

public abstract class BaseCommandTest : IAsyncLifetime
{
    private readonly LedgerDbContextMoq _ledgerDbContextMoq;
    protected readonly Mock<TimeProvider> _timeProvider = new();

    protected readonly Mock<IUserContext> _userContextMock = new();

    protected BaseCommandTest()
    {
        _ledgerDbContextMoq = LedgerDbContextMoq.LedgerDbContextCreationOptions.New()
            .WithUserContext(_userContextMock.Object)
            .WithTimeProvider(_timeProvider.Object)
            .Build();
    }

    protected Mock<ILedgerContext> _ledgerContextMock => _ledgerDbContextMoq.LedgerContextMock;

    public virtual Task InitializeAsync()
    {
        return _ledgerDbContextMoq.SetupAsync();
    }

    public virtual Task DisposeAsync()
    {
        return _ledgerDbContextMoq.DisposeAsync().AsTask();
    }
}