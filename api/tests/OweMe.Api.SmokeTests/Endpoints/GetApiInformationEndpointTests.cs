using Shouldly;

namespace OweMe.Api.SmokeTests.Endpoints;

public sealed class GetApiInformationEndpointTests(OweMeClientFixture fixture)
{
    [Theory]
    [InlineData(OweMeClientFixture.AuthenticatedClientKey, Label = "Authenticated Client")]
    [InlineData(OweMeClientFixture.UnauthenticatedClientKey, Label = "Unauthenticated Client")]
    public async Task ShouldReturnProperSettings(string oweMeClientKey)
    {
        // Arrange
        var client = fixture.GetClient(oweMeClientKey);
        
        // Act
        var response = await client.GetApiInformationAsync(CancellationToken.None);

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