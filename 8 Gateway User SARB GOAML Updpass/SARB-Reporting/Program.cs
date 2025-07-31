using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SARB_Reporting;
using SARB_Reporting.Helpers;
using SARB_Reporting.Services;
using SARB_Reporting.Utils;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog using dependency injection
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add these 2 lines for Consul
builder.Services.AddHealthChecks();
builder.Services.AddHostedService<ConsulRegistration>();

builder.Services.AddScoped<DbContext>();
builder.Services.AddScoped<NecAppConfig>();
builder.Services.AddScoped<SarbDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging() )
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<RoleClaimMiddleware>(); // <---- your custom middleware
app.UseAuthorization();
// Add this line
app.MapHealthChecks("/health");

app.MapControllers();
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
