using OweMe.Application.Common;
using Shouldly;

namespace OweMe.Application.UnitTests.Common;

public class ResultTests
{
    [Fact]
    public void ResultT_Success_ShouldCreateSuccessResultWithValue()
    {
        // Arrange
        const int value = 42;

        // Act
        var result = Result<int>.Success(value);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBe(Error.None);
        result.Value.ShouldBe(value);
    }

    [Fact]
    public void ResultT_Failure_ShouldCreateFailureResult()
    {
        // Arrange
        var error = new Error("Failed", "An error occurred");

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void ResultT_Failure_ShouldThrowOnValueAccess()
    {
        // Arrange
        var error = new Error("Failed", "An error occurred");
        var result = Result<string>.Failure(error);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => _ = result.Value)
            .Message.ShouldBe("Cannot access Value on a failed result.");
    }
}