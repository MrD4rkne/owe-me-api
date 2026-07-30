namespace OweMe.Api.Identity.Configuration;

using System.ComponentModel.DataAnnotations;

public sealed record IdentityServerOptions
{
    public const string SectionName = "IdentityServer";

    [Required(ErrorMessage = "The IdentityServer Authority URL is required.")]
    [Url(ErrorMessage = "The Authority must be a valid URL.")]
    public required string Authority { get; set; }

    [Url(ErrorMessage = "The Metadata Address must be a valid URL.")]
    public string? MetadataAddress { get; set; }

    public bool RequireHttpsMetadata { get; set; } = true;

    public string Audience { get; set; } = "oweme-api";

    public bool ValidateAudience { get; set; } = true;

    private string? _validIssuer;

    /// <summary>
    /// Valid issuer for token validation. If not set, defaults to the Authority URL.
    /// </summary>
    public string ValidIssuer
    {
        get => _validIssuer ?? Authority;
        set => _validIssuer = value;
    }
}