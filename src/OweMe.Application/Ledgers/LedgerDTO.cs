using OweMe.Application.Common;

namespace OweMe.Domain.Ledgers;

public record LedgerDTO : AuditableEntityDTO
{
    public Guid Id { get; init; }
    
    public string Name { get; init; }
    
    public string Description { get; init; }
    
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
    
    public static LedgerDTO FromDomain(Ledger ledger)
    {
        return new LedgerDTO
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