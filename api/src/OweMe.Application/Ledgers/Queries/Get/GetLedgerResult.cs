using OweMe.Application.Common;
using OweMe.Domain.Ledgers;

namespace OweMe.Application.Ledgers.Queries.Get;

public sealed record GetLedgerResult(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    Guid CreatedBy,
    DateTimeOffset? UpdatedAt,
    Guid? UpdatedBy) : AuditableEntityDto(CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
{
    public static GetLedgerResult FromDomain(Ledger ledger)
    {
        return new GetLedgerResult(ledger.Id, ledger.Name, ledger.Description,
            ledger.CreatedAt, ledger.CreatedBy.Id, ledger.UpdatedAt, ledger.UpdatedBy?.Id);
    }
}