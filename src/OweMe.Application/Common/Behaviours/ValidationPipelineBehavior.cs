using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace OweMe.Application.Common.Behaviours;

public class ValidationPipelineBehavior<TRequest, TResponse>(
    ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger,
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<ValidationPipelineBehavior<TRequest, TResponse>> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly IEnumerable<IValidator<TRequest>> _validators =
        validators ?? throw new ArgumentNullException(nameof(validators));

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationFailures = (await ValidateAsync(context, cancellationToken))
                .ToArray();
            if (validationFailures.Length != 0)
            {
                _logger.LogWarning("Validation failed for request {RequestName}: {Errors}",
                    typeof(TRequest).Name, validationFailures);
                throw new ValidationException(validationFailures);
            }

            _logger.LogDebug("Validation passed for request {RequestName}", typeof(TRequest).Name);
        }
        else
        {
            _logger.LogDebug("No validators found for request {RequestName}, skipping validation",
                typeof(TRequest).Name);
        }

        return await next(cancellationToken);
    }

    private async Task<IEnumerable<ValidationFailure>> ValidateAsync(ValidationContext<TRequest> context,
        CancellationToken cancellationToken = default)
    {
        var validationTasks = _validators
            .Select(v => v.ValidateAsync(context, cancellationToken));

        var validationResults = await Task.WhenAll(validationTasks);

        return validationResults
            .SelectMany(result => result.Errors)
            .Where(f => f is not null);
    }
}