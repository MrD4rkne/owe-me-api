using OweMe.Domain.Users;

namespace OweMe.Application.Common;

public record AuditableEntityDTO
{
    public DateTimeOffset CreatedAt { get; init; }
    
    public DateTimeOffset? UpdatedAt { get; init; }
    
    public Guid CreatedBy { get; init; }
    
    public Guid? UpdatedBy { get; init; }
}