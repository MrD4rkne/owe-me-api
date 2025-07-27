using System.Reflection;

namespace OweMe.Api.Description;

public class ApiInformationProvider : IApiInformationProvider
{
    public ApiInformation GetApiInfo()
    {
        return new ApiInformation
        {
            Title = OweMeApiInformation.Title,
            Version = OweMeApiInformation.Version,
            Description = OweMeApiInformation.Description,
            BuildVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown"
        };
    }

    private static class OweMeApiInformation
    {
        public const string Title = "OweMe API";

        public const string Version = "1.0.0";

        public const string Description = "API for OweMe application";
    }
}