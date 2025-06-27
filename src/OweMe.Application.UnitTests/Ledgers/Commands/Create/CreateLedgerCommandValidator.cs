using OweMe.Application.Ledgers.Commands.Create;
using Shouldly;

namespace OweMe.Application.UnitTests.Ledgers.Commands.Create;

public class CreateLedgerCommandValidator
{
    private const int MaxNameLength = 100;
    private const int MaxDescriptionLength = 500;

    private static readonly object[] validNames =
    [
        new string('A', MaxNameLength),
        "Abcefghijklmnopqrstuvwxyz",
        "test"
    ];

    private static readonly object?[] invalidNames =
    [
        new string('A', MaxNameLength + 1), // Exceeding max length
        "",
        string.Empty,
        null
    ];

    private static readonly object?[] validDescriptions =
    [
        new string('A', MaxDescriptionLength),
        "Abcefghijklmnopqrstuvwxyz",
        "",
        string.Empty,
        null
    ];

    private static readonly object[] invalidDescriptions =
    [
        new string('A', MaxDescriptionLength + 1), // Exceeding max length
        new string('A', MaxDescriptionLength + 2),
        new string('A', MaxDescriptionLength + 10)
    ];

    private static object[] ValidNameDescriptionCombinations()
    {
        return validNames.SelectMany(
            name => validDescriptions,
            (name, description) => new[] { name, description }
        ).ToArray<object>();
    }

    [Test]
    [TestCaseSource(nameof(ValidNameDescriptionCombinations))]
    public void Should_Validate_CreateLedgerCommand(string name, string description)
    {
        // Arrange
        var validator = new Application.Ledgers.Commands.Create.CreateLedgerCommandValidator();
        var command = new CreateLedgerCommand
        {
            Name = name,
            Description = description
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue("Validation should pass for valid command.");
    }

    private static object[] InvalidNameValidDescriptionCombinations()
    {
        return invalidNames.SelectMany(
            name => validDescriptions,
            (name, description) => new[] { name, description }
        ).ToArray<object>();
    }

    [Test]
    [TestCaseSource(nameof(InvalidNameValidDescriptionCombinations))]
    public void Should_Invalidate_CreateLedgerCommand_When_Name_Is_Invalid(string name, string description)
    {
        // Arrange
        var validator = new Application.Ledgers.Commands.Create.CreateLedgerCommandValidator();
        var command = new CreateLedgerCommand
        {
            Name = name,
            Description = description
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse("Validation should fail when name is empty.");
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(command.Name)), Is.True);
    }

    private static object[] ValidNameInvalidDescriptionCombinations()
    {
        return validNames.SelectMany(
            name => invalidDescriptions,
            (name, description) => new[] { name, description }
        ).ToArray<object>();
    }

    [Test]
    [TestCaseSource(nameof(ValidNameInvalidDescriptionCombinations))]
    public void Should_Invalidate_CreateLedgerCommand_When_Description_Exceeds_Max_Length(string name,
        string description)
    {
        // Arrange
        var validator = new Application.Ledgers.Commands.Create.CreateLedgerCommandValidator();
        var command = new CreateLedgerCommand
        {
            Name = "Test Ledger",
            Description = new string('A', MaxDescriptionLength + 1) // Exceeding max length
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse("Validation should fail when description exceeds maximum length.");
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(command.Description)), Is.True);
    }
}