using OweMe.Application.Common.Results;
using OweMe.Domain.Ledgers;

namespace OweMe.Application.Ledgers.Commands.Create;

public class CreateLedgerCommandHandler(ILedgerContext context) : IResultRequestHandler<CreateLedgerCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateLedgerCommand request, CancellationToken cancellationToken)
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