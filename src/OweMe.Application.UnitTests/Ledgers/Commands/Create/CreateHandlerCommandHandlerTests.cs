using OweMe.Application.Ledgers.Commands.Create;
using OweMe.Application.UnitTests.Common;
using Shouldly;

namespace OweMe.Application.UnitTests.Ledgers.Commands.Create;

public class CreateHandlerCommandHandlerTests : BaseCommandTest
{
    private readonly DateTimeOffset _currentTime = DateTimeOffset.UtcNow;
    private readonly Guid _currentUserId = Guid.NewGuid();
    private CreateHandlerCommandHandler _sut = null!;

    public CreateHandlerCommandHandlerTests()
    {
        _userContextMock.Setup(x => x.Id).Returns(_currentUserId);
        _timeProvider.Setup(x => x.GetUtcNow()).Returns(_currentTime);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new CreateHandlerCommandHandler(_ledgerContextMock.Object);
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
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        var addedLedger = _ledgerContextMock.Object.Ledgers
            .FirstOrDefault(x => x.Name == ledgerName && x.Description == ledgerDescription);
        addedLedger.ShouldNotBeNull();
        result.ShouldBe(addedLedger.Id);

        addedLedger.Name.ShouldBe(command.Name);
        addedLedger.Description.ShouldBe(command.Description);

        addedLedger.ShouldBeCreated(
            _currentUserId,
            _currentTime
        );
        addedLedger.ShouldBeNeverUpdated();
    }
}