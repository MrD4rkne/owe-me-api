using Shouldly;

namespace OweMe.Application.UnitTests.Ledgers.Commands.Create;

public class CreateLedgerCommandValidator
{
    private const int MaxNameLength = 100;
    private const int MaxDescriptionLength = 500;
    
    [Test]
    public void Should_Validate_CreateLedgerCommand()
    {
        // Arrange
        var validator = new OweMe.Application.Ledgers.Commands.Create.CreateLedgerCommandValidator();
        var command = new OweMe.Application.Ledgers.Commands.Create.CreateLedgerCommand
        {
            Name = "Test Ledger",
            Description = "This is a test ledger description."
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue("Validation should pass for valid command.");
    }

    static readonly object[] validButEmptyDescriptions =
    [
        "",
        string.Empty,
        null
    ];
    
    [Test]
    [TestCaseSource(nameof(validButEmptyDescriptions))]
    public void Should_Validate_CreateLedgerCommand_WhenDescriptionEmpty(string description)
    {
        // Arrange
        var validator = new OweMe.Application.Ledgers.Commands.Create.CreateLedgerCommandValidator();
        var command = new OweMe.Application.Ledgers.Commands.Create.CreateLedgerCommand
        {
            Name = "Test Ledger",
            Description = description
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue("Validation should pass even if description is empty or null.");
    }
    
    [Test]
    public void Should_Invalidate_CreateLedgerCommand_When_Name_Is_Empty()
    {
        // Arrange
        var validator = new OweMe.Application.Ledgers.Commands.Create.CreateLedgerCommandValidator();
        var command = new OweMe.Application.Ledgers.Commands.Create.CreateLedgerCommand
        {
            Name = string.Empty,
            Description = "This is a test ledger description."
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse("Validation should fail when name is empty.");
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(command.Name)), Is.True);
    }
    
    [Test]
    public void Should_Invalidate_CreateLedgerCommand_When_Name_Exceeds_Max_Length()
    {
        // Arrange
        var validator = new OweMe.Application.Ledgers.Commands.Create.CreateLedgerCommandValidator();
        var command = new OweMe.Application.Ledgers.Commands.Create.CreateLedgerCommand
        {
            Name = new string('A', MaxNameLength + 1), // Exceeding max length
            Description = "This is a test ledger description."
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse("Validation should fail when name exceeds maximum length.");
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(command.Name)), Is.True);
    }
    
    [Test]
    public void Should_Invalidate_CreateLedgerCommand_When_Description_Exceeds_Max_Length()
    {
        // Arrange
        var validator = new OweMe.Application.Ledgers.Commands.Create.CreateLedgerCommandValidator();
        var command = new OweMe.Application.Ledgers.Commands.Create.CreateLedgerCommand
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