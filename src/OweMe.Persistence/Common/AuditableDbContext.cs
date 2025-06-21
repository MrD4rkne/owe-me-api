using Microsoft.EntityFrameworkCore;
using OweMe.Application;
using OweMe.Domain.Common;
using OweMe.Domain.Users;

namespace OweMe.Persistence.Common;

public class AuditableDbContext : DbContext
{
    private readonly TimeProvider _timeProvider;
    private readonly IUserContext _userContext;
    
    public AuditableDbContext(DbContextOptions options, TimeProvider timeProvider, IUserContext userContext) : base(options)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AuditableEntity>()
            .Property(a => a.CreatedBy)
            .IsRequired();
        
        modelBuilder.Entity<AuditableEntity>()
            .Property(a => a.CreatedAt)
            .IsRequired();
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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

        return base.SaveChangesAsync(cancellationToken);
    }
}