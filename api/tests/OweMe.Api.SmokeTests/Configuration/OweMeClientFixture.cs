using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OweMe.Api.Client;

namespace OweMe.Api.SmokeTests;

public class OweMeClientFixture
{
    private readonly IServiceProvider _serviceProvider;

    public OweMeClientFixture()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit());

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var testSettings = configuration.GetSection(TestSettings.SectionName).Get<TestSettings>();
        if (testSettings == null || string.IsNullOrEmpty(testSettings.BaseUrl))
        {
            throw new InvalidOperationException("TestSettings or BaseUrl is not configured properly.");
        }

        services.AddTransient<LoggingDelegatingHandler>();

        services.AddHttpClient<OweMeApiClient>(client => { client.BaseAddress = new Uri(testSettings.BaseUrl); })
            .AddHttpMessageHandler<LoggingDelegatingHandler>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public OweMeApiClient GetOweMeApiClient()
    {
        return _serviceProvider.GetRequiredService<OweMeApiClient>();
    }
}