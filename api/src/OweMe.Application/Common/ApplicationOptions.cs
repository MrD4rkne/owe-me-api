namespace OweMe.Application.Common;

public sealed record ApplicationOptions
{
    public int TooLongRequestThresholdMs { get; init; } = 500;
}