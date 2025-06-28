using System.Security.Claims;
using OweMe.Application;
using OweMe.Domain.Users;

namespace OweMe.Api.Identity;

public class UserContext : IUserContext
{
    public UserContext(IHttpContextAccessor httpContextAccessor, ILogger<UserContext> logger)
    {
        string? id = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Id = new UserId(string.IsNullOrWhiteSpace(id) ? Guid.Empty : Guid.Parse(id));

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

    public UserId Id { get; }

    public string? Email { get; }

    public bool IsAuthenticated => Id != UserId.Empty && !string.IsNullOrWhiteSpace(Email);
}