using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Hosting;
using OweMe.Application.Common.Behaviours;
using Wolverine;
using Wolverine.FluentValidation;

namespace OweMe.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        builder.UseWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(DependencyInjection).Assembly);

            opts.Policies.AddMiddleware<PerformancePipelineBehavior>();

            opts.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);

            // TODO: use static code in prod.
        });

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}