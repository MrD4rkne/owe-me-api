using Shouldly;
using Xunit;

namespace OweMe.Tests.Common;

public abstract class BaseExceptionTest<T> where T : Exception
{
    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsMessageAndInnerException()
    {
        // Arrange
        var inner = new Exception("Inner exception");
        const string message = "Test message";

        // Act
        var exception = (T)Activator.CreateInstance(typeof(T), message, inner);

        // Assert
        exception.ShouldNotBeNull();
        exception.Message.ShouldBe(message);
        exception.InnerException.ShouldBe(inner);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        const string message = "Test message";

        var ex = (T)Activator.CreateInstance(typeof(T), message);

        // Assert
        ex.Message.ShouldBe(message);
    }
}