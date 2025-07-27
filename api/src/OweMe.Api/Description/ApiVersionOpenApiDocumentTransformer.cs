using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace OweMe.Api.Description;

public class ApiVersionOpenApiDocumentTransformer(IApiInformationProvider apiInformationProvider) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var apiInfo = apiInformationProvider.GetApiInfo();
        
        document.Info = new OpenApiInfo
        {
            Title = apiInfo.Title,
            Version = apiInfo.Version,
            Description = apiInfo.Description
        };

        return Task.CompletedTask;
    }
}