using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OweMe.Application.Common.Behaviours;

namespace OweMe.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IUserContext>());
        
        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));
        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformancePipelineBehaviour<,>));
    }
}