﻿using Microsoft.EntityFrameworkCore;
using OweMe.Application;
using OweMe.Domain.Common;

namespace OweMe.Persistence.Common;

public class AuditableDbContext : DbContext
{
    private readonly TimeProvider _timeProvider;
    private readonly IUserContext _userContext;

    protected AuditableDbContext(DbContextOptions options, TimeProvider timeProvider, IUserContext userContext) :
        base(options)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var clrType in modelBuilder.Model.GetEntityTypes()
                     .Select(entityType => entityType.ClrType)
                     .Where(clrType => typeof(AuditableEntity).IsAssignableFrom(clrType) &&
                                       clrType != typeof(AuditableEntity)))
        {
            modelBuilder.Entity(clrType)
                .Property(nameof(AuditableEntity.CreatedBy))
                .HasConversion(new UserIdConverter())
                .IsRequired();

            modelBuilder.Entity(clrType)
                .Property(nameof(AuditableEntity.CreatedAt))
                .IsRequired();

            modelBuilder.Entity(clrType)
                .Property(nameof(AuditableEntity.UpdatedBy))
                .HasConversion(new UserIdConverter())
                .IsRequired(false);

            modelBuilder.Entity(clrType)
                .Property(nameof(AuditableEntity.UpdatedAt))
                .IsRequired(false);
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditableValues();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditableValues()
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = _timeProvider.GetUtcNow();
                    entry.Entity.CreatedBy = _userContext.Id;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = _timeProvider.GetUtcNow();
                    entry.Entity.UpdatedBy = _userContext.Id;
                    break;
            }
        }
    }
}