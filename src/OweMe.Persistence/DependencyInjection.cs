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
    public static Task AddPersistence(this IHostApplicationBuilder builder, string connectionString,
        bool shouldRunMigrations = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddDbContext<LedgerDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging();
        });

        builder.Services.AddScoped<ILedgerContext, LedgerDbContext>();

        return builder.TryRunMigrations(builder.Services.BuildServiceProvider(), shouldRunMigrations);
    }

    private static async Task TryRunMigrations(this IHostApplicationBuilder builder, IServiceProvider serviceProvider, bool shouldRunMigrations)
    {
        if (!shouldRunMigrations)
        {
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LedgerDbContext>();
        await context.Database.MigrateAsync();
    }
}