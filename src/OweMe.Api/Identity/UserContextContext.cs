using System.Security.Claims;
using OweMe.Application;

namespace OweMe.Api.Identity;

public class UserContextContext : IUserContext
{
    public UserContextContext(IHttpContextAccessor httpContextAccessor, ILogger<UserContextContext> logger)
    {
        var id = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Id = string.IsNullOrWhiteSpace(id) ? Guid.Empty : Guid.Parse(id);
        
        Email = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        if (IsAuthenticated)
        {
            logger.LogDebug("User authenticated: {Id} - {Email}", Id, Email);
        }
        else
        {
            logger.LogDebug("User not authenticated");
        }
    }
    
    public Guid Id { get; }
    
    public string? Email { get; }
    
    public bool IsAuthenticated => Id != Guid.Empty && !string.IsNullOrWhiteSpace(Email);
}