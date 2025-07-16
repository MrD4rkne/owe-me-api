using OweMe.Application.Common.Results;
using Shouldly;

namespace OweMe.Application.UnitTests.Common;

public class ErrorTests
{
    [Fact]
    public void Errors_WithSameCodeAndDescription_ShouldBeEqual()
    {
        // Arrange
        var e1 = new Error("A", "desc");
        var e2 = new Error("A", "desc");

        // Act & Assert
        e1.ShouldBe(e2);
        (e1 == e2).ShouldBeTrue();
        (e1 != e2).ShouldBeFalse();
        e1.Equals(e2).ShouldBeTrue();
        e1.Equals((object)e2).ShouldBeTrue();
    }

    [Fact]
    public void Errors_WithDifferentCode_ShouldNotBeEqual()
    {
        // Arrange
        var e1 = new Error("A", "desc");
        var e2 = new Error("B", "desc");

        // Act & Assert
        e1.ShouldNotBe(e2);
        (e1 == e2).ShouldBeFalse();
        (e1 != e2).ShouldBeTrue();
    }

    [Fact]
    public void Errors_WithDifferentDescription_ShouldNotBeEqual()
    {
        // Arrange
        var e1 = new Error("A", "desc");
        var e2 = new Error("A", "other");

        // Act & Assert
        e1.ShouldNotBe(e2);
        (e1 == e2).ShouldBeFalse();
        (e1 != e2).ShouldBeTrue();
    }

    [Fact]
    public void Errors_WithDifferentCodeAndDescription_ShouldNotBeEqual()
    {
        // Arrange
        var e1 = new Error("A", "desc");
        var e2 = new Error("B", "other");

        // Act & Assert
        e1.ShouldNotBe(e2);
        (e1 == e2).ShouldBeFalse();
        (e1 != e2).ShouldBeTrue();
    }

    [Fact]
    public void None_ShouldBeDefaultError()
    {
        // Arrange & Act
        var none = Error.None;
        none.Code.ShouldBe(string.Empty);
        none.Description.ShouldBe(string.Empty);
        none.ShouldBe(new Error(string.Empty, string.Empty));
    }

    [Fact]
    public void GetHashCode_ShouldBeEqualForEqualErrors()
    {
        var e1 = new Error("A", "desc");
        var e2 = new Error("A", "desc");

        e1.GetHashCode().ShouldBe(e2.GetHashCode());
    }
}