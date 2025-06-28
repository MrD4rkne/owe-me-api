using Microsoft.EntityFrameworkCore;

namespace OweMe.Domain.Ledgers;

public interface ILedgerContext
{
    DbSet<Ledger> Ledgers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}