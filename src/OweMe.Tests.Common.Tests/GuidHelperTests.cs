using NUnit.Framework;
using Shouldly;

namespace OweMe.Tests.Common;

public class GuidHelperTests
{
    [Test]
    public void CreateDifferentGuid_ShouldReturnDifferentGuid()
    {
        var existingGuid = Guid.NewGuid();
        var newGuid = GuidHelper.CreateDifferentGuid(existingGuid);
        newGuid.ShouldNotBe(existingGuid);
    }

    [Test]
    public void CreateDifferentGuid_ShouldThrowExceptionAfterMaxAttempts()
    {
        var existingGuid = Guid.NewGuid();
        Assert.Throws<InvalidOperationException>(() => GuidHelper.CreateDifferentGuid(existingGuid, 0));
    }
}