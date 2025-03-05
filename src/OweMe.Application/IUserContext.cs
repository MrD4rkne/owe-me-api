namespace OweMe.Application;

/// <summary>
/// Represents the user context.
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// User's id.  <see cref="Guid.Empty"/> if not authenticated <see cref="IsAuthenticated"/>.
    /// </summary>
    Guid Id { get; }
    
    /// <summary>
    /// User's email. <see langword="null"/> if not authenticated <see cref="IsAuthenticated"/>.
    /// </summary>
    string? Email { get; }
    
    /// <summary>
    /// Indicates whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}