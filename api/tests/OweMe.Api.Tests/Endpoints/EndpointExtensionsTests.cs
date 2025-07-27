using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OweMe.Api.Endpoints;
using Shouldly;

namespace OweMe.Api.Tests.Endpoints;

public class EndpointExtensionsTests
{
    [Fact]
    public void AddEndpoints_ShouldRegisterEndpoints()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(TestGetEndpoint).Assembly;

        // Act
        services.AddEndpoints(assembly);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var endpoints = serviceProvider.GetServices<IEndpoint>().ToList();
        endpoints.ShouldNotBeEmpty();
        
        endpoints.ShouldContain(e => e is TestGetEndpoint);
        endpoints.ShouldContain(e => e is TestPostEndpoint);
    }
    
    [Fact]
    public void MapEndpoints_ShouldMapEndpoints()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddEndpoints(typeof(TestGetEndpoint).Assembly);
        var app = builder.Build();

        // Act
        app.MapEndpoints();

        // Assert
        var endpoints = (app as IEndpointRouteBuilder).DataSources
            .SelectMany(ds => ds.Endpoints)
            .ToList();
        
        endpoints.ShouldNotBeEmpty();
        endpoints.ShouldContain(e => e.DisplayName == "HTTP: GET /test");
        endpoints.ShouldContain(e => e.DisplayName == "HTTP: POST /test");
    }
    
    private sealed class TestGetEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("/test", () => "Test endpoint");
        }
    }
    
    private sealed class TestPostEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost("/test", () => "Test endpoint");
        }
    }
}