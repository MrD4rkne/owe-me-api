using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OweMe.Domain.Ledgers;
using OweMe.Persistence.Configuration;
using OweMe.Persistence.Ledgers;

namespace OweMe.Persistence;

public static class DependencyInjection
{
    public static Task AddPersistence(this IHostApplicationBuilder builder, bool shouldNotRunMigrations = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Register the DatabaseOptions with configuration
        builder.Services.AddOptions<DatabaseOptions>()
            .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateOnStart();

        builder.Services.AddDbContext<LedgerDbContext>((serviceProvider, options) =>
        {
            var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(dbOptions.ConnectionString);
            options.EnableSensitiveDataLogging();
        });

        builder.Services.AddScoped<ILedgerContext, LedgerDbContext>();

        return shouldNotRunMigrations
            ? Task.CompletedTask
            : builder.TryRunMigrations(builder.Services.BuildServiceProvider());
    }

    private static async Task TryRunMigrations(this IHostApplicationBuilder builder, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbOptions = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        if (!dbOptions.RunMigrations)
        {
            return;
        }

        var context = scope.ServiceProvider.GetRequiredService<LedgerDbContext>();
        await context.Database.MigrateAsync();
    }
}