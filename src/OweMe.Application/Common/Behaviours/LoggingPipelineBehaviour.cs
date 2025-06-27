using MediatR;
using Microsoft.Extensions.Logging;

namespace OweMe.Application.Common.Behaviours;

#pragma warning disable S2139 // Exceptions should be either logged or rethrown but not both

public class LoggingPipelineBehaviour<TRequest, TResponse>(ILogger logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        logger.LogInformation("Handling {RequestName} with request: {@Request}", requestName, request);
        try
        {
            var response = await next(cancellationToken);
            logger.LogInformation("Handled {RequestName} successfully with response: {@Response}", requestName,
                response);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling {RequestName} with request: {@Request}", requestName, request);
            throw;
        }
    }
}

#pragma warning restore S2139 // Exceptions should be either logged or rethrown but not both