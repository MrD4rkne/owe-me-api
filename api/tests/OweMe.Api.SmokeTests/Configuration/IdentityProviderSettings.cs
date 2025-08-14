namespace OweMe.Api.SmokeTests;

public sealed record IdentityProviderSettings
{
    public const string SectionName = "IdentityProviderSettings";

    public required string Address { get; init; }

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }
}