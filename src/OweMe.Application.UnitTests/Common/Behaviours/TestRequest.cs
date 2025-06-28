using MediatR;

namespace OweMe.Application.UnitTests.Common.Behaviours;

public sealed class TestRequest : IRequest<string>
{
    public required string Value { get; init; }

    public override string ToString()
    {
        return $"Value: {Value}";
    }
}