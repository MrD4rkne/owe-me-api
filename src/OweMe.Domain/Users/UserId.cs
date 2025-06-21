namespace OweMe.Domain.Users;

public readonly record struct UserId(Guid Id)
{
    [Obsolete("Do not use the default constructor. Use UserId.New() or UserId.Empty instead.", true)]
    public UserId() : this(Guid.Empty) { }

    public override string ToString() => Id.ToString();
    
    public static implicit operator Guid(UserId userId) => userId.Id;
    
    public static implicit operator UserId(Guid id) => new(id);
    
    public static UserId Empty => new(Guid.Empty);
    
    public static UserId New() => new(Guid.NewGuid());
}