using OweMe.Domain.Ledgers;

namespace OweMe.Application.Ledgers.Commands.Create;

public static class CreateLedgerCommandHandler
{
    public static async Task<LedgerCreated> Handle(CreateLedgerCommand message, ILedgerContext context,
        CancellationToken cancellationToken = default)
    {
        var ledger = new Ledger
        {
            Name = message.Name,
            Description = message.Description
        };

        context.Ledgers.Add(ledger);
        _ = await context.SaveChangesAsync(cancellationToken);
        return new LedgerCreated(
            ledger.Id
        );
    }

    public sealed record LedgerCreated(Guid Id);
}