using OweMe.Application.Ledgers.Commands.Create;
using OweMe.Tests.Common;
using Shouldly;

namespace OweMe.Application.UnitTests.Ledgers.Commands.Create;

public class CreateLedgerCommandHandlerTests : BaseCommandTest
{
    private readonly DateTimeOffset _currentTime = DateTimeOffset.UtcNow;
    private readonly Guid _currentUserId = Guid.NewGuid();

    public CreateLedgerCommandHandlerTests()
    {
        _userContextMock.Setup(x => x.Id).Returns(_currentUserId);
        _timeProvider.Setup(x => x.GetUtcNow()).Returns(_currentTime);
    }

    [Fact]
    public async Task Handle_ShouldCreateLedger_WhenValidCommand()
    {
        // Arrange
        const string ledgerName = "Test Ledger";
        const string ledgerDescription = "This is a test ledger.";
        var command = new CreateLedgerCommand
        {
            Name = ledgerName,
            Description = ledgerDescription
        };

        // Act
        var result = await CreateLedgerCommandHandler.Handle(command, _ledgerContextMock.Object,
            TestContext.Current.CancellationToken);

        // Assert
        var addedLedger = _ledgerContextMock.Object.Ledgers
            .FirstOrDefault(x => x.Name == ledgerName && x.Description == ledgerDescription);
        addedLedger.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(addedLedger.Id);

        addedLedger.Name.ShouldBe(command.Name);
        addedLedger.Description.ShouldBe(command.Description);

        addedLedger.ShouldBeCreated(
            _currentUserId,
            _currentTime
        );
        addedLedger.ShouldBeNeverUpdated();
    }
}