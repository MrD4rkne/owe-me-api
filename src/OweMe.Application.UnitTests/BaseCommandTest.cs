using Moq;
using OweMe.Application.UnitTests.Ledgers;
using OweMe.Domain.Ledgers;

namespace OweMe.Application.UnitTests;

public abstract class BaseCommandTest : IAsyncLifetime
{
    private readonly LedgerDbContextMoq _ledgerDbContextMoq;
    
    protected readonly Mock<IUserContext> _userContextMock = new();
    protected readonly Mock<TimeProvider> _timeProvider = new();
    protected Mock<ILedgerContext> _ledgerContextMock => _ledgerDbContextMoq.LedgerContextMock;

    protected BaseCommandTest()
    {
        _ledgerDbContextMoq = LedgerDbContextMoq.LedgerDbContextCreationOptions.New()
                .WithUserContext(_userContextMock.Object)
                .WithTimeProvider(_timeProvider.Object)
                .Build();
    }
    
    public Task InitializeAsync()
    {
        return _ledgerDbContextMoq.SetupAsync();
    }

    public Task DisposeAsync()
    {
        return _ledgerDbContextMoq.DisposeAsync().AsTask();
    }
}