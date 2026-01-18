using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace OweMe.Api.Description;

public class ApiVersionOpenApiDocumentTransformer(IApiInformationProvider apiInformationProvider) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var apiInfo = apiInformationProvider.GetApiInfo();
        
        document.Info ??= new OpenApiInfo();
        document.Info.Title = apiInfo.Title;
        document.Info.Version = apiInfo.Version;
        document.Info.Description = apiInfo.Description;

        return Task.CompletedTask;
    }
}