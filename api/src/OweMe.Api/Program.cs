using Azure.Monitor.OpenTelemetry.AspNetCore;
using JasperFx;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OweMe.Api.Configuration;
using OweMe.Api.Description;
using OweMe.Api.Endpoints;
using OweMe.Api.Identity;
using OweMe.Api.Identity.Configuration;
using OweMe.Api.Identity.Description;
using OweMe.Application;
using OweMe.Infrastructure;
using OweMe.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.IncludeFormattedMessage = true;
    logging.AddOtlpExporter();
});

var otel = builder.Services.AddOpenTelemetry()
    .WithLogging(b =>
    {
        b.AddOtlpExporter();
    })
    .WithTracing(b =>
    {
        b.AddAspNetCoreInstrumentation();
        b.AddHttpClientInstrumentation();
        b.AddOtlpExporter();
    })
    .WithMetrics(b =>
    {
        b.AddAspNetCoreInstrumentation();
        b.AddHttpClientInstrumentation();
        b.AddOtlpExporter();
    });
if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
{
    otel.UseAzureMonitor();
}

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<OAuth2SecuritySchemeTransformer>();
    options.AddDocumentTransformer<ApiVersionOpenApiDocumentTransformer>();
});

var identityOptions = builder.Services.AddOptions<IdentityServerOptions>()
    .Bind(builder.Configuration.GetSection(IdentityServerOptions.SectionName));

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

if (!CodeGeneration.IsRunningGeneration())
{
    // Some actions like validating application options must not be run during codegen activities, like OpenApi spec
    // generation or managing Entity Framework Core migrations.
    identityOptions
        .ValidateDataAnnotations()
        .ValidateOnStart();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.AddPreferredSecuritySchemes("OAuth2")
        .AddPasswordFlow("OAuth2", flow =>
        {
            flow.SelectedScopes = [Constants.POLICY_API_SCOPE_CLAIM];
        });
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.UseExceptionHandler();
app.UseStatusCodePages();

return await app.RunJasperFxCommands(args);