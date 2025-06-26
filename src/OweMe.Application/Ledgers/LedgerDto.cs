using OweMe.Application.Common;

namespace OweMe.Domain.Ledgers;

public record LedgerDto : AuditableEntityDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public Ledger FromDTO()
    {
        return new Ledger
        {
            Id = Id,
            Name = Name,
            Description = Description,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            CreatedBy = CreatedBy,
            UpdatedBy = UpdatedBy
        };
    }

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