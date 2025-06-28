using Shouldly;

namespace OweMe.Tests.Common;

public class GuidHelperTests
{
    [Fact]
    public void CreateDifferentGuid_ShouldReturnDifferentGuid()
    {
        var existingGuid = Guid.NewGuid();
        var newGuid = GuidHelper.CreateDifferentGuid(existingGuid);
        newGuid.ShouldNotBe(existingGuid);
    }

    [Fact]
    public void CreateDifferentGuid_ShouldThrowException_WithInvalidMaxAttempts()
    {
        var existingGuid = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() => GuidHelper.CreateDifferentGuid(existingGuid, 0));
    }
}