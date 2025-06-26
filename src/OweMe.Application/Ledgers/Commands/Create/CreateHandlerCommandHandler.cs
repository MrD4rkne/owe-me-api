using MediatR;
using OweMe.Domain.Ledgers;

namespace OweMe.Application.Ledgers.Commands.Create;

public class CreateHandlerCommandHandler(ILedgerContext context) : IRequestHandler<CreateLedgerCommand, Guid>
{
    public async Task<Guid> Handle(CreateLedgerCommand request, CancellationToken cancellationToken)
    {
        var ledger = new Ledger
        {
            Name = request.Name,
            Description = request.Description
        };

        context.Ledgers.Add(ledger);
        _ = await context.SaveChangesAsync(cancellationToken);

        return ledger.Id;
    }
}