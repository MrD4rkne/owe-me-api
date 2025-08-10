using Shouldly;

namespace OweMe.Api.SmokeTests.Endpoints;

public sealed class GetApiInformationEndpoint(OweMeClientFixture fixture)
{
    [Fact]
    public async Task For_UnauthorizedUser_ShouldReturnProperSettings()
    {
        // Arrange
        var client = fixture.GetOweMeApiClient();
        
        // Act
        var response = await client.GetApiInformationAsync(TestContext.Current.CancellationToken);

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

    [Fact]
    public async Task For_AuthorizedUser_ShouldReturnProperSettings()
    {
        // Arrange
        var client = fixture.GetAuthenticatedOweMeApiClientAsync();

        // Act
        var response = await client.GetApiInformationAsync(TestContext.Current.CancellationToken);

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