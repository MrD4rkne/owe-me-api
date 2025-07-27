using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OweMe.Api.Endpoints.Ledgers.Get;
using OweMe.Application.Ledgers;
using OweMe.Application.Ledgers.Queries.Get;
using OweMe.Domain.Common.Exceptions;
using Shouldly;

namespace OweMe.Api.Tests.Endpoints.Ledgers.Get;

public class GetLedgerByIdTests
{
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

        mediatorMock.Setup(m => m.Send(It.IsAny<GetLedgerQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedValue);

        // Act
        var result = await GetLedgerByIdEndpoint.GetLedger(expectedValue.Id, mediatorMock.Object);

        // Assert
        result.ShouldBeOfType<Ok<GetLedgerResponse>>();

        var okResult = result as Ok<GetLedgerResponse>;
        okResult.ShouldNotBeNull();
        okResult.Value.ShouldNotBeNull();

        okResult.Value.Id.ShouldBe(expectedValue.Id);
        okResult.Value.Name.ShouldBe(expectedValue.Name);
        okResult.Value.Description.ShouldBe(expectedValue.Description);
        okResult.Value.CreatedAt.ShouldBe(expectedValue.CreatedAt);
        okResult.Value.CreatedBy.ShouldBe(expectedValue.CreatedBy);
        okResult.Value.UpdatedAt.ShouldBe(expectedValue.UpdatedAt);
        okResult.Value.UpdatedBy.ShouldBe(expectedValue.UpdatedBy);
    }

    [Fact]
    public async Task GetLedger_ReturnsNotFound_WhenLedgerNotFound()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<GetLedgerQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Ledger not found."));

        var ledgerId = Guid.NewGuid();

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(async () =>
        {
            await GetLedgerByIdEndpoint.GetLedger(ledgerId, mediatorMock.Object);
        });
    }
}