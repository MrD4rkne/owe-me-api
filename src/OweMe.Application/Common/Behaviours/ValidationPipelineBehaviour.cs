using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace OweMe.Application.Common.Behaviours;

public class ValidationPipelineBehaviour<TRequest, TResponse>(
    ILogger<ValidationPipelineBehaviour<TRequest, TResponse>> logger,
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidationPipelineBehaviour<TRequest, TResponse>> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly IEnumerable<IValidator<TRequest>> _validators =
        validators ?? throw new ArgumentNullException(nameof(validators));

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            await ValidateAndThrowIfInvalid(context, cancellationToken);

            _logger.LogDebug("Validation passed for request {RequestName}", typeof(TRequest).Name);
        }
        else
        {
            _logger.LogDebug("No validators found for request {RequestName}, skipping validation",
                typeof(TRequest).Name);
        }

        return await next(cancellationToken);
    }

    private async Task ValidateAndThrowIfInvalid(ValidationContext<TRequest> context,
        CancellationToken cancellationToken = default)
    {
        var validationTasks = _validators
            .Select(v => v.ValidateAsync(context, cancellationToken));

        var validationResults = await Task.WhenAll(validationTasks);

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return;
        }

        _logger.LogWarning("Validation errors for request {RequestName}: {Errors}", typeof(TRequest).Name, failures);
        throw new ValidationException(failures);
    }
}