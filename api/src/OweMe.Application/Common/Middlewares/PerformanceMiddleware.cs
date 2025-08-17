using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wolverine;

namespace OweMe.Application.Common.Middlewares;

public class PerformanceMiddleware(
    ILogger<PerformanceMiddleware> logger,
    IOptions<ApplicationOptions> options)
{
    private long? _startTime;

    private long maximumElapsedMilliseconds =>
        options.Value.TooLongRequestThresholdMs > 0
            ? options.Value.TooLongRequestThresholdMs
            : 500; // Default threshold if not set

    public void Before(IMessageContext context)
    {
        if (_startTime is not null)
        {
            throw new InvalidOperationException(
                $"{nameof(PerformanceMiddleware)} is already running. Ensure that {nameof(Before)} is called only once per request.");
        }

        _startTime = Stopwatch.GetTimestamp();
        string? requestName = context.Envelope.MessageType;
        logger.LogInformation("Started processing {RequestName}.", requestName);
    }

    public void Finally(IMessageContext context)
    {
        if (_startTime is null)
        {
            throw new InvalidOperationException(
                $"{nameof(PerformanceMiddleware)} has not been started. Ensure that {nameof(Before)} is called before {nameof(Finally)}.");
        }

        var elapsed = Stopwatch.GetElapsedTime(_startTime.Value);
        string? requestName = context.Envelope.MessageType;
        logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds} ms.", requestName,
            elapsed.Milliseconds);

        if (elapsed.Milliseconds > maximumElapsedMilliseconds)
        {
            logger.LogWarning(
                "Performance warning: {RequestName} took {ElapsedMilliseconds} ms, exceeding the threshold of {Threshold} ms",
                requestName, elapsed.Milliseconds, maximumElapsedMilliseconds);
        }
    }
}