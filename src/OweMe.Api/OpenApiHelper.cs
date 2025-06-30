using System.Reflection;

namespace OweMe.Api;

public static class OpenApiHelper
{
    public static bool IsGeneratingApiSpec()
    {
        return Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider";
    }
}