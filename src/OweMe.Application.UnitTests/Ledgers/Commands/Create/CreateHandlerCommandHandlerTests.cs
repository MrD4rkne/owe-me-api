using Moq;
using OweMe.Application.Ledgers.Commands.Create;
using OweMe.Domain.Ledgers;
using Shouldly;

namespace OweMe.Application.UnitTests.Ledgers.Commands.Create;

public class CreateHandlerCommandHandlerTests
{
    private Mock<ILedgerContext> _ledgerContextMock;
    private CreateHandlerCommandHandler _handler;
    
    [SetUp]
    public void Setup()
    {
        _ledgerContextMock = new Mock<ILedgerContext>();
        _handler = new CreateHandlerCommandHandler(_ledgerContextMock.Object);
    }
    
    [Test]
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
        _ledgerContextMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.ShouldBe(expectedLedgerId);
        _ledgerContextMock.Verify(x => x.Ledgers.Add(It.Is<Ledger>(ledger =>
            ledger.Name == ledgerName && ledger.Description == ledgerDescription
        )), Times.Once);
        _ledgerContextMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _ledgerContextMock.VerifyNoOtherCalls();
    }
}