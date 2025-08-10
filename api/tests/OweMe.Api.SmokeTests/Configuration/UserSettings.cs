namespace OweMe.Api.SmokeTests;

internal sealed record UserSettings
{
    public const string SectionName = "UserSettings";

    public required string Username { get; init; }

    public required string Password { get; init; }

    public required string Scope { get; init; }
}