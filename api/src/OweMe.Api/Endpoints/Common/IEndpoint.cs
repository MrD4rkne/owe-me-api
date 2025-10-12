namespace OweMe.Api.Endpoints.Common;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder builder);
}