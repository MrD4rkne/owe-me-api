using OweMe.Domain.Common.Exceptions;
using OweMe.Tests.Common;
using Shouldly;

namespace OweMe.Domain.UnitTests.Common.Exceptions;

public class ValidationExceptionTests : BaseExceptionTest<ValidationException>
{
    [Fact]
    public void Constructor_WithErrors_SetsErrorsProperty()
    {
        // Arrange
        var errors = new List<KeyValuePair<string, string>>
        {
            new("Field1", "Error1"),
            new("Field2", "Error2")
        };

        // Act
        var ex = new ValidationException("Test message", errors);

        // Assert
        ex.Errors.ShouldBeEquivalentTo(errors);
    }
}