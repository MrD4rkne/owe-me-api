using OweMe.Domain.Common;
using OweMe.Domain.Users;
using Shouldly;

namespace OweMe.Tests.Common.Tests;

public class ShouldAuditableExtensionsTests
{
    [Fact]
    public void ShouldBeCreatedBy_Asserts_CreatedBy()
    {
        var userId = new UserId(Guid.NewGuid());
        var entity = new TestAuditableEntity(userId, DateTimeOffset.UtcNow);

        entity.ShouldBeCreatedBy(userId);
    }

    [Fact]
    public void ShouldBeCreatedAt_Asserts_CreatedAt()
    {
        var now = DateTimeOffset.UtcNow;
        var entity = new TestAuditableEntity(new UserId(Guid.NewGuid()), now);

        entity.ShouldBeCreatedAt(now);
    }

    [Fact]
    public void ShouldBeUpdatedBy_Asserts_UpdatedBy()
    {
        var userId = new UserId(Guid.NewGuid());
        var entity = new TestAuditableEntity(new UserId(Guid.NewGuid()), DateTimeOffset.UtcNow, userId);

        entity.ShouldBeUpdatedBy(userId);
    }

    [Fact]
    public void ShouldBeUpdatedAt_Asserts_UpdatedAt()
    {
        var now = DateTimeOffset.UtcNow;
        var entity = new TestAuditableEntity(new UserId(Guid.NewGuid()), DateTimeOffset.UtcNow,
            new UserId(Guid.NewGuid()), now);

        entity.ShouldBeUpdatedAt(now);
    }

    [Fact]
    public void ShouldBeCreated_Asserts_AllCreatedFields()
    {
        var userId = new UserId(Guid.NewGuid());
        var now = DateTimeOffset.UtcNow;
        var entity = new TestAuditableEntity(userId, now);

        entity.ShouldBeCreated(userId, now);
    }

    [Fact]
    public void ShouldBeUpdated_Asserts_AllUpdatedFields()
    {
        var userId = new UserId(Guid.NewGuid());
        var now = DateTimeOffset.UtcNow;
        var entity = new TestAuditableEntity(new UserId(Guid.NewGuid()), DateTimeOffset.UtcNow, userId, now);

        entity.ShouldBeUpdated(userId, now);
    }

    [Fact]
    public void ShouldBeNeverUpdated_Asserts_UpdatedFieldsAreNull()
    {
        var entity = new TestAuditableEntity(new UserId(Guid.NewGuid()), DateTimeOffset.UtcNow);

        entity.ShouldBeNeverUpdated();
    }

    [Fact]
    public void ShouldBeCreatedBy_Fails_WithWrongUserId()
    {
        var correctUserId = new UserId(Guid.NewGuid());
        var wrongUserId = new UserId(Guid.NewGuid());
        var entity = new TestAuditableEntity(correctUserId, DateTimeOffset.UtcNow);

        Should.Throw<ShouldAssertException>(() => entity.ShouldBeCreatedBy(wrongUserId));
    }

    [Fact]
    public void ShouldBeCreatedAt_Fails_WithWrongDate()
    {
        var now = DateTimeOffset.UtcNow;
        var wrongDate = now.AddMinutes(1);
        var entity = new TestAuditableEntity(new UserId(Guid.NewGuid()), now);

        Should.Throw<ShouldAssertException>(() => entity.ShouldBeCreatedAt(wrongDate));
    }

    [Fact]
    public void ShouldBeUpdatedBy_Fails_WithWrongUserId()
    {
        var correctUserId = new UserId(Guid.NewGuid());
        var wrongUserId = new UserId(Guid.NewGuid());
        var entity = new TestAuditableEntity(new UserId(Guid.NewGuid()), DateTimeOffset.UtcNow, correctUserId);

        Should.Throw<ShouldAssertException>(() => entity.ShouldBeUpdatedBy(wrongUserId));
    }

    [Fact]
    public void ShouldBeUpdatedAt_Fails_WithWrongDate()
    {
        var now = DateTimeOffset.UtcNow;
        var wrongDate = now.AddMinutes(1);
        var entity = new TestAuditableEntity(new UserId(Guid.NewGuid()), DateTimeOffset.UtcNow,
            new UserId(Guid.NewGuid()), now);

        Should.Throw<ShouldAssertException>(() => entity.ShouldBeUpdatedAt(wrongDate));
    }

    [Fact]
    public void ShouldBeCreated_Fails_WithWrongData()
    {
        var userId = new UserId(Guid.NewGuid());
        var now = DateTimeOffset.UtcNow;
        var wrongUserId = new UserId(Guid.NewGuid());
        var wrongDate = now.AddMinutes(1);
        var entity = new TestAuditableEntity(userId, now);

        Should.Throw<ShouldAssertException>(() => entity.ShouldBeCreated(wrongUserId, now));
        Should.Throw<ShouldAssertException>(() => entity.ShouldBeCreated(userId, wrongDate));
    }

    [Fact]
    public void ShouldBeUpdated_Fails_WithWrongData()
    {
        var userId = new UserId(Guid.NewGuid());
        var now = DateTimeOffset.UtcNow;
        var wrongUserId = new UserId(Guid.NewGuid());
        var wrongDate = now.AddMinutes(1);
        var entity = new TestAuditableEntity(new UserId(Guid.NewGuid()), DateTimeOffset.UtcNow, userId, now);

        Should.Throw<ShouldAssertException>(() => entity.ShouldBeUpdated(wrongUserId, now));
        Should.Throw<ShouldAssertException>(() => entity.ShouldBeUpdated(userId, wrongDate));
    }

    [Fact]
    public void ShouldBeNeverUpdated_Fails_WhenUpdatedFieldsAreNotNull()
    {
        var userId = new UserId(Guid.NewGuid());
        var now = DateTimeOffset.UtcNow;
        var entity = new TestAuditableEntity(new UserId(Guid.NewGuid()), DateTimeOffset.UtcNow, userId, now);

        Should.Throw<ShouldAssertException>(() => entity.ShouldBeNeverUpdated());
    }

    private class TestAuditableEntity : AuditableEntity
    {
        public TestAuditableEntity(UserId createdBy, DateTimeOffset createdAt, UserId? updatedBy = null,
            DateTimeOffset? updatedAt = null)
        {
            CreatedBy = createdBy;
            CreatedAt = createdAt;
            UpdatedBy = updatedBy;
            UpdatedAt = updatedAt;
        }
    }
}