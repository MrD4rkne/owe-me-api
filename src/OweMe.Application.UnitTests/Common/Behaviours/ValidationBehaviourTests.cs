using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using OweMe.Application.Common.Behaviours;
using Shouldly;

namespace OweMe.Application.UnitTests.Common.Behaviours;

using LocalValidationBehaviour = ValidationBehaviour<TestRequest, string>;

[TestFixture]
public class ValidationBehaviourTests
{
    
    [Test]
    public void Constructor_Should_Throw_When_LoggerIsNull()
    {
        // Arrange
        ILogger<LocalValidationBehaviour>? logger = null!;
        var validators = new List<IValidator<TestRequest>>();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new LocalValidationBehaviour(logger!, validators));
        Assert.That(ex.ParamName, Is.EqualTo("logger"));
    }

    [Test]
    public void Constructor_Should_Throw_When_ValidatorsIsNull()
    {
        // Arrange
        var logger = Mock.Of<ILogger>();
        IEnumerable<IValidator<TestRequest>>? validators = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new LocalValidationBehaviour(logger, validators!));
        Assert.That(ex.ParamName, Is.EqualTo("validators"));
    }

    [Test]
    public async Task Handle_Should_CallAllValidators_When_ValidatorsArePresent()
    {
        // Arrange
        var logger = Mock.Of<ILogger>();
        const int n = 5;
        var validatorMocks = Enumerable.Range(0, n)
            .Select(_ => new Mock<IValidator<TestRequest>>())
            .ToList();
        foreach (var mock in validatorMocks)
        {
            mock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }

        var validators = validatorMocks.Select(m => m.Object).ToList();
        var sut = new LocalValidationBehaviour(logger, validators);
        var next = new RequestHandlerDelegate<string>(_ => Task.FromResult("response"));

        // Act
        string result = await sut.Handle(new TestRequest { Value = "request" }, next, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo("response"));
        foreach (var mock in validatorMocks)
        {
            mock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    [Test]
    public void Handle_Should_CallAllValidators_Concurrently()
    {
        // Arrange
        const int numberOfValidators = 10;
        const int validateTimeMs = 100;

        var logger = Mock.Of<ILogger>();

        using var cts = new CancellationTokenSource();
        const int maxAllowedProcessTime = 2 * validateTimeMs;
        maxAllowedProcessTime.ShouldBeLessThan(numberOfValidators * validateTimeMs);
        cts.CancelAfter(TimeSpan.FromMilliseconds(3 * validateTimeMs));

        var validators = Enumerable.Range(0, numberOfValidators).Select(_ =>
        {
            var mock = new Mock<IValidator<TestRequest>>();
            mock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    await Task.Delay(validateTimeMs, cts.Token);

                    return new ValidationResult();
                });
            return mock.Object;
        }).ToList();

        var sut = new LocalValidationBehaviour(logger, validators);
        var next = new RequestHandlerDelegate<string>(_ => Task.FromResult("ok"));

        // Act
        Assert.DoesNotThrowAsync(async () =>
        {
            string result = await sut.Handle(new TestRequest { Value = "request" }, next, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo("ok"));
        });
    }
}