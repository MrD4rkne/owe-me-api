using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using OweMe.Api.Configuration.Identity;
using Scalar.AspNetCore;
using Serilog;
using OweMe.Application;
using OweMe.Infrastructure;
using OweMe.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(logger);
builder.Services.AddSerilog(logger);

builder.Services.AddControllers();

builder.Services.Configure<IdentityServerOptions>(builder.Configuration.GetSection(IdentityServerOptions.SECTION_NAME));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();

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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .RequireAuthorization(Constants.POLICY_API_SCOPE);

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(builder =>
{
    builder.MapControllers()
        .RequireAuthorization(Constants.POLICY_API_SCOPE);
});

await app.RunAsync();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record Test(string Name){}