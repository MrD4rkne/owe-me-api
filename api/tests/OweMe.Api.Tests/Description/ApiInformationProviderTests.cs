using System.Reflection;
using OweMe.Api.Description;
using Shouldly;

namespace OweMe.Api.Tests.Description;

public class ApiInformationProviderTests
{
    [Fact]
    public void GetApiInfo_ShouldReturnApiInformation()
    {
        // Arrange
        var provider = new ApiInformationProvider();

        // Act
        var apiInfo = provider.GetApiInfo();

        // Assert
        apiInfo.ShouldNotBeNull();
        apiInfo.Version.ShouldBe("1.0.1");
        apiInfo.BuildVersion.ShouldBe(ThisAssembly.AssemblyInformationalVersion); // Assuming ThisAssembly.AssemblyVersion is set the same in all projects.
        apiInfo.Description.ShouldBe("API for OweMe application");
        apiInfo.Title.ShouldBe("OweMe API");
    }
}