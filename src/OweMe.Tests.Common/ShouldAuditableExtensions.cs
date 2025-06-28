using OweMe.Domain.Common;
using OweMe.Domain.Users;
using Shouldly;

namespace OweMe.Application.UnitTests.Common;

public static class ShouldAuditableExtensions
{
    public static AuditableEntity ShouldBeCreatedBy(this AuditableEntity entity, UserId userId)
    {
        entity.CreatedBy.ShouldBe(userId);
        return entity;
    }

    public static AuditableEntity ShouldBeCreatedAt(this AuditableEntity entity, DateTimeOffset offset)
    {
        entity.CreatedAt.ShouldBe(offset);
        return entity;
    }

    public static AuditableEntity ShouldBeUpdatedBy(this AuditableEntity entity, UserId? userId)
    {
        entity.UpdatedBy.ShouldBe(userId);
        return entity;
    }

    public static AuditableEntity ShouldBeUpdatedAt(this AuditableEntity entity, DateTimeOffset? offset)
    {
        entity.UpdatedAt.ShouldBe(offset);
        return entity;
    }

    public static AuditableEntity ShouldBeCreated(this AuditableEntity entity, UserId userId, DateTimeOffset offset)
    {
        entity.ShouldBeCreatedBy(userId);
        entity.ShouldBeCreatedAt(offset);
        entity.ShouldBeUpdatedBy(null);
        entity.ShouldBeUpdatedAt(null);
        return entity;
    }

    public static AuditableEntity ShouldBeUpdated(this AuditableEntity entity, UserId userId, DateTimeOffset offset)
    {
        entity.ShouldBeUpdatedBy(userId);
        entity.ShouldBeUpdatedAt(offset);
        return entity;
    }

    public static AuditableEntity ShouldBeNeverUpdated(this AuditableEntity entity)
    {
        entity.UpdatedBy.ShouldBeNull();
        entity.UpdatedAt.ShouldBeNull();
        return entity;
    }
}