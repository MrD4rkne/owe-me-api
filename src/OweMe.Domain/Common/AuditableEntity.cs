using OweMe.Domain.Users;

namespace OweMe.Domain.Common;

public class AuditableEntity
{
    public UserId CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public UserId? UpdatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}