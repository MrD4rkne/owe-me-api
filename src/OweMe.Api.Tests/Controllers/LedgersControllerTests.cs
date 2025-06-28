using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OweMe.Api.Controllers;
using OweMe.Application.Common;
using OweMe.Application.Ledgers.Commands.Create;
using OweMe.Domain.Ledgers;
using OweMe.Domain.Ledgers.Queries.Get;
using Shouldly;

namespace OweMe.Api.Tests.Controllers;

public class LedgersControllerTests
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
        var result = await LedgersController.CreateLedger(command, mediatorMock.Object);

        // Assert
        result.ShouldBeOfType<Created>();

        var createdResult = result as Created;
        createdResult.ShouldNotBeNull();
        createdResult.Location.ShouldBe($"/api/ledgers/{ledgerId}");
    }

    [Fact]
    public async Task GetLedger_ReturnsOk_WhenSuccess()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var expectedValue = new LedgerDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Ledger",
            Description = "This is a test ledger.",
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedBy = Guid.NewGuid(),
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var successResult = Result<LedgerDto>.Success(expectedValue);
        mediatorMock.Setup(m => m.Send(It.IsAny<GetLedgerQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await LedgersController.GetLedger(expectedValue.Id, mediatorMock.Object);

        // Assert
        result.ShouldBeOfType<Ok<LedgerDto>>();

        var okResult = result as Ok<LedgerDto>;
        okResult.ShouldNotBeNull();
        okResult.Value.ShouldNotBeNull();
        okResult.Value.ShouldBe(expectedValue);
    }

    [Fact]
    public async Task GetLedger_ReturnsNotFound_WhenLedgerNotFound()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var ledgerId = Guid.NewGuid();
        var returnedResult = Result<LedgerDto>.Failure(LedgerErrors.Errors.LedgerNotFound);

        mediatorMock.Setup(m => m.Send(It.IsAny<GetLedgerQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedResult);

        // Act
        var result = await LedgersController.GetLedger(ledgerId, mediatorMock.Object);

        // Assert
        result.ShouldBeOfType<NotFound<string>>();

        var notFoundResult = result as NotFound<string>;
        notFoundResult.ShouldNotBeNull();
        notFoundResult.Value.ShouldBe(LedgerErrors.Errors.LedgerNotFound.Description);
    }
}