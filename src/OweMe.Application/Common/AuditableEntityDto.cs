namespace OweMe.Application.Common;

public record AuditableEntityDto
{
    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }

    public Guid CreatedBy { get; init; }

    public Guid? UpdatedBy { get; init; }
}