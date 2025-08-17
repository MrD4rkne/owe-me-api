using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OweMe.Application.Common;
using OweMe.Application.Common.Middlewares;
using Wolverine;
using Wolverine.FluentValidation;

namespace OweMe.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<ApplicationOptions>();
        
        builder.UseWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(DependencyInjection).Assembly);

            opts.Policies.AddMiddleware<PerformanceMiddleware>();

            opts.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);

            // TODO: use static code in prod.
        });

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}