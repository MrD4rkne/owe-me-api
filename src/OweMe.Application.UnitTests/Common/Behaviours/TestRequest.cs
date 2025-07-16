using OweMe.Application.Common;

namespace OweMe.Application.UnitTests.Common.Behaviours;

public sealed class TestRequest : IResultRequest<string>
{
    public required string Value { get; init; }

    public override string ToString()
    {
        return $"Value: {Value}";
    }
}