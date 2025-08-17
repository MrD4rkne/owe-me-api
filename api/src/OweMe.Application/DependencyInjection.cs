using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using JasperFx;
using JasperFx.CodeGeneration;
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

            if (builder.Environment.IsProduction())
            {
                opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;
                opts.Services.CritterStackDefaults(cr =>
                {
                    // I'm only going to care about this in production
                    cr.Production.AssertAllPreGeneratedTypesExist = true;
                });
            }
        });

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}