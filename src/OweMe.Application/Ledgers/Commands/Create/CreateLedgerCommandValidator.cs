using FluentValidation;
using OweMe.Domain.Ledgers;

namespace OweMe.Application.Ledgers.Commands.Create;

public class CreateLedgerCommandValidator : AbstractValidator<CreateLedgerCommand>
{
    public CreateLedgerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Ledger name is required.")
            .MaximumLength(LedgerConstants.MaxNameLength)
            .WithMessage($"Ledger name must not exceed {LedgerConstants.MaxNameLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(LedgerConstants.MaxDescriptionLength)
            .WithMessage($"Ledger description must not exceed {LedgerConstants.MaxDescriptionLength} characters.");
    }
}