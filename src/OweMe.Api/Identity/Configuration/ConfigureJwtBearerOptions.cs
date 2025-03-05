using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace OweMe.Api.Identity.Configuration;

public class ConfigureJwtBearerOptions(IOptions<IdentityServerOptions> identityServerOptions,
    IHostEnvironment environment) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(JwtBearerOptions options)
    {
        options.Authority = identityServerOptions.Value.Authority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = identityServerOptions.Value.ValidateAudience
        };
        
        if(!string.IsNullOrWhiteSpace(identityServerOptions.Value.Audience))
        {
            options.Audience = identityServerOptions.Value.Audience;
        }
        
        if(!string.IsNullOrWhiteSpace(identityServerOptions.Value.ValidIssuer))
        {
            options.TokenValidationParameters.ValidIssuer = identityServerOptions.Value.ValidIssuer;
        }

        if(environment.IsDevelopment())
        {
            options.RequireHttpsMetadata = false;
        }
    }

    public void Configure(string name, JwtBearerOptions options)
    {
        Configure(options);
    }
}