namespace OweMe.Api.Identity.Configuration;

using System.ComponentModel.DataAnnotations;

public sealed record IdentityServerOptions
{
    public const string SectionName = "IdentityServer";

    [Required(ErrorMessage = "The IdentityServer Authority URL is required.")]
    [Url(ErrorMessage = "The Authority must be a valid URL.")]
    public string? Authority { get; set; }

    public bool ValidateAudience { get; set; } = true;

    public string? Audience { get; set; }

    public string? ValidIssuer { get; set; }
    
    public bool? RequireHttpsMetadata { get; set; }
}