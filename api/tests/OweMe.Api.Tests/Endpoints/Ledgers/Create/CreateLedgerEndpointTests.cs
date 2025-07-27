using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OweMe.Api.Endpoints.Ledgers.Create;
using OweMe.Application.Ledgers.Commands.Create;
using Shouldly;

namespace OweMe.Api.Tests.Endpoints.Ledgers.Create;

public class CreateLedgerEndpointTests
{
    [Fact]
    public async Task CreateLedger_ReturnsCreatedResult()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var request = new CreateLedgerRequest
        {
            Name = "Test Ledger",
            Description = "This is a test ledger."
        };

        var ledgerId = Guid.NewGuid();
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateLedgerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ledgerId);

        // Act
        var result = await CreateLedgerEndpoint.CreateLedger(request, mediatorMock.Object);

        // Assert
        result.ShouldBeOfType<Created>();

        var createdResult = result as Created;
        createdResult.ShouldNotBeNull();
        createdResult.Location.ShouldBe($"/api/ledgers/{ledgerId}");

        mediatorMock.Verify(m =>
            m.Send(
                It.Is<CreateLedgerCommand>(command =>
                    command.Name == request.Name && command.Description == request.Description),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}