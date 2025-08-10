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

        var response = await base.SendAsync(request, cancellationToken);

        _logger?.LogInformation("Response: {StatusCode} {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
        
        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger?.LogInformation("Response Body: {Body}", responseBody);

        return response;
    }
}