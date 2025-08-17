using Microsoft.EntityFrameworkCore;
using OweMe.Domain.Common.Exceptions;

namespace OweMe.Application.Ledgers.Queries.Get;

public static class GetLedgerQueryHandler
{
    public static async Task<GetLedgerResult> HandleAsync(
        GetLedgerQuery query,
        ILedgerContext context,
        IUserContext userContext,
        CancellationToken cancellationToken)
    {
        var ledger = await context.Ledgers.FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);
        if (ledger is null || !ledger.CanUserAccess(userContext.Id))
        {
            throw new NotFoundException($"Ledger with id {query.Id} not found.");
        }

        return GetLedgerResult.FromDomain(ledger);
    }
}