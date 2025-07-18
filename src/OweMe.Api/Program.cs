using Microsoft.AspNetCore.Authentication.JwtBearer;
using OweMe.Api;
using OweMe.Api.Controllers;
using OweMe.Api.Identity;
using OweMe.Api.Identity.Configuration;
using OweMe.Application;
using OweMe.Infrastructure;
using OweMe.Persistence;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

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

app.MapLedgersEndpoints();

await app.RunAsync();