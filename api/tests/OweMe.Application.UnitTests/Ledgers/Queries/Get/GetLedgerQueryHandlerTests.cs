using Moq;
using OweMe.Application.Ledgers.Queries.Get;
using OweMe.Domain.Common.Exceptions;
using OweMe.Domain.Ledgers;
using OweMe.Domain.Users;
using Shouldly;

namespace OweMe.Application.UnitTests.Ledgers.Queries.Get;

public class GetLedgerQueryHandlerTests : BaseCommandTest
{
    [Fact]
    public async Task Handle_ShouldReturnLedger_WhenLedgerExistsAndUserHasAccess()
    {
        // Arrange
        var userId = UserId.New();
        _userContextMock.Setup(x => x.Id).Returns(userId);

        var ledger = new Ledger { Name = "Test Ledger", CreatedBy = userId };
        await _ledgerContextMock.Object.Ledgers.AddAsync(ledger, TestContext.Current.CancellationToken);
        await _ledgerContextMock.Object.SaveChangesAsync(TestContext.Current.CancellationToken);
        var ledgerId = ledger.Id;

        _ledgerContextMock.Invocations.Clear();
        _userContextMock.Invocations.Clear();

        var query = new GetLedgerQuery(ledgerId);

        // Act
        var result = await GetLedgerQueryHandler.HandleAsync(query,
            _ledgerContextMock.Object, _userContextMock.Object,
            TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test Ledger");
        result.Id.ShouldBe(ledgerId);
        result.CreatedBy.ShouldBe<Guid>(userId);
        result.CreatedAt.ShouldBe(ledger.CreatedAt);
        result.UpdatedBy.ShouldBeNull();
        result.UpdatedAt.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrow_NotFound_WhenLedgerDoesNotExist()
    {
        // Arrange
        var ledgerId = Guid.NewGuid();

        var query = new GetLedgerQuery(ledgerId);

        // Act
        await Assert.ThrowsAsync<NotFoundException>(() => GetLedgerQueryHandler.HandleAsync(query,
            _ledgerContextMock.Object, _userContextMock.Object,
            TestContext.Current.CancellationToken));

        _userContextMock.Verify(x => x.Id, Times.AtMostOnce);
    }

    [Fact]
    public async Task HandleShouldThrow_NotFound_WhenUserDoesNotHaveAccessToLedger()
    {
        // Arrange
        var otherUserId = UserId.New();
        _userContextMock.Setup(x => x.Id).Returns(otherUserId);

        // Let's create a ledger with a different user
        var ledger = new Ledger { Name = "Test Ledger", CreatedAt = DateTimeOffset.UtcNow, CreatedBy = otherUserId };
        await _ledgerContextMock.Object.Ledgers.AddAsync(ledger, TestContext.Current.CancellationToken);
        await _ledgerContextMock.Object.SaveChangesAsync(TestContext.Current.CancellationToken);
        var ledgerId = ledger.Id;

        var userId = UserId.New();
        userId.ShouldNotBe(otherUserId);
        _userContextMock.Setup(x => x.Id).Returns(userId);

        _userContextMock.Invocations.Clear();
        _ledgerContextMock.Invocations.Clear();

        var query = new GetLedgerQuery(ledgerId);

        // Act
        await Assert.ThrowsAsync<NotFoundException>(() => GetLedgerQueryHandler.HandleAsync(query,
            _ledgerContextMock.Object, _userContextMock.Object,
            TestContext.Current.CancellationToken));

        _userContextMock.Verify(x => x.Id, Times.Once);
    }
}