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

        (TraceId, RequestId) = TryExtractTraceAndRequestId(problemDetails);

        if (problemDetails is ExtendedProblemDetails extendedDetails)
        {
            Errors = extendedDetails.Errors ?? new Dictionary<string, string[]>();
        }
    }

    public Dictionary<string, string[]> Errors { get; init; } = new();

    public string? TraceId { get; init; }

    public string? RequestId { get; init; }
    
    private static (string, string) TryExtractTraceAndRequestId(ProblemDetails problemDetails)
    {
        string? traceId = null;
        string? requestId = null;
        
        if (problemDetails.Extensions.TryGetValue(TraceIdKey, out object? extractedTraceId))
        {
            traceId = extractedTraceId?.ToString();
        }

        if (problemDetails.Extensions.TryGetValue(RequestIdKey, out object? extractedRequestId))
        {
            requestId = extractedRequestId?.ToString();
        }
        
        return (traceId, requestId);
    }
}