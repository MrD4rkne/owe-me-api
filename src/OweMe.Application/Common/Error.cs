namespace OweMe.Application.Common;

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Description);
    }
}