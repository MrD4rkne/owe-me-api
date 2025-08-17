using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OweMe.Api.Endpoints.Ledgers.Get;
using OweMe.Application.Ledgers.Queries.Get;
using OweMe.Domain.Common.Exceptions;
using Shouldly;
using Wolverine;

namespace OweMe.Api.Tests.Endpoints.Ledgers.Get;

public class GetLedgerByIdTests
{
    [Fact]
    public async Task GetLedger_ReturnsOk_WhenSuccess()
    {
        // Arrange
        var messageBusMock = new Mock<IMessageBus>();
        var expectedValue = new GetLedgerResult(Guid.NewGuid(), "Test Ledger", "This is a test ledger.",
            DateTimeOffset.UtcNow, Guid.NewGuid(), DateTimeOffset.UtcNow, Guid.NewGuid());

        messageBusMock.Setup(m =>
                m.InvokeAsync<GetLedgerResult>(It.IsAny<GetLedgerQuery>(), It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(expectedValue);

        // Act
        var result = await GetLedgerByIdEndpoint.GetLedger(expectedValue.Id, messageBusMock.Object);

        // Assert
        result.ShouldBeOfType<Ok<GetLedgerResult>>();

        var okResult = result as Ok<GetLedgerResult>;
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
        var messageBusMock = new Mock<IMessageBus>();
        messageBusMock.Setup(m =>
                m.InvokeAsync<GetLedgerResult>(It.IsAny<GetLedgerQuery>(), It.IsAny<CancellationToken>(), null))
            .Throws(new NotFoundException("Ledger not found."));

        var ledgerId = Guid.NewGuid();

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(async () =>
        {
            await GetLedgerByIdEndpoint.GetLedger(ledgerId, messageBusMock.Object);
        });
    }
}