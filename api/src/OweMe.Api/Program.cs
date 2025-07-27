using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using OweMe.Api.Controllers;
using OweMe.Api.Description;
using OweMe.Api.Identity;
using OweMe.Api.Identity.Configuration;
using OweMe.Application;
using OweMe.Infrastructure;
using OweMe.Persistence;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddDocumentTransformer<ApiVersionOpenApiDocumentTransformer>();
});

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(logger);
builder.Services.AddSerilog(logger);

builder.Services.AddControllers();

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

await app.RunAsync();