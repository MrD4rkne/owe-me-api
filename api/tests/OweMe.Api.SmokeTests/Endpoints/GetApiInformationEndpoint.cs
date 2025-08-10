using Microsoft.Extensions.DependencyInjection;
using OweMe.Api.Client;
using Shouldly;

namespace OweMe.Api.SmokeTests;

public sealed class GetApiInformationEndpoint
{
    private readonly IServiceProvider _serviceProvider;

    public GetApiInformationEndpoint(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    [Fact]
    public async Task ForUnauthorizedUser()
    {
        // Arrange
        var client = _serviceProvider.GetRequiredService<OweMeApiClient>();

        // Act
        var response = await client.GetApiInformationAsync();

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(200);
        response.Result.ShouldNotBeNull();
        response.Result.Title.ShouldBe("OweMe API");
        response.Result.Version.ShouldNotBeNull();
        response.Result.Version.ShouldNotBeEmpty();
        response.Result.Description.ShouldNotBeNull();
        response.Result.Description.ShouldNotBeEmpty();
        response.Result.BuildVersion.ShouldNotBeNull();
        response.Result.BuildVersion.ShouldNotBeEmpty();
        response.Result.AdditionalProperties.ShouldBeEmpty();
    }
}