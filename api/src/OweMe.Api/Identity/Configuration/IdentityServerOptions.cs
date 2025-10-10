namespace OweMe.Api.Identity.Configuration;

public class IdentityServerOptions
{
    public const string SectionName = "IdentityServer";

    public string? Authority { get; set; }

    public bool ValidateAudience { get; set; } = true;

    public string? Audience { get; set; }

    public string? ValidIssuer { get; set; }
    
    public bool? RequireHttpsMetadata { get; set; }
}