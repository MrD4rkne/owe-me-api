using Microsoft.EntityFrameworkCore;
using OweMe.Domain.Ledgers;

namespace OweMe.Application.Ledgers;

public interface ILedgerContext
{
    DbSet<Ledger> Ledgers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}