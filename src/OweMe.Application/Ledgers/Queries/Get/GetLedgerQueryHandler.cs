using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMe.Application.Common;
using OweMe.Domain.Ledgers;
using OweMe.Domain.Ledgers.Queries.Get;

namespace OweMe.Application.Ledgers.Commands.Create;

public class GetLedgerQueryHandler(ILedgerContext context, IUserContext userContext)
    : IRequestHandler<GetLedgerQuery, Result<LedgerDto>>
{
    public async Task<Result<LedgerDto>> Handle(GetLedgerQuery request, CancellationToken cancellationToken)
    {
        var ledger = await context.Ledgers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (ledger is null || !ledger.CanUserAccess(userContext.Id))
        {
            return Result<LedgerDto>.Failure(LedgerErrors.Errors.LedgerNotFound);
        }

        return Result<LedgerDto>.Success(LedgerDto.FromDomain(ledger));
    }
}