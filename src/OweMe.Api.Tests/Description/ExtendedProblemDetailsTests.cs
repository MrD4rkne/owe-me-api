using Microsoft.AspNetCore.Mvc;
using OweMe.Api.Common;

namespace OweMe.Api.Tests.Description;

public class ExtendedProblemDetailsTests
{
    [Fact]
    public void Should_Initialize_ExtendedProblemDetails_From_ProblemDetails()
    {
        // Arrange
        var problemDetails = new ProblemDetails
        {
            Title = "Test Error",
            Detail = "This is a test error detail.",
            Status = 400,
            Type = "https://example.com/test-error",
            Instance = "/test/instance",
            Extensions =
            {
                { "traceId", "12345" },
                { "requestId", "67890" }
            }
        };

        // Act
        var extendedProblemDetails = new ExtendedProblemDetails(problemDetails);

        // Assert
        Assert.Equal(problemDetails.Title, extendedProblemDetails.Title);
        Assert.Equal(problemDetails.Detail, extendedProblemDetails.Detail);
        Assert.Equal(problemDetails.Status, extendedProblemDetails.Status);
        Assert.Equal(problemDetails.Type, extendedProblemDetails.Type);
        Assert.Equal(problemDetails.Instance, extendedProblemDetails.Instance);
        Assert.Equal("12345", extendedProblemDetails.TraceId);
        Assert.Equal("67890", extendedProblemDetails.RequestId);
    }
}