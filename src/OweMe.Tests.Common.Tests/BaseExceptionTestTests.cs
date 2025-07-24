using Shouldly;

namespace OweMe.Tests.Common.Tests;

public class BaseExceptionTestTests
{
    [Fact]
    public void
        OnValidExceptionType_DoNotThrow_In_Constructor_WithMessageAndInnerException_SetsMessageAndInnerException()
    {
        // Arrange
        var testClass = new ValidExceptionTestClass();

        // Act & Assert
        Should.NotThrow(() => testClass.Constructor_WithMessageAndInnerException_SetsMessageAndInnerException());
    }

    [Fact]
    public void OnValidExceptionType_DoNotThrow_In_Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var testClass = new ValidExceptionTestClass();

        // Act & Assert
        Should.NotThrow(() => testClass.Constructor_WithMessage_SetsMessage());
    }

    [Fact]
    public void OnInvalidExceptionType_Throw_In_Constructor_WithMessageAndInnerException_SetsMessageAndInnerException()
    {
        // Arrange
        var testClass = new InvalidExceptionTestClass();

        // Act & Assert
        Should.Throw<ShouldAssertException>(() =>
            testClass.Constructor_WithMessageAndInnerException_SetsMessageAndInnerException());
    }

    [Fact]
    public void OnInvalidExceptionType_Throw_In_Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var testClass = new InvalidExceptionTestClass();
        // Act & Assert
        Should.Throw<ShouldAssertException>(() =>
            testClass.Constructor_WithMessage_SetsMessage());
    }
}

internal class ExampleTest : BaseExceptionTest<NotImplementedException>;

internal class ValidExceptionTestClass : BaseExceptionTest<ValidException>;

internal class InvalidExceptionTestClass : BaseExceptionTest<InvalidException>;

internal class ValidException : Exception
{
    public ValidException(string message) : base(message)
    {
    }

    public ValidException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

internal class InvalidException : Exception
{
    public InvalidException(string message) : base(message + " Invalid")
    {
    }

    public InvalidException(string message, Exception innerException) : base(message + " Invalid",
        new Exception("something", innerException))
    {
    }
}