using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OweMe.Api.Description;
using OweMe.Api.Endpoints;
using OweMe.Api.Endpoints.Common;
using Shouldly;

namespace OweMe.Api.Tests.Endpoints;

public class GetApiInformationEndpointTests
{
    private readonly Mock<IApiInformationProvider> _apiInformationProvider = new();

    [Fact]
    public async Task GetApiInformation_ShouldReturnApiInfo()
    {
        // Arrange
        var expectedApiInfo = new ApiInformation
        {
            Title = "OweMe API",
            Version = "1.0.0",
            Description = "API for managing debts and ledgers.",
            BuildVersion = "1.0.0+build.123"
        };

        _apiInformationProvider
            .Setup(provider => provider.GetApiInfo())
            .Returns(expectedApiInfo);

        // Act
        var result = await GetApiInformationEndpoint.GetApiInformation(_apiInformationProvider.Object);

        // Assert
        result.ShouldBeOfType<Ok<ApiInformation>>();
        var okResult = result as Ok<ApiInformation>;
        okResult.ShouldNotBeNull();
        okResult.Value.ShouldNotBeNull();
        okResult.Value.ShouldBeEquivalentTo(expectedApiInfo);

        _apiInformationProvider.Verify(provider => provider.GetApiInfo(), Times.Once);
    }
}