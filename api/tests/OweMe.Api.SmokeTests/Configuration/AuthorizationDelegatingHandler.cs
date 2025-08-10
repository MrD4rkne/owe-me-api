using System.Net.Http.Headers;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OweMe.Api.SmokeTests;

internal sealed class AuthorizationDelegatingHandler(
    IOptions<UserSettings> userSettings,
    IServiceProvider serviceProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string accessToken = await GetAccessTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var httpClient = serviceProvider.GetRequiredService<TokenClient>();
        var token = await httpClient.RequestPasswordTokenAsync(
            userSettings.Value.Username,
            userSettings.Value.Password,
            userSettings.Value.Scope,
            cancellationToken: cancellationToken);

        if (token.IsError)
        {
            throw new InvalidOperationException($"Failed to obtain token: {token.Error}");
        }

        return token.AccessToken;
    }
}