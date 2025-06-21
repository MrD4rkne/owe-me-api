using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OweMe.Api.Identity;
using OweMe.Domain.Users;
using Shouldly;

namespace OweMe.Api.Tests;

public class UserContextTests
{
    private const string AUTH_SCHEME = JwtBearerDefaults.AuthenticationScheme;

    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();

    private readonly Mock<ILogger<UserContext>> _logger = new();

    [Fact]
    public void UserContext_WhenUserIsAuthenticated_ShouldShowUserData()
    {
        // Arrange
        var id = UserId.New();
        const string email = "user@oweme.pl";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, id.ToString()),
            new(ClaimTypes.Email, email)
        };

        var identity = new ClaimsIdentity(claims, AUTH_SCHEME);
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var userContext = new UserContext(_httpContextAccessor.Object, _logger.Object);

        // Assert
        userContext.Id.ShouldBe(id);
        userContext.Email.ShouldBe(email);
        userContext.IsAuthenticated.ShouldBeTrue();
    }
    
    [Fact]
    public void UserContext_WhenUserIsNotAuthenticated_ShouldNotShowUserData()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var userContext = new UserContext(_httpContextAccessor.Object, _logger.Object);

        // Assert
        userContext.Id.ShouldBe(UserId.Empty);
        userContext.Email.ShouldBeNull();
        userContext.IsAuthenticated.ShouldBeFalse();
    }
}