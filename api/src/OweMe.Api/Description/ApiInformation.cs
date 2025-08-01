﻿namespace OweMe.Api.Description;

public sealed record ApiInformation
{
    public required string Title { get; init; }

    public required string Version { get; init; }

    public required string Description { get; init; }

    public required string BuildVersion { get; init; }
}