using OweMe.Domain.Users;
using Shouldly;

namespace OweMe.Domain.UnitTests.Users;

public class UserIdTests
{
    [Fact]
    public void New_ShouldReturnUniqueUserIds()
    {
        // Arrange & Act
        var id1 = UserId.New();
        var id2 = UserId.New();

        // Assert
        id1.ShouldNotBe(id2);
        id1.Id.ShouldNotBe(Guid.Empty);
        id2.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Empty_ShouldReturnUserIdWithEmptyGuid()
    {
        // Act
        var empty = UserId.Empty;

        // Assert
        empty.Id.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void ToString_ShouldReturnGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        // Act
        var result = userId.ToString();

        // Assert
        result.ShouldBe(guid.ToString());
    }

    [Fact]
    public void ImplicitConversion_ToGuid_ShouldReturnGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        // Act
        Guid result = userId;

        // Assert
        result.ShouldBe(guid);
    }

    [Fact]
    public void ImplicitConversion_FromGuid_ShouldReturnUserId()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        UserId userId = guid;

        // Assert
        userId.Id.ShouldBe(guid);
    }
}