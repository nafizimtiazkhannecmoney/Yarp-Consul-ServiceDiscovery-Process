using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGateway.Configurations;
using ApiGateway.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load Serilog configuration from appsettings.json
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext());

#region Consul service-discovery wiring
// ---------------------------------------------------------------------------
//  ConsulConfig implements IProxyConfigProvider *and* IHostedService.
//  1. Register one singleton instance.
//  2. Expose it to YARP via IProxyConfigProvider for dynamic routes/clusters.
//  3. Run it as a background task so it polls Consul on a schedule.
// ---------------------------------------------------------------------------
#endregion
builder.Services.AddSingleton<ConsulConfig>();
builder.Services.AddSingleton<IProxyConfigProvider>(p => p.GetRequiredService<ConsulConfig>());
builder.Services.AddHostedService(p => p.GetRequiredService<ConsulConfig>());


#region Health-check & gateway registration
// ---------------------------------------------------------------------------
//  • Health-checks endpoint    =>  /health  (used by Consul).
//  • GatewayConsulRegistration =>  registers this gateway instance in Consul
//    so other services know where to send traffic.
// ---------------------------------------------------------------------------
#endregion
builder.Services.AddHealthChecks();
builder.Services.AddHostedService<GatewayConsulRegistration>();

#region YARP reverse-proxy
// ---------------------------------------------------------------------------
//  Add the core YARP middleware. Actual route/cluster definitions come
//  from ConsulConfig (registered above) or from appsettings.json if present.
// ---------------------------------------------------------------------------
#endregion
builder.Services.AddReverseProxy();

var app = builder.Build();

app.UseMiddleware<RoleClaimMiddleware>(); // <---- your custom middleware
app.UseAuthorization();
// Health-check probe (Consul / k8s / Docker)
app.MapHealthChecks("/health");

// -----------------------------------------------------------
//  Diagnostic interceptor: logs every request passing through
//  the gateway so you can see which downstream service YARP
//  selected. Remove or switch to a proper logger in production.
// -----------------------------------------------------------
app.Use(async (context, next) =>
{
    var method = context.Request.Method;
    var originalPath = context.Request.Path;
    var targetHost = context.Request.Headers["Host"].FirstOrDefault() ?? "unknown";

    Console.WriteLine("-------------------------------------------------------");
    Console.WriteLine($" [Gateway Routing]");
    Console.WriteLine($" Incoming Request : {method} {originalPath}");
    Console.WriteLine($" Target Service   : {targetHost}");
    Console.WriteLine("-------------------------------------------------------");

    await next(); // Continue to YARP
});
 
// Wire up YARP – must come *after* any auth/diagnostic middleware
app.MapReverseProxy();

// Simple root endpoint to verify the gateway is alive (optional)
app.MapGet("/", () => "ApiGateway Running...");  
app.Run();


#region User Manual For Consul
//============================================================
// Run Consul - consul agent -dev -ui

// Generate Token From Gateway 
// POST : http://localhost:5000/api/authentication/login   
//{
//  "username": "test@necmoney.co.za",
//  "password": "123"
//}

// Access SARB Reporting with the JWT Token
// GET : http://localhost:5000/api/Sarb/GetTestData 

// Access SAGOAML Reporting with the JWT Token
// GET : http://localhost:5000/api/GoAML/GetTestData

// Access GET_ALL_USER with the JWT Token
// GET : http://localhost:5000/api/user/GET_ALL_USER

// Access GET_USER_BY_ID with the JWT Token
// GET : http://localhost:5000/api/user/GET_USER_BY_ID/100004

//============================================================

// To add JWT Auth --> Keep thesame key in all the projects in appsettings, keep the same settings of jwt in all program.cs

// 1. Run Consul with the following command:
//    This will start the Consul agent in development mode and open the UI at http://localhost:8500
//    Command: 
//    "consul agent -dev -ui"

//    Key = Authorization, Value = Bearer <JWT_Token>
//    Authorization -- Auth Type - Bearer Token - Token <JWT_Token>


//    Testing the JWT Token on jwt.io
//    You can test your JWT token on jwt.io to verify its contents:
//    - Copy the JWT token obtained after registering the admin or user
//    - Go to https://jwt.io/ and paste the token in the "Encoded" section to see its contents.

// Test this endpoint https://localhost:5010/WeatherForecast (GET) With The Jtw Token
#endregion

#region Other
// Test - jwt.io
// First run Consul with this command "consul agent -dev -ui"
// Consul ui will be available at http://localhost:8500
#endregion
