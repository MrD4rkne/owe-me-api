using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OweMe.Api.Endpoints.Ledgers.Create;
using OweMe.Application.Ledgers.Commands.Create;
using Shouldly;
using Wolverine;

namespace OweMe.Api.Tests.Endpoints.Ledgers.Create;

public class CreateLedgerEndpointTests
{
    [Fact]
    public async Task CreateLedger_ReturnsCreatedResult()
    {
        // Arrange
        var messageBusMock = new Mock<IMessageBus>();
        var request = new CreateLedgerCommand
        {
            Name = "Test Ledger",
            Description = "This is a test ledger."
        };

        var ledgerId = Guid.NewGuid();
        messageBusMock.Setup(m =>
                m.InvokeAsync<Guid>(It.IsAny<CreateLedgerCommand>(), It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(ledgerId);

        // Act
        var result = await CreateLedgerEndpoint.CreateLedger(request, messageBusMock.Object);

        // Assert
        result.ShouldBeOfType<Created>();

        var createdResult = result as Created;
        createdResult.ShouldNotBeNull();
        createdResult.Location.ShouldBe($"/api/ledgers/{ledgerId}");

        messageBusMock.Verify(m =>
            m.InvokeAsync<Guid>(
                It.Is<CreateLedgerCommand>(command =>
                    command.Name == request.Name && command.Description == request.Description),
                It.IsAny<CancellationToken>(), null), Times.Once);
    }
}