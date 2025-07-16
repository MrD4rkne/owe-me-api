using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using OweMe.Application.Common.Results;

namespace OweMe.Application.Common.Behaviours;

public class ValidationPipelineBehaviour<TRequest, TResponse>(
    ILogger<ValidationPipelineBehaviour<TRequest, TResponse>> logger,
    IEnumerable<IValidator<TRequest>> validators)
    : IResultPipelineBehaviour<TRequest, TResponse>
    where TRequest : IRequest<Result<TResponse>>
{
    private readonly ILogger<ValidationPipelineBehaviour<TRequest, TResponse>> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly IEnumerable<IValidator<TRequest>> _validators =
        validators ?? throw new ArgumentNullException(nameof(validators));

    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next,
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
                return Result<TResponse>.Failure(ValidationError.From(validationFailures));
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