namespace OweMe.Api.Configuration.Identity;

public class IdentityServerOptions
{
    public const string SECTION_NAME = "IdentityServer";
    
    public string Authority { get; set; }
    
    public bool ValidateAudience { get; set; } = true;
    
    public string? Audience { get; set; }
    
    public string? ValidIssuer { get; set; }
}