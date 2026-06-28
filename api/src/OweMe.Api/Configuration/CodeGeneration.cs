using System.Reflection;

namespace OweMe.Api.Configuration;

// Origin: https://github.com/JasperFx/wolverine/blob/6e2a244f1a2a6aab6c94935cef42d72c51f9464b/docs/guide/codegen.md?plain=1#L532-L540
internal static class CodeGeneration
{
    internal static bool IsRunningGeneration()
    {
        return Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider" || Environment.GetCommandLineArgs().Contains("codegen");
    }
}