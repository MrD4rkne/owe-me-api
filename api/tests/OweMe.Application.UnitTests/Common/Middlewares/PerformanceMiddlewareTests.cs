using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OweMe.Application.Common;
using OweMe.Application.Common.Middlewares;
using Shouldly;
using Wolverine;

namespace OweMe.Application.UnitTests.Common.Behaviours;

public sealed class PerformanceMiddlewareTests
{
    private readonly Mock<ILogger<PerformanceMiddleware>> _logger = new();

    private readonly IOptions<ApplicationOptions> _options = Options.Create(new ApplicationOptions
    {
        TooLongRequestThresholdMs = 500 // Set a threshold for testing
    });

    private readonly IMessageContext _context = new TestMessageContext();

    [Fact]
    public void On_BeforeRunTwice_Should_ThrowInvalidOperationException()
    {
        // Arrange
        var logger = new Mock<ILogger<PerformanceMiddleware>>();
        var options = Options.Create(new ApplicationOptions { TooLongRequestThresholdMs = 500 });
        var sut = new PerformanceMiddleware(logger.Object, options);

        // Act
        sut.Before(_context);

        // Assert
        Should.Throw<InvalidOperationException>(() => sut.Before(_context));
    }

    [Fact]
    public void On_FinallyRunWithoutBefore_Should_ThrowInvalidOperationException()
    {
        // Arrange
        var logger = new Mock<ILogger<PerformanceMiddleware>>();
        var options = Options.Create(new ApplicationOptions { TooLongRequestThresholdMs = 500 });
        var sut = new PerformanceMiddleware(logger.Object, options);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => sut.Finally(_context));
    }

    [Fact]
    public void On_ValidRun_Should_LogInformation()
    {
        // Arrange
        const string requestName = "TestRequest";
        _context.Envelope.MessageType = requestName;

        var sut = new PerformanceMiddleware(_logger.Object, _options);

        // Act
        sut.Before(_context);
        sut.Finally(_context);

        // Assert
        _logger.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Started processing {requestName}.")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>())
        );
        _logger.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Handled {requestName} in")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>())
        );

        var startLogMessage = _logger.Invocations
            .FirstOrDefault(i => i.Method.Name == "Log" && i.Arguments[0].Equals(LogLevel.Information))?
            .Arguments[2]?.ToString();
        startLogMessage.ShouldNotBeNull();
        startLogMessage.ShouldBe($"Started processing {requestName}.");

        var endLogMessage = _logger.Invocations
            .FirstOrDefault(i => i.Method.Name == "Log"
                                 && i.Arguments[0].Equals(LogLevel.Information)
                                 && i.Arguments[2].ToString().Contains("Handled")
            )?
            .Arguments[2]?.ToString();
        endLogMessage.ShouldNotBeNull();

        Regex regex = new(@$"Handled {requestName} in (\d+) ms", RegexOptions.Compiled);
        regex.IsMatch(endLogMessage).ShouldBeTrue("End log message should match the expected format.");
    }
}