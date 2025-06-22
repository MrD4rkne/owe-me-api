using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace OweMe.Application.Common.Behaviours;

public class PerformancePipelineBehaviour<TRequest, TResponse>(ILogger logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int MaximumElapsedMilliseconds = 500; // Threshold for performance logging
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        
        var stopwatch = Stopwatch.StartNew();
        try
        {
            // Await must be used not to trigger finally block before the request is processed.
            return await next(cancellationToken);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds} ms", requestName, stopwatch.ElapsedMilliseconds);
            
            if (stopwatch.ElapsedMilliseconds > MaximumElapsedMilliseconds)
            {
                logger.LogWarning("Performance warning: {RequestName} took {ElapsedMilliseconds} ms, exceeding the threshold of {Threshold} ms",
                    requestName, stopwatch.ElapsedMilliseconds, MaximumElapsedMilliseconds);
            }
        }
    }
}