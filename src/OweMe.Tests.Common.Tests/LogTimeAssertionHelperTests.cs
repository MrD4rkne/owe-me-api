using Shouldly;

namespace OweMe.Tests.Common;

public class LogTimeAssertionHelperTests
{
    [Test]
    public void ContainsExpectedTime_Should_Validate_Time_Within_Percentage()
    {
        // Arrange
        var logMessage = "Handled TestRequest in 600 ms";
        long expectedTime = 600;
        double percent = 10; // 10% tolerance

        // Act & Assert
        Assert.DoesNotThrow(() => LogTimeAssertionHelper.ContainsExpectedTime(logMessage, expectedTime, percent));
    }

    [Test]
    public Task ContainsExpectedTime_Should_Fail_When_Time_Outside_Percentage()
    {
        // Arrange
        var logMessage = "Handled TestRequest in 600 ms";
        long expectedTime = 500;
        double percent = 10; // 10% tolerance

        // Act & Assert
        Should.Throw<Exception>(() => LogTimeAssertionHelper.ContainsExpectedTime(logMessage, expectedTime, percent));
        return Task.CompletedTask;
    }

    [Test]
    public Task ContainsExpectedTime_Should_Fail_When_Time_Not_Parsed()
    {
        // Arrange
        var logMessage = "Handled TestRequest in abc ms"; // Invalid time
        long expectedTime = 600;
        double percent = 10; // 10% tolerance

        // Act & Assert
        Should.Throw<Exception>(() => LogTimeAssertionHelper.ContainsExpectedTime(logMessage, expectedTime, percent));
        return Task.CompletedTask;
    }

    [Test]
    public Task ContainsExpectedTime_Should_Fail_When_Unit_Mismatch()
    {
        // Arrange
        var logMessage = "Handled TestRequest in 600 seconds"; // Different unit
        long expectedTime = 600;
        double percent = 10; // 10% tolerance

        // Act & Assert
        Should.Throw<Exception>(() => LogTimeAssertionHelper.ContainsExpectedTime(logMessage, expectedTime, percent));
        return Task.CompletedTask;
    }
}