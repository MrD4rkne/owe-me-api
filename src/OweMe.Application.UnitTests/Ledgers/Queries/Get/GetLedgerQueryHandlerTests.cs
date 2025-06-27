using Moq;
using OweMe.Application.Ledgers.Commands.Create;
using OweMe.Domain.Ledgers;
using OweMe.Domain.Ledgers.Queries.Get;
using OweMe.Domain.Users;
using Shouldly;

namespace OweMe.Application.UnitTests.Ledgers.Queries.Get;

public class GetLedgerQueryHandlerTests : IAsyncDisposable
{
    private readonly LedgerDbContextMoq _ledgerDbContextMoq;
    private readonly Mock<IUserContext> _userContextMock = new();

    private GetLedgerQueryHandler _handler;

    public GetLedgerQueryHandlerTests()
    {
        _ledgerDbContextMoq = LedgerDbContextMoq.Create(new LedgerDbContextMoq.LedgerDbContextCreationOptions()
            .WithUserContext(_userContextMock.Object));
    }

    private Mock<ILedgerContext> _ledgerContextMock => _ledgerDbContextMoq.LedgerContextMock;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _ledgerDbContextMoq.SetupAsync();
    }

    [SetUp]
    public async Task Setup()
    {
        _handler = new GetLedgerQueryHandler(_ledgerContextMock.Object, _userContextMock.Object);

        _ledgerContextMock.Invocations.Clear();
        _userContextMock.Invocations.Clear();
    }
    
    public async ValueTask DisposeAsync()
    {
        await _ledgerDbContextMoq.DisposeAsync().AsTask();
        GC.SuppressFinalize(this);
    }

    [Test]
    public async Task Handle_ShouldReturnLedger_WhenLedgerExistsAndUserHasAccess()
    {
        // Arrange
        var userId = UserId.New();
        _userContextMock.Setup(x => x.Id).Returns(userId);

        var ledger = new Ledger { Name = "Test Ledger", CreatedAt = DateTimeOffset.UtcNow, CreatedBy = userId };
        await _ledgerDbContextMoq.GetLedgerContext().Ledgers.AddAsync(ledger);
        await _ledgerDbContextMoq.GetLedgerContext().SaveChangesAsync();
        var ledgerId = ledger.Id;

        _ledgerContextMock.Invocations.Clear();
        _userContextMock.Invocations.Clear();

        var query = new GetLedgerQuery(ledgerId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(ledgerId);
        result.Value.Name.ShouldBe(ledger.Name);
        result.Value.CreatedAt.ShouldBe(ledger.CreatedAt);
        result.Value.CreatedBy.ShouldBe<Guid>(ledger.CreatedBy);
    }

    [Test]
    public async Task Handle_ShouldReturnFailure_WhenLedgerDoesNotExist()
    {
        // Arrange
        var ledgerId = Guid.NewGuid();

        var query = new GetLedgerQuery(ledgerId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(LedgerErrors.LedgerNotFound);
    }

    [Test]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotHaveAccessToLedger()
    {
        // Arrange
        var otherUserId = UserId.New();
        _userContextMock.Setup(x => x.Id).Returns(otherUserId);

        // Let's create a ledger with a different user
        var ledger = new Ledger { Name = "Test Ledger", CreatedAt = DateTimeOffset.UtcNow, CreatedBy = otherUserId };
        await _ledgerDbContextMoq.GetLedgerContext().Ledgers.AddAsync(ledger);
        await _ledgerDbContextMoq.GetLedgerContext().SaveChangesAsync();
        var ledgerId = ledger.Id;

        var userId = UserId.New();
        userId.ShouldNotBe(otherUserId);
        _userContextMock.Setup(x => x.Id).Returns(userId);

        _userContextMock.Invocations.Clear();
        _ledgerContextMock.Invocations.Clear();

        var query = new GetLedgerQuery(ledgerId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(LedgerErrors.LedgerNotFound);

        _userContextMock.Verify(x => x.Id, Times.Once);
    }
}