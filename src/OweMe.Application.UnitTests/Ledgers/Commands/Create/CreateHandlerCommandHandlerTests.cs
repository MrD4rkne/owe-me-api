using Moq;
using OweMe.Application.Ledgers.Commands.Create;
using OweMe.Domain.Ledgers;
using Shouldly;

namespace OweMe.Application.UnitTests.Ledgers.Commands.Create;

public class CreateHandlerCommandHandlerTests : BaseCommandTest
{
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

        var expectedLedgerId = Guid.NewGuid();
        _ledgerContextMock.Setup(x => x.Ledgers.Add(It.IsAny<Ledger>()))
            .Callback<Ledger>(l => l.Id = expectedLedgerId);
        _ledgerContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var sut = new CreateHandlerCommandHandler(_ledgerContextMock.Object);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedLedgerId);
        _ledgerContextMock.Verify(x => x.Ledgers.Add(It.Is<Ledger>(ledger =>
            ledger.Name == ledgerName && ledger.Description == ledgerDescription
        )), Times.Once);
        _ledgerContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _ledgerContextMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenTaskCancelledDuringSave_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var command = new CreateLedgerCommand
        {
            Name = "Test Ledger",
            Description = "This is a test ledger."
        };

        using var cts = new CancellationTokenSource();

        _ledgerContextMock.Setup(x => x.Ledgers.Add(It.IsAny<Ledger>()))
            .Callback<Ledger>(l => l.Id = Guid.NewGuid());
        _ledgerContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(async ct =>
            {
                await cts.CancelAsync();
                ct.ThrowIfCancellationRequested();
                return 1;
            });

        var sut = new CreateHandlerCommandHandler(_ledgerContextMock.Object);

        // Act & Assert
        await Should.ThrowAsync<TaskCanceledException>(() => sut.Handle(command, cts.Token));

        _ledgerContextMock.Verify(x => x.SaveChangesAsync(cts.Token), Times.Once);
    }
}