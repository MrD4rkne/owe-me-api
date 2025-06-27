using OweMe.Domain.Ledgers;
using Shouldly;

namespace OweMe.Application.UnitTests.Ledgers;

public class LedgerDtoTests
{
    private static IEnumerable<object[]> ModifiedByAndAtData()
    {
        yield return [Guid.NewGuid(), DateTimeOffset.UtcNow];
        yield return [null, null];
    }

    [Theory]
    [TestCaseSource(nameof(ModifiedByAndAtData))]
    public void FromDomain_MapsAllPropertiesCorrectly(Guid? modifiedBy, DateTimeOffset? modifiedAt)
    {
        // Arrange
        var ledger = new Ledger
        {
            Id = Guid.NewGuid(),
            Name = "Test Ledger",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = modifiedAt,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = modifiedBy
        };

        // Act
        var dto = LedgerDto.FromDomain(ledger);

        // Assert
        dto.Id.ShouldBe(ledger.Id);
        dto.Name.ShouldBe(ledger.Name);
        dto.Description.ShouldBe(ledger.Description);
        dto.CreatedAt.ShouldBe(ledger.CreatedAt);
        dto.UpdatedAt.ShouldBe(ledger.UpdatedAt);
        dto.CreatedBy.ShouldBe(ledger.CreatedBy.Id);
        dto.UpdatedBy.ShouldBe(ledger.UpdatedBy?.Id);
    }
}