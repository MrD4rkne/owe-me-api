using System.Diagnostics.CodeAnalysis;

namespace OweMe.Application.Common;

public readonly struct Error(string code, string description) : IEquatable<Error>
{
    public string Code { get; } = code;
    public string Description { get; } = description;

    public static readonly Error None = new(string.Empty, string.Empty);

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Error other &&
               Code == other.Code &&
               Description == other.Description;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Description);
    }

    public bool Equals(Error other)
    {
        return Code == other.Code && Description == other.Description;
    }

    public static bool operator ==(Error left, Error right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Error left, Error right)
    {
        return !(left == right);
    }
}