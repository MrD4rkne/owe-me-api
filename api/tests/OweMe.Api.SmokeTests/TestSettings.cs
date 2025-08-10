namespace OweMe.Api.SmokeTests;

public sealed record TestSettings
{
    public const string SectionName = "TestSettings";

    public string BaseUrl { get; init; }
}