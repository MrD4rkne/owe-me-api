using Microsoft.EntityFrameworkCore;
using OweMe.Application;
using OweMe.Domain.Ledgers;
using OweMe.Persistence.Common;

namespace OweMe.Persistence.Ledgers;

public class LedgerDbContext : AuditableDbContext, ILedgerContext
{
    public LedgerDbContext(DbContextOptions<LedgerDbContext> options, TimeProvider timeProvider,
        IUserContext userContext) : base(options, timeProvider, userContext)
    {
    }

    public DbSet<Participant> Participants { get; set; }
    public DbSet<Ledger> Ledgers { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ledger>()
            .HasKey(l => l.Id);

        modelBuilder.Entity<Participant>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Ledger>()
            .Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(LedgerConstants.MaxNameLength);
        modelBuilder.Entity<Ledger>()
            .Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(LedgerConstants.MaxDescriptionLength);

        modelBuilder.Entity<Participant>()
            .Property(p => p.Nickname)
            .IsRequired()
            .HasMaxLength(LedgerConstants.Participant.MaxNicknameLength);

        base.OnModelCreating(modelBuilder);
    }
}