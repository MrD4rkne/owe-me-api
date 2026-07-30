using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using OweMe.Api.Identity.Configuration;
using Shouldly;

namespace OweMe.Api.Tests.Identity;

public class ConfigureJwtBearerOptionsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Configure_Should_Set_Authority(bool requireHttpsMetadata = true)
    {
        IdentityServerOptions identityServerOptions = new()
        {
            Authority = "https://example.com",
            ValidateAudience = true,
            Audience = "audience",
            ValidIssuer = "issuer",
            MetadataAddress = "http://metadata.com",
            RequireHttpsMetadata = requireHttpsMetadata,
        };

        var configureJwtBearerOptions =
            new ConfigureJwtBearerOptions(Options.Create(identityServerOptions));

        var options = new JwtBearerOptions();

        // Act
        configureJwtBearerOptions.Configure(options);

        // Assert
        options.Authority.ShouldBe(identityServerOptions.Authority, "Authority should be set");
        options.TokenValidationParameters.ValidateAudience.ShouldBe(identityServerOptions.ValidateAudience,
            "ValidateAudience should be set");
        options.Audience.ShouldBe(identityServerOptions.Audience, "Audience should be set");

        options.TokenValidationParameters.ValidIssuer.ShouldBe(identityServerOptions.ValidIssuer,
            "ValidIssuer should be set");
        options.TokenValidationParameters.ValidateIssuer.ShouldBe(true);
        
        options.TokenValidationParameters.ValidateLifetime.ShouldBe(true);

        options.MetadataAddress.ShouldBe(identityServerOptions.MetadataAddress, "MetadataAddress should be set");
        options.RequireHttpsMetadata.ShouldBe(identityServerOptions.RequireHttpsMetadata, "RequireHttpsMetadata should be set");
    }

    [Fact]
    public void Configure_Should_SetIssuerToAuthority_When_Not_Provided()
    {
        var identityServerOptions = new IdentityServerOptions
        {
            Authority = "https://example.com",
            ValidateAudience = true,
            Audience = "audience",
        };
        var configureJwtBearerOptions =
            new ConfigureJwtBearerOptions(Options.Create(identityServerOptions));

        var options = new JwtBearerOptions();

        // Act
        configureJwtBearerOptions.Configure(options);

        // Assert
        options.TokenValidationParameters.ValidIssuer.ShouldBe(identityServerOptions.Authority, "ValidIssuer should default to Authority when not provided");
    }
}