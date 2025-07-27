using System.ComponentModel;

namespace OweMe.Api.Endpoints.Common;

public abstract record AuditableEntityModel
{
    [Description("Date and time when the entity was created. UTC format.")]
    [ReadOnly(true)]
    public required DateTimeOffset CreatedAt { get; init; }

    [Description("Date and time when the entity was last updated. UTC format.")]
    [ReadOnly(true)]
    public DateTimeOffset? UpdatedAt { get; init; }

    [Description("Unique identifier of the user who created the entity.")]
    [ReadOnly(true)]
    public required Guid CreatedBy { get; init; }

    [Description("Unique identifier of the user who last updated the entity.")]
    [ReadOnly(true)]
    public Guid? UpdatedBy { get; init; }
}