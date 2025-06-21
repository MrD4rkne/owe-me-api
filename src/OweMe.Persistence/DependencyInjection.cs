using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OweMe.Domain.Ledgers;
using OweMe.Persistence.Ledgers;

namespace OweMe.Persistence;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddPersistence(this IHostApplicationBuilder builder, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNullOrEmpty(connectionString);

        builder.Services.AddDbContext<LedgerDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging();
        });

        builder.Services.AddScoped<ILedgerContext, LedgerDbContext>();
    }
}