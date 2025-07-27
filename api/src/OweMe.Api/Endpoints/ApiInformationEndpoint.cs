using System.Diagnostics.CodeAnalysis;
using OweMe.Api.Description;

namespace OweMe.Api.Endpoints;

public class ApiInformationEndpoint : IEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/info", GetApiInformation)
            .WithName("GetApiInformation")
            .WithDescription("Get information about the API.")
            .Produces<ApiInfo>()
            .WithTags(Tags.ApiInformation);
    }
    
    public static Task<IResult> GetApiInformation(
        IApiInformationProvider apiInformationProvider)
    {
        var apiInfo = apiInformationProvider.GetApiInfo();
        return Task.FromResult(Results.Ok(apiInfo));
    }
}