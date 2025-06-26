using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using OweMe.Application.Common.Behaviours;
using NUnit.Framework;
using Shouldly;

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
        const int numberOfValidators = 10;
        const int validateTimeMs = 100;
       
        var logger = Mock.Of<ILogger<ValidationBehaviour<string, string>>>();
        
        var cts = new CancellationTokenSource();
        const int maxAllowedProcessTime = 2 * validateTimeMs;
        maxAllowedProcessTime.ShouldBeLessThan(numberOfValidators * validateTimeMs);
        cts.CancelAfter(TimeSpan.FromMilliseconds(3*validateTimeMs));
        
        var validators = Enumerable.Range(0, numberOfValidators).Select(_ =>
        {
            var mock = new Mock<IValidator<string>>();
            mock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<string>>(), It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    await Task.Delay(validateTimeMs, cts.Token);

                    return new ValidationResult();
                });
            return mock.Object;
        }).ToList();
        
        var sut = new ValidationBehaviour<string, string>(logger, validators);
        var next = new RequestHandlerDelegate<string>(_ => Task.FromResult("ok"));

        // Act
        Assert.DoesNotThrowAsync(async() =>
        {
            string result = await sut.Handle("req", next, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo("ok"));
        });
    }
}