using OweMe.Api.SmokeTests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace OweMe.Api.SmokeTests;

public sealed class GetApiInformationEndpoint
{
    private readonly ClientFactory _clientFactory;

    public GetApiInformationEndpoint()
    {
        _clientFactory = new ClientFactory(new Settings
        {
            ApiBaseUrl = "https://localhost:8081" // Adjust the base URL as needed
        });
    }
    
    [Fact]
    public async Task ForUnauthorizedUser()
    {
        // Arrange
        var client = _clientFactory.CreateClient();

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