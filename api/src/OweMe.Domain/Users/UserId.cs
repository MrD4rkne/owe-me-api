namespace OweMe.Domain.Users;

public readonly record struct UserId(Guid Id)
{
    [Obsolete("Do not use the default constructor. Use UserId.New() or UserId.Empty instead.", true)]
    public UserId() : this(Guid.Empty)
    {
    }

    public static UserId Empty => new(Guid.Empty);

    public override string ToString()
    {
        return Id.ToString();
    }

    public static implicit operator Guid(UserId userId)
    {
        return userId.Id;
    }

    public static implicit operator UserId(Guid id)
    {
        return new UserId(id);
    }

    public static UserId New()
    {
        return new UserId(Guid.NewGuid());
    }
}