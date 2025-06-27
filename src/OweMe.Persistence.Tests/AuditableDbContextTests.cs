using Microsoft.EntityFrameworkCore;
using Moq;
using OweMe.Application;
using OweMe.Domain.Common;
using OweMe.Domain.Users;
using OweMe.Persistence.Common;
using OweMe.Tests.Common;
using Shouldly;

namespace OweMe.Persistence.Tests;

internal sealed class TestEntity : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;
}

internal sealed class TestDbContext(
    DbContextOptions<TestDbContext> options,
    TimeProvider timeProvider,
    IUserContext userContext)
    : AuditableDbContext(options, timeProvider, userContext)
{
    public DbSet<TestEntity> _entities => Set<TestEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TestEntity>()
            .HasKey(e => e.Id);
    }
}

public class AuditableDbContextTests : PostgresTestBase, IAsyncLifetime
{
    private readonly UserId _createdBy = UserId.New();

    private readonly DateTime _now = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly DateTime _then = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly Mock<TimeProvider> _timeProviderMock;
    private readonly UserId _updatedBy = UserId.New();
    private readonly Mock<IUserContext> _userContextMock;
    private DbContextOptions<TestDbContext> _options = null!;

    public AuditableDbContextTests()
    {
        _timeProviderMock = new Mock<TimeProvider>();
        _userContextMock = new Mock<IUserContext>();
    }

    public async Task InitializeAsync()
    {
        await SetupAsync();

        _options = new DbContextOptionsBuilder<TestDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var ctx = new TestDbContext(_options, _timeProviderMock.Object, _userContextMock.Object);
        await ctx.Database.EnsureCreatedAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return base.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task SaveChangesAsync_SetsCreatedFields_OnAdd()
    {
        // Arrange
        await using var sut = new TestDbContext(_options, _timeProviderMock.Object, _userContextMock.Object);
        var entity = new TestEntity();
        sut._entities.Add(entity);

        _timeProviderMock.Setup(tp => tp.GetUtcNow()).Returns(_now);
        _userContextMock.Setup(uc => uc.Id).Returns(_createdBy);

        // Act
        int changed = await sut.SaveChangesAsync();

        // Assert
        changed.ShouldBe(1);
        entity.CreatedAt.ShouldBe(_now, "Created fields should be set on creation");
        entity.CreatedBy.ShouldBe(_createdBy, "Created fields should be set on creation");
        entity.UpdatedAt.ShouldBeNull("Updated fields should be null on creation");
        entity.UpdatedBy.ShouldBeNull("Updated fields should be null on creation");

        _timeProviderMock.Verify(tp => tp.GetUtcNow(), Times.Once, "GetUtcNow should be called once");
        _userContextMock.Verify(uc => uc.Id, Times.Once, "UserContext Id should be accessed once");
    }

    [Fact]
    public async Task SaveChangesAsync_SetsUpdatedFields_OnModify()
    {
        // Arrange
        _timeProviderMock.Setup(tp => tp.GetUtcNow()).Returns(_now);
        await using var sut = new TestDbContext(_options, _timeProviderMock.Object, _userContextMock.Object);

        _timeProviderMock.Setup(tp => tp.GetUtcNow()).Returns(_now);
        _userContextMock.Setup(uc => uc.Id).Returns(_createdBy);

        var entity = new TestEntity { CreatedAt = _now, CreatedBy = _userContextMock.Object.Id };
        sut._entities.Add(entity);
        await sut.SaveChangesAsync();

        _timeProviderMock.Setup(tp => tp.GetUtcNow()).Returns(_then);
        _userContextMock.Setup(uc => uc.Id).Returns(_updatedBy);

        _timeProviderMock.Invocations.Clear();
        _userContextMock.Invocations.Clear();

        // Act
        entity.Name = "Updated Name"; // Simulate change
        int changes = await sut.SaveChangesAsync();

        // Assert
        changes.ShouldBe(1);

        entity.UpdatedAt.ShouldBe(_then, "UpdatedAt should be set to current time on update");
        entity.UpdatedBy.ShouldBe(_updatedBy, "UpdatedBy should be set to current user on update");

        entity.CreatedAt.ShouldBe(_now, "CreatedAt should not change on update");
        entity.CreatedBy.ShouldBe(_createdBy, "CreatedBy should not change on update");

        _timeProviderMock.Verify(tp => tp.GetUtcNow(), Times.Exactly(1), "GetUtcNow should be called once for update");
        _userContextMock.Verify(uc => uc.Id, Times.Exactly(1), "UserContext Id should be accessed once for update");
    }

    [Fact]
    public void Constructor_Throws_OnNullTimeProvider()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TestDbContext(_options, null!, _userContextMock.Object));
    }

    [Fact]
    public void Constructor_Throws_OnNullUserContext()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TestDbContext(_options, _timeProviderMock.Object, null!));
    }
}