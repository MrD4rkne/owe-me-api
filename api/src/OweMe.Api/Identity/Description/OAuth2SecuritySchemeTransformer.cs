using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OweMe.Api.Identity.Configuration;

namespace OweMe.Api.Identity.Description;

public sealed class OAuth2SecuritySchemeTransformer(
    ILogger<OAuth2SecuritySchemeTransformer> logger,
    IOptions<IdentityServerOptions> identityServerOptions)
    : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        TryAddOAuth2Description(document, identityServerOptions, logger);
        TryAddBearerDescription(document);

        return Task.CompletedTask;
    }

    private static void TryAddOAuth2Description(OpenApiDocument document, IOptions<IdentityServerOptions> identityServerOptions, ILogger logger)
    {
        if (identityServerOptions.Value.Authority is null)
        {
            logger.LogWarning("Authority is not configured. Skipping OAuth2 security scheme addition.");
            return;
        }
        
        if (!Uri.TryCreate(identityServerOptions.Value.Authority, UriKind.Absolute, out var authorityUri))
        {
            logger.LogWarning("Invalid Authority URI: {Authority}. Skipping OAuth2 security scheme addition.", identityServerOptions.Value.Authority);
            return;
        }
        
        var oauth2Scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Description = "IdentityServer OpenID Connect Password Flow",
            Flows = new OpenApiOAuthFlows
            {
                Password = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri(authorityUri, "connect/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        { Constants.POLICY_API_SCOPE_CLAIM, "Access to the API endpoints" }
                    }
                }
            }
        };

        document.Components.SecuritySchemes.TryAdd("OAuth2", oauth2Scheme);

        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "OAuth2"
                }
            }] = [Constants.POLICY_API_SCOPE_CLAIM]
        };

        document.SecurityRequirements.Add(requirement);
    }

    private static void TryAddBearerDescription(OpenApiDocument document)
    {
        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT token"
        };
        
        document.Components.SecuritySchemes.TryAdd("Bearer", bearerScheme);
        
        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }] = []
        };

        document.SecurityRequirements.Add(requirement);
    }
}