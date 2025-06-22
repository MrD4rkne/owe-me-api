using OweMe.Domain.Common;
using OweMe.Domain.Users;

namespace OweMe.Domain.Ledgers;

public class Ledger : AuditableEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool CanUserAccess(UserId userId)
    {
        return CreatedBy == userId;
    }
}