using Microsoft.Extensions.Logging;
using Moq;
using OweMe.Application.Common.Behaviours;
using OweMe.Tests.Common;
using Shouldly;

namespace OweMe.Application.UnitTests.Common.Behaviours;

public class PerformancePipelineBehaviourTests
{
    private const int timeoutThreshold = 2000;
    private const int almostToLongWork = (int) (timeoutThreshold * 0.9);
    private const int littleToLongWork = (int) (timeoutThreshold * 1.05);
    private const double expectedAccuracyPercent = 5;
    private const string expectedTimeUnit = "ms";

    private static bool AssertTime(string logMessage, long expectedTime, double percent = 0.01, string unit = "ms")
    {
        try
        {
            LogTimeAssertionHelper.ContainsExpectedTime(logMessage, expectedTime, percent, unit);
            return true;
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
            return false;
        }
    }

    [Fact]
    public async Task Handle_Should_Log_Performance_When_Exceeding_Threshold()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PerformancePipelineBehaviour<TestRequest, string>>>();
        var behaviour = new PerformancePipelineBehaviour<TestRequest, string>(loggerMock.Object, timeoutThreshold);
        var request = new TestRequest { Value = "test" };

        // Simulate a long-running operation
        Task<string> NextWithDelay(CancellationToken cancellationToken)
        {
            return Task.Delay(littleToLongWork, cancellationToken).ContinueWith(_ => "response", cancellationToken);
        }

        // Act
        string result = await behaviour.Handle(request, NextWithDelay, CancellationToken.None);

        // Assert
        result.ShouldBe("response");
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v != null &&
                    v.ToString()!.Contains("Handled TestRequest in") && AssertTime(v.ToString()!, littleToLongWork,
                        expectedAccuracyPercent, expectedTimeUnit)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v != null &&
                    v.ToString()!.Contains($", exceeding the threshold of {delayThreshold} ms")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Performance_When_Exceeding_Threshold_WithException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PerformancePipelineBehaviour<TestRequest, string>>>();
        
        var behaviour = new PerformancePipelineBehaviour<TestRequest, string>(loggerMock.Object, timeoutThreshold);
        var request = new TestRequest { Value = "test" };

        var exception = new Exception("fail");

        // Simulate a long-running operation
        Task<string> NextWithDelayAndException(CancellationToken cancellationToken)
        {
            return Task.Delay(littleToLongWork, cancellationToken).ContinueWith<string>(_ => throw exception, cancellationToken);
        }

        // Act and Assert
        await Should.ThrowAsync<Exception>(() =>
            behaviour.Handle(request, NextWithDelayAndException, CancellationToken.None));

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v != null &&
                    v.ToString()!.Contains("Handled TestRequest in") && AssertTime(v.ToString()!, littleToLongWork,
                        expectedAccuracyPercent, expectedTimeUnit)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v != null &&
                    v.ToString()!.Contains($", exceeding the threshold of {delayThreshold} ms")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNot_Log_Performance_When_Not_Exceeding_Threshold()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PerformancePipelineBehaviour<TestRequest, string>>>();
        
        var behaviour = new PerformancePipelineBehaviour<TestRequest, string>(loggerMock.Object, timeoutThreshold);
        var request = new TestRequest { Value = "test" };

        // Simulate a short operation
        Task<string> NextWithShortDelay(CancellationToken cancellationToken)
        {
            return Task.Delay(almostToLongWork, cancellationToken).ContinueWith(_ => "response", cancellationToken);
        }

        // Act
        string result = await behaviour.Handle(request, NextWithShortDelay, CancellationToken.None);

        // Assert
        result.ShouldBe("response");
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v != null &&
                    v.ToString()!.Contains("Handled TestRequest in") && AssertTime(v.ToString()!, almostToLongWork,
                        expectedAccuracyPercent, expectedTimeUnit)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNot_Log_Performance_When_Not_Exceeding_Threshold_WithException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PerformancePipelineBehaviour<TestRequest, string>>>();
        var behaviour = new PerformancePipelineBehaviour<TestRequest, string>(loggerMock.Object);
        var request = new TestRequest { Value = "test" };

        // Simulate a short operation
        var exception = new Exception("fail");

        Task<string> NextWithShortDelay(CancellationToken cancellationToken)
        {
            return Task.Delay(almostToLongWork, cancellationToken).ContinueWith<string>(_ => throw exception, cancellationToken);
        }

        // Act
        await Should.ThrowAsync<Exception>(() => behaviour.Handle(request, NextWithShortDelay, CancellationToken.None));

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v != null &&
                    v.ToString()!.Contains("Handled TestRequest in") && AssertTime(v.ToString()!, almostToLongWork,
                        expectedAccuracyPercent, expectedTimeUnit)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
}