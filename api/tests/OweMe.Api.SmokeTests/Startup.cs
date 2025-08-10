using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OweMe.Api.Client;

namespace OweMe.Api.SmokeTests;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var testSettings = _configuration.GetSection(TestSettings.SectionName).Get<TestSettings>();
        if (testSettings == null || string.IsNullOrEmpty(testSettings.BaseUrl))
        {
            throw new InvalidOperationException("TestSettings or BaseUrl is not configured properly.");
        }

        services.AddHttpClient<OweMeApiClient>(client => { client.BaseAddress = new Uri(testSettings.BaseUrl); });
    }
}