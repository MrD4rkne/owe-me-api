using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OweMe.Domain.Common.Exceptions;

namespace OweMe.Api.Description;

public sealed class ExceptionProblemDetailsMatcher(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationException validationException => EnrichWithException(validationException),
            NotFoundException notFoundException => EnrichWithException(notFoundException),
            ForbiddenException forbiddenException => EnrichWithException(forbiddenException),
            BadHttpRequestException badHttpRequestException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request",
                Detail = badHttpRequestException.Message
            },
            _ => new ProblemDetails
            {
                Title = "An unexpected error occurred",
                Detail = "An unexpected error occurred while processing your request.",
                Status = StatusCodes.Status500InternalServerError
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        return problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static ExtendedProblemDetails EnrichWithException(ValidationException exception)
    {
        return new ExtendedProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Detail = exception.Message,
            Errors = exception.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray())
        };
    }

    private static ProblemDetails EnrichWithException(NotFoundException exception)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Resource not found",
            Detail = exception.Message
        };
        return problemDetails;
    }

    private static ProblemDetails EnrichWithException(ForbiddenException exception)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Access forbidden",
            Detail = exception.Message
        };
        return problemDetails;
    }
}