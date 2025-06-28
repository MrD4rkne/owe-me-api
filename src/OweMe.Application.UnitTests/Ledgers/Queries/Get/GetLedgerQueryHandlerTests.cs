using Moq;
using OweMe.Application.Ledgers.Queries.Get;
using OweMe.Domain.Ledgers;
using OweMe.Domain.Ledgers.Queries.Get;
using OweMe.Domain.Users;
using Shouldly;

namespace OweMe.Application.UnitTests.Ledgers.Queries.Get;

public class GetLedgerQueryHandlerTests : BaseCommandTest
{
    private GetLedgerQueryHandler _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new GetLedgerQueryHandler(_ledgerContextMock.Object, _userContextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLedger_WhenLedgerExistsAndUserHasAccess()
    {
        // Arrange
        var userId = UserId.New();
        _userContextMock.Setup(x => x.Id).Returns(userId);

        var ledger = new Ledger { Name = "Test Ledger", CreatedBy = userId };
        await _ledgerContextMock.Object.Ledgers.AddAsync(ledger);
        await _ledgerContextMock.Object.SaveChangesAsync();
        var ledgerId = ledger.Id;

        _ledgerContextMock.Invocations.Clear();
        _userContextMock.Invocations.Clear();

        var query = new GetLedgerQuery(ledgerId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(ledgerId);
        result.Value.Name.ShouldBe(ledger.Name);
        result.Value.CreatedAt.ShouldBe(ledger.CreatedAt);
        result.Value.CreatedBy.ShouldBe<Guid>(ledger.CreatedBy);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenLedgerDoesNotExist()
    {
        // Arrange
        var ledgerId = Guid.NewGuid();

        var query = new GetLedgerQuery(ledgerId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(LedgerErrors.Errors.LedgerNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotHaveAccessToLedger()
    {
        // Arrange
        var otherUserId = UserId.New();
        _userContextMock.Setup(x => x.Id).Returns(otherUserId);

        // Let's create a ledger with a different user
        var ledger = new Ledger { Name = "Test Ledger", CreatedAt = DateTimeOffset.UtcNow, CreatedBy = otherUserId };
        await _ledgerContextMock.Object.Ledgers.AddAsync(ledger);
        await _ledgerContextMock.Object.SaveChangesAsync();
        var ledgerId = ledger.Id;

        var userId = UserId.New();
        userId.ShouldNotBe(otherUserId);
        _userContextMock.Setup(x => x.Id).Returns(userId);

        _userContextMock.Invocations.Clear();
        _ledgerContextMock.Invocations.Clear();

        var query = new GetLedgerQuery(ledgerId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(LedgerErrors.Errors.LedgerNotFound);

        _userContextMock.Verify(x => x.Id, Times.Once);
    }
}