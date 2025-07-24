using Microsoft.AspNetCore.Http;
using Moq;
using OweMe.Api.Description;
using OweMe.Domain.Common.Exceptions;
using Shouldly;

namespace OweMe.Api.Tests.Description;

public class ExceptionProblemDetailsMatcherTests
{
    private readonly Mock<IProblemDetailsService> _mockProblemDetailsService;
    private readonly ExceptionProblemDetailsMatcher _matcher;
    private readonly HttpContext _httpContext;

    public ExceptionProblemDetailsMatcherTests()
    {
        _mockProblemDetailsService = new Mock<IProblemDetailsService>();
        _matcher = new ExceptionProblemDetailsMatcher(_mockProblemDetailsService.Object);
        _httpContext = new DefaultHttpContext();
        
        // Setup mock to return true by default
        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);
    }
    
    [Fact]
    public async Task TryHandleAsync_WithValidationException_ShouldReturnCorrectProblemDetails()
    {
        // Arrange
        var errors = new Dictionary<string, string> 
        {
            { "Field1", "Error1" },
            { "Field2", "Error2" }
        };
        var exception = new ValidationException("Validation failed", errors);
        ProblemDetailsContext? capturedContext = null;

        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => capturedContext = context)
            .ReturnsAsync(true);

        // Act
        var result = await _matcher.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.ShouldBeTrue();
        capturedContext.ShouldNotBeNull();
        capturedContext.Exception.ShouldBe(exception);
        capturedContext.HttpContext.ShouldBe(_httpContext);
        
        var problemDetails = capturedContext.ProblemDetails;
        problemDetails.ShouldNotBeNull();
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
        problemDetails.Title.ShouldBe("Validation error");
        problemDetails.Detail.ShouldBe("Validation failed");
        
        problemDetails.Extensions.ShouldContainKey("errors");
        var errorsDict = problemDetails.Extensions["errors"] as Dictionary<string, string[]>;
        errorsDict.ShouldNotBeNull();
        errorsDict.ShouldContainKey("Field1");
        errorsDict["Field1"].ShouldContain("Error1");
        errorsDict.ShouldContainKey("Field2");
        errorsDict["Field2"].ShouldContain("Error2");
    }
    
    [Fact]
    public async Task TryHandleAsync_WithNotFoundException_ShouldReturnCorrectProblemDetails()
    {
        // Arrange
        var exception = new NotFoundException("Resource not found");
        ProblemDetailsContext? capturedContext = null;

        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => capturedContext = context)
            .ReturnsAsync(true);

        // Act
        var result = await _matcher.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.ShouldBeTrue();
        capturedContext.ShouldNotBeNull();
        
        var problemDetails = capturedContext.ProblemDetails;
        problemDetails.ShouldNotBeNull();
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
        problemDetails.Title.ShouldBe("Resource not found");
        problemDetails.Detail.ShouldBe("Resource not found");
    }
    
    [Fact]
    public async Task TryHandleAsync_WithForbiddenException_ShouldReturnCorrectProblemDetails()
    {
        // Arrange
        var exception = new ForbiddenException("Access denied");
        ProblemDetailsContext? capturedContext = null;

        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => capturedContext = context)
            .ReturnsAsync(true);

        // Act
        var result = await _matcher.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.ShouldBeTrue();
        capturedContext.ShouldNotBeNull();
        
        var problemDetails = capturedContext.ProblemDetails;
        problemDetails.ShouldNotBeNull();
        problemDetails.Status.ShouldBe(StatusCodes.Status403Forbidden);
        problemDetails.Title.ShouldBe("Access forbidden");
        problemDetails.Detail.ShouldBe("Access denied");
    }
    
    [Fact]
    public async Task TryHandleAsync_WithBadHttpRequestException_ShouldReturnCorrectProblemDetails()
    {
        // Arrange
        var exception = new BadHttpRequestException("Invalid request");
        ProblemDetailsContext? capturedContext = null;

        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => capturedContext = context)
            .ReturnsAsync(true);

        // Act
        var result = await _matcher.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.ShouldBeTrue();
        capturedContext.ShouldNotBeNull();
        
        var problemDetails = capturedContext.ProblemDetails;
        problemDetails.ShouldNotBeNull();
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
        problemDetails.Title.ShouldBe("Bad request");
        problemDetails.Detail.ShouldBe("Invalid request");
    }
    
    [Fact]
    public async Task TryHandleAsync_WithGenericException_ShouldReturnDefaultProblemDetails()
    {
        // Arrange
        var exception = new InvalidOperationException("Something went wrong");
        ProblemDetailsContext? capturedContext = null;

        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => capturedContext = context)
            .ReturnsAsync(true);

        // Act
        var result = await _matcher.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.ShouldBeTrue();
        capturedContext.ShouldNotBeNull();
        
        var problemDetails = capturedContext.ProblemDetails;
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("An unexpected error occurred");
        problemDetails.Detail.ShouldBe("Something went wrong");
    }
    
    [Fact]
    public async Task TryHandleAsync_WhenProblemDetailsServiceReturnsFalse_ShouldReturnFalse()
    {
        // Arrange
        var exception = new Exception("Test exception");
        
        _mockProblemDetailsService
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(false);

        // Act
        var result = await _matcher.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.ShouldBeFalse();
    }
}