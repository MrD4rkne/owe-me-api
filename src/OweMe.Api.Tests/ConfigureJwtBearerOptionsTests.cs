using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using OweMe.Api.Identity.Configuration;
using Shouldly;

namespace OweMe.Api.Tests;

public class ConfigureJwtBearerOptionsTests
{
    [Fact]
    public void Configure_Should_Set_Authority()
    {
        IdentityServerOptions identityServerOptions = new()
        {
            Authority = "https://example.com",
            ValidateAudience = true,
            Audience = "audience",
            ValidIssuer = "issuer"
        };
        var environment = new Mock<IHostEnvironment>();
        var configureJwtBearerOptions =
            new ConfigureJwtBearerOptions(Options.Create(identityServerOptions), environment.Object);

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
    }

    [Fact]
    public void Configure_Should_Not_Set_Audience_When_Not_Provided()
    {
        var identityServerOptions = new IdentityServerOptions
        {
            Authority = "https://example.com",
            ValidateAudience = true,
            Audience = null,
            ValidIssuer = "issuer"
        };
        var environment = new Mock<IHostEnvironment>();
        var configureJwtBearerOptions =
            new ConfigureJwtBearerOptions(Options.Create(identityServerOptions), environment.Object);

        var options = new JwtBearerOptions();

        // Act
        configureJwtBearerOptions.Configure(options);

        // Assert
        options.Audience.ShouldBeNull("Audience should not be set");
    }

    [Fact]
    public void Configure_Should_Not_Set_ValidIssuer_When_Not_Provided()
    {
        var identityServerOptions = new IdentityServerOptions
        {
            Authority = "https://example.com",
            ValidateAudience = true,
            Audience = "audience",
            ValidIssuer = null
        };
        var environment = new Mock<IHostEnvironment>();
        var configureJwtBearerOptions =
            new ConfigureJwtBearerOptions(Options.Create(identityServerOptions), environment.Object);

        var options = new JwtBearerOptions();

        // Act
        configureJwtBearerOptions.Configure(options);

        // Assert
        options.TokenValidationParameters.ValidIssuer.ShouldBeNull("ValidIssuer should not be set");
    }

    [Fact]
    public void Configure_Should_Disable_RequireHttpsMetadata_In_Development()
    {
        var identityServerOptions = new IdentityServerOptions
        {
            Authority = "https://example.com",
            ValidateAudience = true,
            Audience = "audience",
            ValidIssuer = "issuer"
        };
        var environment = new Mock<IHostEnvironment>();
        environment.Setup(e => e.EnvironmentName).Returns(Environments.Development);
        var configureJwtBearerOptions =
            new ConfigureJwtBearerOptions(Options.Create(identityServerOptions), environment.Object);

        var options = new JwtBearerOptions();

        // Act
        configureJwtBearerOptions.Configure(options);

        // Assert
        options.RequireHttpsMetadata.ShouldBeFalse("RequireHttpsMetadata should be false in development");
    }
}