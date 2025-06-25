using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using OweMe.Application.Common.Behaviours;
using NUnit.Framework;

namespace OweMe.Application.UnitTests.Common.Behaviours;

[TestFixture]
public class ValidationBehaviourTests
{
    [Test]
    public void Constructor_Should_Throw_When_LoggerIsNull()
    {
        // Arrange
        ILogger<ValidationBehaviour<string, string>> logger = null;
        var validators = new List<IValidator<string>>();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new ValidationBehaviour<string, string>(logger, validators));
        Assert.That(ex.ParamName, Is.EqualTo("logger"));
    }

    [Test]
    public void Constructor_Should_Throw_When_ValidatorsIsNull()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ValidationBehaviour<string, string>>>();
        IEnumerable<IValidator<string>> validators = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new ValidationBehaviour<string, string>(logger, validators));
        Assert.That(ex.ParamName, Is.EqualTo("validators"));
    }
    
    [Test]
    public async Task Handle_Should_CallAllValidators_When_ValidatorsArePresent()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ValidationBehaviour<string, string>>>();
        const int n = 5;
        var validatorMocks = Enumerable.Range(0, n)
            .Select(_ => new Mock<IValidator<string>>())
            .ToList();
        foreach (var mock in validatorMocks)
        {
            mock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }
        var validators = validatorMocks.Select(m => m.Object).ToList();
        var sut = new ValidationBehaviour<string, string>(logger, validators);
        var next = new RequestHandlerDelegate<string>(_ => Task.FromResult("response"));

        // Act
        string result = await sut.Handle("request", next, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo("response"));
        foreach (var mock in validatorMocks)
        {
            mock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<string>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
    
    [Test]
    public async Task Handle_Should_CallAllValidators_Concurrently()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ValidationBehaviour<string, string>>>();
        var started = new List<DateTime>();
        var tcs = new TaskCompletionSource();
        var validators = Enumerable.Range(0, 5).Select(_ =>
        {
            var mock = new Mock<IValidator<string>>();
            mock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<string>>(), It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    lock (started) { started.Add(DateTime.UtcNow); }
                    // Wait until all validators have started
                    if (started.Count < 5)
                        await tcs.Task;
                    else
                        tcs.SetResult();
                    return new ValidationResult();
                });
            return mock.Object;
        }).ToList();
        
        var sut = new ValidationBehaviour<string, string>(logger, validators);
        var next = new RequestHandlerDelegate<string>(_ => Task.FromResult("ok"));

        // Act
        string result = await sut.Handle("req", next, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo("ok"));
        // All validators should have started within a very short time window (e.g., 100ms)
        var min = started.Min();
        var max = started.Max();
        Assert.That((max - min).TotalMilliseconds, Is.LessThanOrEqualTo(100), "Validators were not called concurrently");
    }
}