using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMe.Application.Common;
using OweMe.Domain.Ledgers;
using OweMe.Domain.Ledgers.Queries.Get;

namespace OweMe.Application.Ledgers.Commands.Create;

public class GetLedgerQueryHandler(ILedgerContext context, IUserContext userContext) : IRequestHandler<GetLedgerQuery, Result<LedgerDTO>>
{
    public async Task<Result<LedgerDTO>> Handle(GetLedgerQuery request, CancellationToken cancellationToken)
    {
        var ledger = await context.Ledgers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (ledger is null || !ledger.CanUserAccess(userContext.Id))
        {
            return Result<LedgerDTO>.Failure(LedgerErrors.LedgerNotFound);
        }

        return Result<LedgerDTO>.Success(LedgerDTO.FromDomain(ledger));
    }
}