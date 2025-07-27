using OweMe.Api.Description;

namespace OweMe.Api.Controllers;

public class ApiInformationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/info", GetApiInformation)
            .WithName("GetApiInformation")
            .Produces<ApiInfo>()
            .WithTags(Description.Tags.ApiInformation);
    }
    
    public static Task<IResult> GetApiInformation(
        IApiInformationProvider apiInformationProvider)
    {
        var apiInfo = apiInformationProvider.GetApiInfo();
        return Task.FromResult(Results.Ok(apiInfo));
    }
}