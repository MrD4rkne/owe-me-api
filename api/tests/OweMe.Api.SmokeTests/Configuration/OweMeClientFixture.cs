using Duende.IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OweMe.Api.Client;

namespace OweMe.Api.SmokeTests;

public class OweMeClientFixture
{
    private const string authenticatedClientKey = "authenticatedClient";
    
    private readonly IServiceProvider _serviceProvider;

    public OweMeClientFixture()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit());

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var testSettings = configuration.GetSection(ApiSettings.SectionName).Get<ApiSettings>();
        if (testSettings == null || string.IsNullOrEmpty(testSettings.BaseUrl))
        {
            throw new InvalidOperationException("TestSettings or BaseUrl is not configured properly.");
        }

        services.AddSingleton(testSettings);
        services.Configure<UserSettings>(configuration.GetSection("UserSettings"));
        services.Configure<TokenClientOptions>(configuration.GetSection("IdentityProviderSettings"));

        services.AddTransient<LoggingDelegatingHandler>();
        services.AddTransient<AuthorizationDelegatingHandler>();

        services.AddHttpClient<OweMeApiClient>(client => { client.BaseAddress = new Uri(testSettings.BaseUrl); })
            .AddHttpMessageHandler<LoggingDelegatingHandler>();

        services.AddHttpClient<TokenClient>()
            .AddHttpMessageHandler<LoggingDelegatingHandler>();

        services.AddHttpClient(authenticatedClientKey,
                client => { client.BaseAddress = new Uri(testSettings.BaseUrl); })
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .AddHttpMessageHandler<AuthorizationDelegatingHandler>();

        services.AddKeyedTransient<OweMeApiClient>(authenticatedClientKey, (sp, key) =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient(key.ToString());
            return new OweMeApiClient(httpClient);
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    public OweMeApiClient GetOweMeApiClient()
    {
        return _serviceProvider.GetRequiredService<OweMeApiClient>();
    }

    public OweMeApiClient GetAuthenticatedOweMeApiClientAsync(
        CancellationToken cancellationToken = default)
    {
        return _serviceProvider.GetRequiredKeyedService<OweMeApiClient>(authenticatedClientKey);
    }
}