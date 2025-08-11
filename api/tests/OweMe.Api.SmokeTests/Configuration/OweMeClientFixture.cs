using Duende.IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OweMe.Api.Client;

namespace OweMe.Api.SmokeTests;

public class OweMeClientFixture
{
    public const string AuthenticatedClientKey = "authenticatedOweMeClient";
    public const string UnauthenticatedClientKey = "unauthenticatedOweMeClient";
    
    private readonly IServiceProvider _serviceProvider;

    public OweMeClientFixture()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit());

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        var testSettings = configuration.GetSection(ApiSettings.SectionName).Get<ApiSettings>();
        if (testSettings == null || string.IsNullOrEmpty(testSettings.BaseUrl))
        {
            throw new InvalidOperationException("TestSettings or BaseUrl is not configured properly.");
        }

        services.AddSingleton(testSettings);
        services.Configure<UserSettings>(configuration.GetSection(UserSettings.SectionName));
        services.Configure<IdentityProviderSettings>(configuration.GetSection(IdentityProviderSettings.SectionName));

        services.AddTransient<TokenClientOptions>(options =>
        {
            var apiSettings = options.GetRequiredService<IOptions<IdentityProviderSettings>>().Value;
            return new TokenClientOptions
            {
                Address = $"{apiSettings.Address.TrimEnd('/')}/connect/token",
                ClientId = apiSettings.ClientId,
                ClientSecret = apiSettings.ClientSecret
            };
        });
        services.AddHttpClient<TokenClient>()
            .AddHttpMessageHandler<LoggingDelegatingHandler>();

        services.AddHttpClient(AuthenticatedClientKey, client => client.BaseAddress = new Uri(testSettings.BaseUrl))
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .AddHttpMessageHandler<AuthorizationDelegatingHandler>();
        services.AddKeyedTransient(AuthenticatedClientKey, CreateOweMeClientFromKey);

        services.AddHttpClient(UnauthenticatedClientKey, client => client.BaseAddress = new Uri(testSettings.BaseUrl))
            .AddHttpMessageHandler<LoggingDelegatingHandler>();
        services.AddKeyedTransient(UnauthenticatedClientKey, CreateOweMeClientFromKey);

        services.AddTransient<LoggingDelegatingHandler>();
        services.AddTransient<AuthorizationDelegatingHandler>();

        _serviceProvider = services.BuildServiceProvider();
    }

    private static OweMeApiClient CreateOweMeClientFromKey(IServiceProvider sp, object? key)
    {
        var name = key?.ToString();
        ArgumentNullException.ThrowIfNullOrEmpty(name);

        var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
        return new OweMeApiClient(clientFactory.CreateClient(name));
    }

    public OweMeApiClient GetClient(string clientKey)
    {
        return _serviceProvider.GetRequiredKeyedService<OweMeApiClient>(clientKey) ??
               throw new InvalidOperationException($"Client with key '{clientKey}' not found.");
    }
}