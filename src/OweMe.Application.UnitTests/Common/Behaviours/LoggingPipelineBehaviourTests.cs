using OweMe.Application.Common.Behaviours;

namespace OweMe.Application.UnitTests.Common.Behaviours;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

public class LoggingPipelineBehaviourTests
{
    [Test]
    public async Task Handle_Should_Log_Information_On_Success()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var behaviour = new LoggingPipelineBehaviour<TestRequest, string>(loggerMock.Object);
        var request = new TestRequest { Value = "test" };
        const string response = "response";

        // Act
        string result = await behaviour.Handle(request, NextWithSimpleResponse, CancellationToken.None);

        // Assert
        result.ShouldBe(response);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => 
                    v.ToString().Contains("Handling TestRequest")
                    && v.ToString().Contains($"with request: {request}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Handled TestRequest successfully")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
        return;
        
        Task<string> NextWithSimpleResponse(CancellationToken cancellationToken) => Task.FromResult(response);
    }

    [Test]
    public async Task Handle_Should_Log_Error_On_Exception()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var behaviour = new LoggingPipelineBehaviour<TestRequest, string>(loggerMock.Object);
        var request = new TestRequest { Value = "test" };
        var exception = new Exception("fail");
    
        // Act & Assert
        var ex = await Should.ThrowAsync<Exception>(() => behaviour.Handle(request, NextThrowingException, CancellationToken.None));
        ex.ShouldBe(exception);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error handling TestRequest")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
        return;
        
        Task<string> NextThrowingException(CancellationToken cancellationToken) => throw exception;
    }

    private class TestRequest : IRequest<string>
    {
        public string Value { get; set; }
        
        public override string ToString() => $"Value: {Value}";
    }
}