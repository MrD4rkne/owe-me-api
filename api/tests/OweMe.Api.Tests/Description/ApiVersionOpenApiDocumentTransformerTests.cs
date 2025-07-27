using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Moq;
using OweMe.Api.Description;
using Shouldly;

namespace OweMe.Api.Tests.Description;

public class ApiVersionOpenApiDocumentTransformerTests
{
    private readonly Mock<IApiInformationProvider> _apiInformationProviderMock = new();
    
    private readonly ApiInformation _apiInformation = new()
    {
        Title = "Some title",
        Version = "2.0.0",
        Description = "Some description",
        BuildVersion = "1.0.5"
    };
    
    public ApiVersionOpenApiDocumentTransformerTests()
    {
        _apiInformationProviderMock.Setup(provider => provider.GetApiInfo())
            .Returns(_apiInformation);
    }
    
    [Fact]
    public async Task Transform_ShouldReturnTransformedDocument()
    {
        // Arrange
        var sut = new ApiVersionOpenApiDocumentTransformer(_apiInformationProviderMock.Object);
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = "Test API",
                Version = "1.0.0",
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    { "x-version", new OpenApiString("5.0.0") }
                }
            }
        };
        
        var context = new OpenApiDocumentTransformerContext()
        {
            DocumentName = "test-document",
            DescriptionGroups = [],
            ApplicationServices = Mock.Of<IServiceProvider>()
        };

        // Act
        await sut.TransformAsync(document, context, CancellationToken.None);

        // Assert
        document.Info.ShouldNotBeNull();
        document.Info.Title.ShouldBe(_apiInformation.Title, "Transformer should set the title from ApiInformation");
        document.Info.Version.ShouldBe(_apiInformation.Version, "Transformer should set the version from ApiInformation");
        document.Info.Description.ShouldBe(_apiInformation.Description, "Transformer should set the description from ApiInformation");
        document.Info.Extensions.ShouldNotBeNull();
        document.Info.Extensions.ShouldContain(
            ext => ext.Key == "x-version", "Transformer should not remove existing extensions");
        
        document.Info.Extensions["x-version"].ShouldBeOfType<OpenApiString>();
        var versionExtension = document.Info.Extensions["x-version"] as OpenApiString;
        versionExtension.ShouldNotBeNull();
        versionExtension!.Value.ShouldBe("5.0.0");
        
        _apiInformationProviderMock.Verify(provider => provider.GetApiInfo(), Times.AtLeastOnce,
            "ApiInformationProvider should be called to get API information"); 
    }
}