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
        Extensions = problemDetails.Extensions;

        if (problemDetails.Extensions.TryGetValue(TraceIdKey, out object? traceId))
        {
            TraceId = traceId?.ToString();
        }

        if (problemDetails.Extensions.TryGetValue(RequestIdKey, out object? requestId))
        {
            RequestId = requestId?.ToString();
        }
    }

    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

    public string? TraceId { get; set; }

    public string? RequestId { get; set; }
}