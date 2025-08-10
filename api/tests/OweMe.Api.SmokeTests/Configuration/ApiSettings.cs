namespace OweMe.Api.SmokeTests;

internal sealed record ApiSettings
{
    public const string SectionName = "ApiSettings";

    public required string BaseUrl { get; init; }
}