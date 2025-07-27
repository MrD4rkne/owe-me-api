using Microsoft.AspNetCore.Mvc;
using OweMe.Api.Description;
using Shouldly;

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
        extendedProblemDetails.Title.ShouldBe(problemDetails.Title);
        extendedProblemDetails.Detail.ShouldBe(problemDetails.Detail);
        extendedProblemDetails.Status.ShouldBe(problemDetails.Status);
        extendedProblemDetails.Type.ShouldBe(problemDetails.Type);
        extendedProblemDetails.Instance.ShouldBe(problemDetails.Instance);
        extendedProblemDetails.Errors.ShouldBeEmpty();
        extendedProblemDetails.TraceId.ShouldBe("12345");
        extendedProblemDetails.RequestId.ShouldBe("67890");
        extendedProblemDetails.Extensions.ShouldNotContainKey("traceId");
        extendedProblemDetails.Extensions.ShouldNotContainKey("requestId");
        extendedProblemDetails.Extensions.ShouldContainKeyAndValue("a", "b");
    }

    [Fact]
    public void Should_Initialize_ExtendedProblemDetails_From_ExtendedProblemDetails()
    {
        // Arrange
        var problemDetails = new ExtendedProblemDetails
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
            },
            Errors = new Dictionary<string, string[]>
            {
                { "Field1", ["Error1"] },
                { "Field2", ["Error2"] }
            }
        };

        // Act
        var extendedProblemDetails = new ExtendedProblemDetails(problemDetails);

        // Assert
        extendedProblemDetails.Title.ShouldBe(problemDetails.Title);
        extendedProblemDetails.Detail.ShouldBe(problemDetails.Detail);
        extendedProblemDetails.Status.ShouldBe(problemDetails.Status);
        extendedProblemDetails.Type.ShouldBe(problemDetails.Type);
        extendedProblemDetails.Instance.ShouldBe(problemDetails.Instance);
        extendedProblemDetails.Errors.ShouldBe(problemDetails.Errors);
        extendedProblemDetails.TraceId.ShouldBe("12345");
        extendedProblemDetails.RequestId.ShouldBe("67890");
        extendedProblemDetails.Extensions.ShouldContainKeyAndValue("traceId", "12345");
        extendedProblemDetails.Extensions.ShouldContainKeyAndValue("requestId", "67890");
        extendedProblemDetails.Extensions.ShouldContainKeyAndValue("a", "b");
// Removed redundant assertion as equivalence is already verified on line 78.
    }
}