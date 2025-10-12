using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using OweMe.Api.Identity;
using OweMe.Persistence.Ledgers;

namespace OweMe.Api.HealthChecks;

internal static class HealthChecksRegistration
{
    private static class Tags
    {
        public const string Database = nameof(Database);
        public const string IdentityServer = nameof(IdentityServer);
    }
    
    public static IHealthChecksBuilder AddOweMeHealthChecks(
        this IHealthChecksBuilder builder)
    {
        builder
            .AddDbContextCheck<LedgerDbContext>(
                "db_connection_check",
                failureStatus: HealthStatus.Unhealthy,
                tags: [Tags.Database],
                customTestQuery: async (db, token) => await db.Ledgers.CountAsync(token) >= 0)
            .AddCheck<IdentityServerHealthCheck>(
                "identity_server_discovery",
                HealthStatus.Degraded,
                [Tags.IdentityServer]);
        
        return builder;
    }
    
    public static WebApplication UseOweMeHealthChecks(this WebApplication app)
    {
        var healthCheckOptions = new HealthCheckOptions
        {
            ResponseWriter = WriteAction
        };
        app.MapHealthChecks("/health", healthCheckOptions);
        return app;
    }
    
    private static Task WriteAction(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json";
        var jsonDocument = JsonConvert.SerializeObject(healthReport.Entries);
        return context.Response.WriteAsync(jsonDocument);
    }
}