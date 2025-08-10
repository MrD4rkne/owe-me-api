using OweMe.Api.Client;

namespace OweMe.Api.SmokeTests.Helpers;

public class ClientFactory(Settings settings)
{
    public OweMeApiClient CreateClient()
    {
        return new OweMeApiClient(CreateHttpClient());
    }
    
    private HttpClient CreateHttpClient()
    {
        return new HttpClient
        {
            BaseAddress = new Uri(settings.ApiBaseUrl)
        };
    }
}