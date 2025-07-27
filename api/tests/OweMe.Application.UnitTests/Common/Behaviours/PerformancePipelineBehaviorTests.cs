using Microsoft.Extensions.Logging;
using Moq;
using OweMe.Application.Common.Behaviours;
using Shouldly;

namespace OweMe.Application.UnitTests.Common.Behaviours;

public class PerformancePipelineBehaviorTests
{
    private const int TimeoutThreshold = 4000;
    private const int AlmostToLongWork = (int)(TimeoutThreshold * 0.9);
    private const int LittleToLongWork = (int)(TimeoutThreshold * 1.05);

    [Fact]
    public async Task Handle_Should_Log_Performance_When_Exceeding_Threshold()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PerformancePipelineBehavior<TestRequest, string>>>();
        var behaviour =
            new PerformancePipelineBehavior<TestRequest, string>(loggerMock.Object, TimeoutThreshold);
        var request = new TestRequest { Value = "test" };

        // Simulate a long-running operation
        Task<string> NextWithDelay(CancellationToken cancellationToken)
        {
            return Task.Delay(LittleToLongWork, cancellationToken)
                .ContinueWith<string>(_ => "response", cancellationToken);
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
                    v.ToString()!.Contains("Handled TestRequest in")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v != null &&
                    v.ToString()!.Contains($", exceeding the threshold of {TimeoutThreshold} ms")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Performance_When_Exceeding_Threshold_WithException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PerformancePipelineBehavior<TestRequest, string>>>();

        var behaviour =
            new PerformancePipelineBehavior<TestRequest, string>(loggerMock.Object, TimeoutThreshold);
        var request = new TestRequest { Value = "test" };

        var exception = new Exception("fail");

        // Simulate a long-running operation
        Task<string> NextWithDelayAndException(CancellationToken cancellationToken)
        {
            return Task.Delay(LittleToLongWork, cancellationToken)
                .ContinueWith<string>(_ => throw exception, cancellationToken);
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
                    v.ToString()!.Contains("Handled TestRequest in")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v != null &&
                    v.ToString()!.Contains($", exceeding the threshold of {TimeoutThreshold} ms")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNot_Log_Performance_When_Not_Exceeding_Threshold()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PerformancePipelineBehavior<TestRequest, string>>>();

        var behaviour =
            new PerformancePipelineBehavior<TestRequest, string>(loggerMock.Object, TimeoutThreshold);
        var request = new TestRequest { Value = "test" };

        // Simulate a short operation
        Task<string> NextWithShortDelay(CancellationToken cancellationToken)
        {
            return Task.Delay(AlmostToLongWork, cancellationToken)
                .ContinueWith<string>(_ => "response", cancellationToken);
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
                    v.ToString()!.Contains("Handled TestRequest in")),
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
        var loggerMock = new Mock<ILogger<PerformancePipelineBehavior<TestRequest, string>>>();
        var behaviour =
            new PerformancePipelineBehavior<TestRequest, string>(loggerMock.Object, TimeoutThreshold);
        var request = new TestRequest { Value = "test" };

        // Simulate a short operation
        var exception = new Exception("fail");

        Task<string> NextWithShortDelay(CancellationToken cancellationToken)
        {
            return Task.Delay(AlmostToLongWork, cancellationToken)
                .ContinueWith<string>(_ => throw exception, cancellationToken);
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
                    v.ToString()!.Contains("Handled TestRequest in")),
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