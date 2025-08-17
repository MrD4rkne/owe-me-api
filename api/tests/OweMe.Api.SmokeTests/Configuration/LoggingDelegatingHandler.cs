using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace OweMe.Api.SmokeTests;

public class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly ILogger? _logger = TestContext.Current.TestOutputHelper?.ToLogger<LoggingDelegatingHandler>();
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _logger?.LogInformation("Request: {Method} {Uri}", request.Method, request.RequestUri);
        if (request.Content != null)
        {
            string requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            _logger?.LogInformation("Request Body: {Body}", requestBody);
        }

        _logger?.LogInformation("Request Headers: {Headers}", FormatHeaders(request.Headers));

        var response = await base.SendAsync(request, cancellationToken);

        _logger?.LogInformation("Response: {StatusCode} {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
        
        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger?.LogInformation("Response Body: {Body}", responseBody);
        _logger?.LogInformation("Response Headers: {Headers}", FormatHeaders(response.Headers));

        return response;
    }

    private static string FormatHeaders(HttpHeaders headers)
    {
        return string.Join("; ", headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));
    }
}