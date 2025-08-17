namespace OweMe.Application.Common;

public record AuditableEntityDto(DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt, Guid CreatedBy, Guid? UpdatedBy);