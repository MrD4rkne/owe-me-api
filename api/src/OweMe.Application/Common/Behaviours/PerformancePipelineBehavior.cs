using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wolverine.Runtime;

namespace OweMe.Application.Common.Behaviours;

public class PerformancePipelineBehavior(
    ILogger<PerformancePipelineBehavior> logger,
    IOptions<ApplicationOptions> options)
{
    private readonly Stopwatch _stopwatch = new();

    private long maximumElapsedMilliseconds =>
        options.Value.TooLongRequestThresholdMs > 0
            ? options.Value.TooLongRequestThresholdMs
            : 500; // Default threshold if not set

    public void Before(MessageContext context)
    {
        _stopwatch.Start();
        string? requestName = context.Envelope.MessageType;
        logger.LogInformation("Started processing {RequestName}.", requestName);
    }

    public void Finally(MessageContext context)
    {
        _stopwatch.Stop();
        string? requestName = context.Envelope.MessageType;
        logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds} ms.", requestName,
            _stopwatch.ElapsedMilliseconds);

        if (_stopwatch.ElapsedMilliseconds > maximumElapsedMilliseconds)
        {
            logger.LogWarning(
                "Performance warning: {RequestName} took {ElapsedMilliseconds} ms, exceeding the threshold of {Threshold} ms",
                requestName, _stopwatch.ElapsedMilliseconds, maximumElapsedMilliseconds);
        }
    }
}