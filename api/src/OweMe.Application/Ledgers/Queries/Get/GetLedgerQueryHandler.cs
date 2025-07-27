using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMe.Domain.Common.Exceptions;

namespace OweMe.Application.Ledgers.Queries.Get;

public class GetLedgerQueryHandler(ILedgerContext context, IUserContext userContext)
    : IRequestHandler<GetLedgerQuery, LedgerDto>
{
    public async Task<LedgerDto> Handle(GetLedgerQuery request, CancellationToken cancellationToken)
    {
        var ledger = await context.Ledgers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (ledger is null || !ledger.CanUserAccess(userContext.Id))
        {
            throw new NotFoundException($"Ledger with id {request.Id} not found.");
        }

        return LedgerDto.FromDomain(ledger);
    }
}