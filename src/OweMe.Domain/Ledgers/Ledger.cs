using OweMe.Domain.Common;

namespace OweMe.Domain.Ledgers;

public class Ledger : AuditableEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
}