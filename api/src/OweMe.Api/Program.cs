using JasperFx;
using JasperFx.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OweMe.Api.Description;
using OweMe.Api.Endpoints;
using OweMe.Api.Identity;
using OweMe.Api.Identity.Configuration;
using OweMe.Application;
using OweMe.Infrastructure;
using OweMe.Persistence;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);

if (Log.Logger.GetType().FullName == "Serilog.Core.Pipeline.SilentLogger")
{
    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithSpan()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .CreateBootstrapLogger();
}

builder.Host.UseSerilog();
builder.Services.AddSerilog();

builder.Services.AddOpenTelemetry()
    .WithLogging()
    .WithTracing(b =>
    {
        b.AddAspNetCoreInstrumentation();
        b.AddHttpClientInstrumentation();
    })
    .WithMetrics(b =>
    {
        b.AddAspNetCoreInstrumentation();
        b.AddHttpClientInstrumentation();
    });

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddDocumentTransformer<ApiVersionOpenApiDocumentTransformer>();
});

builder.Services.Configure<IdentityServerOptions>(builder.Configuration.GetSection(IdentityServerOptions.SectionName));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddSingleton<IApiInformationProvider, ApiInformationProvider>();

builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Constants.POLICY_API_SCOPE, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", Constants.POLICY_API_SCOPE_CLAIM);
    });

builder.Services.AddResourceSetupOnStartup();

builder.AddApplication();
builder.AddInfrastructure();

builder.AddPersistence();

builder.Services.AddExceptionHandler<ExceptionProblemDetailsMatcher>();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails = new ExtendedProblemDetails(context.ProblemDetails)
        {
            Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}",
            RequestId = context.HttpContext.TraceIdentifier,
            TraceId = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity?.Id
        };
    };
});

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.Servers = []; // Clear the default servers so it shows only the one the browser is running on,
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.UseExceptionHandler();
app.UseStatusCodePages();

try
{
    return await app.RunJasperFxCommands(args);
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception during application startup");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}