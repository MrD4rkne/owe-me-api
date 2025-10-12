using System.Diagnostics.CodeAnalysis;
using OweMe.Api.Description;

namespace OweMe.Api.Endpoints.Common;

public class GetApiInformationEndpoint : IEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/info", GetApiInformation)
            .WithName("GetApiInformation")
            .WithDescription("Get information about the API.")
            .Produces<ApiInformation>()
            .WithTags(Tags.ApiInformation)
            .AllowAnonymous();
    }

    public static Task<IResult> GetApiInformation(
        IApiInformationProvider apiInformationProvider)
    {
        var apiInfo = apiInformationProvider.GetApiInfo();
        return Task.FromResult(Results.Ok(apiInfo));
    }
}