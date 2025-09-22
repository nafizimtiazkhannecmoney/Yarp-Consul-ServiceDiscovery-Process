using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SaGoAMLReporting;
using SaGoAMLReporting.Service;
using SaGoAMLReporting.Service.Interfaces;
using SARB_Reporting.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add these 2 lines for Consul
builder.Services.AddHealthChecks();
builder.Services.AddHostedService<ConsulRegistration>();

builder.Services.AddScoped<IGoAMLDataService, GoAMLDataService>();
builder.Services.AddScoped<ISarbDataService, SarbDataService>();
builder.Services.AddScoped<ISqlService, SqlService>();
builder.Services.AddScoped<IValidateXML, ValidateXML>();

var app = builder.Build();

var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration?["Logging:LogFilePath"]?.ToString());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging())
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

app.Run();
