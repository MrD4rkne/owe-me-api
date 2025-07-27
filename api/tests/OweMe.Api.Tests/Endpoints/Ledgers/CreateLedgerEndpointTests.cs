using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OweMe.Api.Endpoints.Ledgers;
using OweMe.Application.Ledgers.Commands.Create;
using Shouldly;

namespace OweMe.Api.Tests.Endpoints.Ledgers;

public class CreateLedgerEndpointTests
{
    [Fact]
    public async Task CreateLedger_ReturnsCreatedResult()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var command = new CreateLedgerCommand
        {
            Name = "Test Ledger",
            Description = "This is a test ledger."
        };

        var ledgerId = Guid.NewGuid();
        mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ledgerId);

        // Act
        var result = await CreateLedgerEndpoint.CreateLedger(command, mediatorMock.Object);

        // Assert
        result.ShouldBeOfType<Created>();

        var createdResult = result as Created;
        createdResult.ShouldNotBeNull();
        createdResult.Location.ShouldBe($"/api/ledgers/{ledgerId}");
    }
}