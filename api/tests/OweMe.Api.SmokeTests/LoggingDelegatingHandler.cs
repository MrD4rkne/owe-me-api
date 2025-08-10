using Microsoft.Extensions.Logging;

namespace OweMe.Api.SmokeTests;

public class LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Request: {Method} {Uri}", request.Method, request.RequestUri);
        if (request.Content != null)
        {
            string requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            logger.LogInformation("Request Body: {Body}", requestBody);
        }

        var response = await base.SendAsync(request, cancellationToken);

        logger.LogInformation("Response: {StatusCode} {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);

        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        logger.LogInformation("Response Body: {Body}", responseBody);

        return response;
    }
}