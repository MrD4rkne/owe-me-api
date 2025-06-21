namespace OweMe.Domain.Users;

public readonly record struct UserId(Guid Id)
{
    public override string ToString() => Id.ToString();
    
    public static implicit operator Guid(UserId userId) => userId.Id;
    
    public static implicit operator UserId(Guid id) => new(id);
    
    public static UserId Empty => new(Guid.Empty);
    
    public static UserId New() => new(Guid.NewGuid());
}