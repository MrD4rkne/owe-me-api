using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OweMe.Api.Identity.Configuration;
using OweMe.Api.Identity.Description;
using Shouldly;

namespace OweMe.Api.Identity.Tests.Description;

public class OAuth2SecuritySchemeTransformerTests
{
    [Fact]
    public async Task TransformAsync_WithValidAuthority_AddsBothOAuth2AndBearerSchemesAndRequirements()
    {
        // Arrange
        var options = new IdentityServerOptions { Authority = "https://identity.example.com" };
        var optionsWrapper = Options.Create(options);
        var logger = NullLogger<OAuth2SecuritySchemeTransformer>.Instance;
        
        var transformer = new OAuth2SecuritySchemeTransformer(logger, optionsWrapper);
        var document = new OpenApiDocument();
        OpenApiDocumentTransformerContext context = default!;

        // Act
        await transformer.TransformAsync(document, context, CancellationToken.None);

        // Assert - Structural Verification
        document.Components.ShouldNotBeNull();
        document.Components.SecuritySchemes.ShouldContainKey("OAuth2");
        document.Components.SecuritySchemes.ShouldContainKey("Bearer");

        // Assert - OAuth2 Specifications
        var oauth2Scheme = document.Components.SecuritySchemes["OAuth2"];
        oauth2Scheme.Type.ShouldBe(SecuritySchemeType.OAuth2);
        oauth2Scheme.Flows.ShouldNotBeNull();
        oauth2Scheme.Flows.Password.ShouldNotBeNull();
        oauth2Scheme.Flows.Password.TokenUrl.ShouldBe(new Uri("https://identity.example.com/connect/token"));
        oauth2Scheme.Flows.Password.Scopes.ShouldContainKey(Constants.POLICY_API_SCOPE_CLAIM);

        // Assert - Bearer Specifications
        var bearerScheme = document.Components.SecuritySchemes["Bearer"];
        bearerScheme.Type.ShouldBe(SecuritySchemeType.Http);
        bearerScheme.Scheme.ShouldBe("Bearer");
        bearerScheme.BearerFormat.ShouldBe("JWT");

        // Assert - Requirements Verification
        document.SecurityRequirements.Count.ShouldBe(2);
        document.SecurityRequirements.Any(r => r.Keys.Any(k => k.Reference?.Id == "OAuth2")).ShouldBeTrue();
        document.SecurityRequirements.Any(r => r.Keys.Any(k => k.Reference?.Id == "Bearer")).ShouldBeTrue();
    }

    [Fact]
    public async Task TransformAsync_WithNullAuthority_SkipsOAuth2AndOnlyAddsBearer()
    {
        // Arrange
        var options = new IdentityServerOptions { Authority = null };
        var optionsWrapper = Options.Create(options);
        var logger = NullLogger<OAuth2SecuritySchemeTransformer>.Instance;
        
        var transformer = new OAuth2SecuritySchemeTransformer(logger, optionsWrapper);
        var document = new OpenApiDocument();
        OpenApiDocumentTransformerContext context = default!;

        // Act
        await transformer.TransformAsync(document, context, CancellationToken.None);

        // Assert
        document.Components.ShouldNotBeNull();
        document.Components.SecuritySchemes.ShouldNotContainKey("OAuth2");
        document.Components.SecuritySchemes.ShouldContainKey("Bearer");
        
        document.SecurityRequirements.Count.ShouldBe(1);
        document.SecurityRequirements.First().Keys.First().Reference.Id.ShouldBe("Bearer");
    }

    [Fact]
    public async Task TransformAsync_WithInvalidAuthorityUri_SkipsOAuth2AndOnlyAddsBearer()
    {
        // Arrange
        var options = new IdentityServerOptions { Authority = "not-a-valid-uri" };
        var optionsWrapper = Options.Create(options);
        var logger = NullLogger<OAuth2SecuritySchemeTransformer>.Instance;
        
        var transformer = new OAuth2SecuritySchemeTransformer(logger, optionsWrapper);
        var document = new OpenApiDocument();
        OpenApiDocumentTransformerContext context = default!;

        // Act
        await transformer.TransformAsync(document, context, CancellationToken.None);

        // Assert
        document.Components.ShouldNotBeNull();
        document.Components.SecuritySchemes.ShouldNotContainKey("OAuth2");
        document.Components.SecuritySchemes.ShouldContainKey("Bearer");
        
        document.SecurityRequirements.Count.ShouldBe(1);
        document.SecurityRequirements.First().Keys.First().Reference.Id.ShouldBe("Bearer");
    }
}