﻿namespace OweMe.Application.Common;

public record AuditableEntityDto
{
    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }

    public required Guid CreatedBy { get; init; }

    public Guid? UpdatedBy { get; init; }
}