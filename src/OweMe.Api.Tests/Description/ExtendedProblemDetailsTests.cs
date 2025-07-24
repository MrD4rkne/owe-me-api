using Microsoft.AspNetCore.Mvc;
using OweMe.Api.Description;

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
                { "requestId", "67890" },
                { "a", "b" }
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
        Assert.Contains(extendedProblemDetails.Extensions, kvp => kvp.Key == "traceId" && kvp.Value.ToString() == "12345");
        Assert.Contains(extendedProblemDetails.Extensions, kvp => kvp.Key == "requestId" && kvp.Value.ToString() == "67890");
        Assert.Contains(extendedProblemDetails.Extensions, kvp => kvp.Key == "a" && kvp.Value.ToString() == "b");
    }
}