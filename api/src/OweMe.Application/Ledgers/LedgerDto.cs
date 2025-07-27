using OweMe.Application.Common;
using OweMe.Domain.Ledgers;

namespace OweMe.Application.Ledgers;

public record LedgerDto : AuditableEntityDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public static LedgerDto FromDomain(Ledger ledger)
    {
        return new LedgerDto
        {
            Id = ledger.Id,
            Name = ledger.Name,
            Description = ledger.Description,
            CreatedAt = ledger.CreatedAt,
            UpdatedAt = ledger.UpdatedAt,
            CreatedBy = ledger.CreatedBy,
            UpdatedBy = ledger.UpdatedBy
        };
    }
}