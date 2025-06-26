using Microsoft.EntityFrameworkCore;

namespace OweMe.Domain.Ledgers;

public interface ILedgerContext
{
    DbSet<Ledger> Ledgers { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}