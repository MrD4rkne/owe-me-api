using OweMe.Domain.Ledgers;
using OweMe.Domain.Users;
using OweMe.Tests.Common;
using Shouldly;

namespace OweMe.Domain.UnitTests;

public class LedgerTests
{
    [Test]
    public void CanUserAccess_Creator_ShouldReturnTrue()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ledger = new Ledger
        {
            CreatedBy = userId,
            Id = Guid.NewGuid(),
        };

        // Act
        var canAccess = ledger.CanUserAccess(userId);

        // Assert
        canAccess.ShouldBeTrue();
    }
    
    [Test]
    public void CanUserAccess_OtherUser_ShouldReturnFalse()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var otherUserId = new UserId(GuidHelper.CreateDifferentGuid(creatorId));
        
        var ledger = new Ledger
        {
            CreatedBy = creatorId,
            Id = Guid.NewGuid(),
        };

        // Act
        var canAccess = ledger.CanUserAccess(otherUserId);

        // Assert
        canAccess.ShouldBeFalse();
    }
}