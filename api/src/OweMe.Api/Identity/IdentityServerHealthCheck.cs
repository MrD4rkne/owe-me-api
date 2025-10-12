using Duende.IdentityModel.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OweMe.Api.Identity.Configuration;

namespace OweMe.Api.Identity;

public sealed class IdentityServerHealthCheck(
    IOptions<IdentityServerOptions> identityServerOptions,
    IHttpClientFactory httpClientFactory
    ) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        if (identityServerOptions?.Value == null || string.IsNullOrWhiteSpace(identityServerOptions.Value.Authority))
        {
            return HealthCheckResult.Unhealthy("IdentityServer authority is not configured.");
        }
        
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(identityServerOptions.Value.Authority);

        var discoveryResponse = await httpClient.GetDiscoveryDocumentAsync(cancellationToken: cancellationToken);
        if (discoveryResponse.IsError)
        {
            return HealthCheckResult.Unhealthy("Failed to retrieve discovery document.", discoveryResponse.Exception);
        }
        
        return HealthCheckResult.Healthy("IdentityServer is healthy.");
    }
}