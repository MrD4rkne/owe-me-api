using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Moq;
using OweMe.Api.Identity.Configuration;

namespace OweMe.Api.Tests;

public class BearerSecuritySchemeTransformerTests
{
    [Fact]
    public async Task TransformAsync_ShouldAddBearerSecurityScheme_WhenBearerExists()
    {
        // Arrange
        var mockIAuthenticationHandler = new Mock<IAuthenticationHandler>();

        var mockAuthProvider = new Mock<IAuthenticationSchemeProvider>();
        mockAuthProvider
            .Setup(p => p.GetAllSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>
                { new("Bearer", "Bearer", mockIAuthenticationHandler.Object.GetType()) });

        var transformer = new BearerSecuritySchemeTransformer(mockAuthProvider.Object);
        var document = new OpenApiDocument { Paths = new OpenApiPaths() };
        var context = new OpenApiDocumentTransformerContext
        {
            DocumentName = "ExampleDocument",
            DescriptionGroups = [],
            ApplicationServices = new Mock<IServiceProvider>().Object
        };

        // Act
        await transformer.TransformAsync(document, context, CancellationToken.None);

        // Assert
        Assert.NotNull(document.Components);
        Assert.NotNull(document.Components.SecuritySchemes);
        Assert.True(document.Components.SecuritySchemes.ContainsKey("Bearer"));
        Assert.Equal(SecuritySchemeType.Http, document.Components.SecuritySchemes["Bearer"].Type);
    }

    [Fact]
    public async Task TransformAsync_ShouldNotAddBearerSecurityScheme_WhenBearerDoesNotExist()
    {
        // Arrange
        var mockAuthProvider = new Mock<IAuthenticationSchemeProvider>();
        mockAuthProvider
            .Setup(p => p.GetAllSchemesAsync())
            .ReturnsAsync(new List<AuthenticationScheme>()); // No Bearer scheme

        var transformer = new BearerSecuritySchemeTransformer(mockAuthProvider.Object);
        var document = new OpenApiDocument { Paths = new OpenApiPaths() };
        var context = new OpenApiDocumentTransformerContext
        {
            DocumentName = "ExampleDocument",
            DescriptionGroups = [],
            ApplicationServices = new Mock<IServiceProvider>().Object
        };

        // Act
        await transformer.TransformAsync(document, context, CancellationToken.None);

        // Assert
        Assert.Null(document.Components?.SecuritySchemes);
    }
}