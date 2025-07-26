using Microsoft.AspNetCore.Mvc;

namespace OweMe.Api.Description;

public class ExtendedProblemDetails : ProblemDetails
{
    private const string TraceIdKey = "traceId";
    private const string RequestIdKey = "requestId";

    public ExtendedProblemDetails()
    {
    }

    public ExtendedProblemDetails(ProblemDetails problemDetails)
    {
        Title = problemDetails.Title;
        Detail = problemDetails.Detail;
        Status = problemDetails.Status;
        Type = problemDetails.Type;
        Instance = problemDetails.Instance;
        Extensions = problemDetails.Extensions ?? new Dictionary<string, object?>();

        if (problemDetails.Extensions.TryGetValue(TraceIdKey, out object? traceId))
        {
            TraceId = traceId?.ToString();
        }

        if (problemDetails.Extensions.TryGetValue(RequestIdKey, out object? requestId))
        {
            RequestId = requestId?.ToString();
        }

        if (problemDetails is ExtendedProblemDetails extendedDetails)
        {
            Errors = extendedDetails.Errors ?? new Dictionary<string, string[]>();
        }
    }

    public Dictionary<string, string[]> Errors { get; init; } = new();

    public string? TraceId { get; init; }

    public string? RequestId { get; init; }
}