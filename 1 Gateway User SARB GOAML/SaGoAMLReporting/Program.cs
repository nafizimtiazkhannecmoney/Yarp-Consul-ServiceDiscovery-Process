using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SaGoAMLReporting;
using SaGoAMLReporting.Service;
using SaGoAMLReporting.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//  JWT 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Use JwtBearerDefaults.AuthenticationScheme instead of "Bearer"
   .AddJwtBearer(o =>
   {
       o.TokenValidationParameters = new()
       {
           ValidateIssuer = false,
           ValidateAudience = false,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ClockSkew = TimeSpan.Zero,
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
           ValidAudience = builder.Configuration["Jwt:Audience"],
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
       };
   });

builder.Services.AddAuthorization();

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

app.UseAuthorization();

// Add this line
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
