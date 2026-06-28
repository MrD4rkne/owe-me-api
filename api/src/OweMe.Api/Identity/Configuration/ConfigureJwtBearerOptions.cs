using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace OweMe.Api.Identity.Configuration;

internal sealed class ConfigureJwtBearerOptions(
    IOptions<IdentityServerOptions> identityServerOptions) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(JwtBearerOptions options)
    {
        options.Authority = identityServerOptions.Value.Authority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = identityServerOptions.Value.ValidIssuer,
            ValidateAudience = identityServerOptions.Value.ValidateAudience,
            ValidateLifetime = true,
        };

        options.Audience = identityServerOptions.Value.Audience;

        options.TokenValidationParameters.ValidIssuer = identityServerOptions.Value.ValidIssuer;
        options.TokenValidationParameters.ValidTypes = ["at+jwt"];
        
        if (!string.IsNullOrEmpty(identityServerOptions.Value.MetadataAddress))
        {
            options.MetadataAddress = identityServerOptions.Value.MetadataAddress;
        }
        options.RequireHttpsMetadata = identityServerOptions.Value.RequireHttpsMetadata;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}