using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OweMe.Application.Ledgers;
using OweMe.Persistence.Configuration;
using OweMe.Persistence.Ledgers;

namespace OweMe.Persistence;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddPersistence(this IHostApplicationBuilder builder)
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
        builder.Services.AddHostedService<MigrationHostedService>();

        return builder;
    }
}