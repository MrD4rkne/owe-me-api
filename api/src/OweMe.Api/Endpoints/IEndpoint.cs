namespace OweMe.Api.Controllers;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}