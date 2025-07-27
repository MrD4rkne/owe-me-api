namespace OweMe.Api.Description;

public class ApiInformationProvider : IApiInformationProvider
{
    public ApiInfo GetApiInfo()
    {
        return new ApiInfo
        {
            Title = ApiInformation.Title,
            Version = ApiInformation.Version,
            Description = ApiInformation.Description,
            BuildVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown"
        };
    }
}