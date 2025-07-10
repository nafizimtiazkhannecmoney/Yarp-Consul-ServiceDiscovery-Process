using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGateway.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

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


#region JWT authentication / authorization
// ---------------------------------------------------------------------------
//  Authentication:
//    • Use the default “Bearer” scheme provided by JwtBearerDefaults.
//    • Validate signature + expiry; issuer/audience checks can be re-enabled
//      (set ValidateIssuer / ValidateAudience = true) once all clients
//      issue properly-stamped tokens.
//
//  Authorization:
//    • Add a simple policy (“Authenticated”) that requires a *valid* token.
//      YARP routes can reference this policy via `AuthorizationPolicy`.
// ---------------------------------------------------------------------------
#endregion
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Use JwtBearerDefaults.AuthenticationScheme instead of "Bearer"
   .AddJwtBearer(o =>
   {
       o.TokenValidationParameters = new()
       {
           ValidateIssuer = false,
           ValidateAudience = false,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           //ClockSkew = TimeSpan.Zero,
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
           ValidAudience = builder.Configuration["Jwt:Audience"],
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
       };
   });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", p => p.RequireAuthenticatedUser());
});



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
// Test - JWT Authentication and API Access

// To add JWT Auth --> Keep thesame key in all the projects in appsettings, keep the same settings of jwt in all program.cs

// 1. Run Consul with the following command:
//    This will start the Consul agent in development mode and open the UI at http://localhost:8500
//    Command: 
//    "consul agent -dev -ui"

// 2. Register the admin user
//    First, send a POST request to register the admin user:
//    POST http://localhost:5000/api/identity/admin@admin.com/Admin@123
//    A success message will be returned after registration.

// 3. Obtain JWT Token for Admin
//    Send a POST request to the same endpoint again to get the JWT token:
//    POST http://localhost:5000/api/identity/admin@admin.com/Admin@123
//    You will receive a JWT token in the response.

// 4. Access Product API with Admin Token
//    Once you have the JWT token, copy it and include it in the Authorization header
//    for accessing the product API. Use the following GET request:
//    GET http://localhost:5000/api/product
//    In the request headers, set:
//    Key = Authorization, Value = Bearer <JWT_Token>
//    As an Admin, you will be able to access the product API.

// 5. Register Regular User
//    Similarly, to register a regular user, send a POST request:
//    POST http://localhost:5000/api/identity/user@user.com/User@123
//    A success message will be returned after registration.

// 6. Obtain JWT Token for User
//    Send a POST request to the same endpoint again to get the JWT token for the user:
//    POST http://localhost:5000/api/identity/user@user.com/User@123
//    You will receive a JWT token in the response.

// 7. Access Order API with User Token
//    After obtaining the user’s JWT token, copy it and include it in the Authorization header
//    for accessing the order API. Use the following GET request:
//    GET http://localhost:5000/api/order
//    In the request headers, set:
//    Key = Authorization, Value = Bearer <JWT_Token>
//    As a regular user, you will have access to the order API.

// 8. API Access Control Based on Roles
//    - Admin: Can access the product API
//    - User: Can access the order API

// 9. Testing the JWT Token on jwt.io
//    You can test your JWT token on jwt.io to verify its contents:
//    - Copy the JWT token obtained after registering the admin or user
//    - Go to https://jwt.io/ and paste the token in the "Encoded" section to see its contents.

// 10. Example API Endpoints:
//    - Product API (Admin only):
//      GET http://localhost:5000/api/product
//    - Order API (User only):
//      GET http://localhost:5000/api/order
//    - Get Specific Order (User only):
//      GET http://localhost:5000/api/order/3
//    - Create a New Order (User only):
//      POST http://localhost:5000/api/order

// 6.  SARB-Reporting – Smoke test endpoint
//
//     6.1  Test data (no DB hit, no external call)
//
//          GET http://localhost:5000/api/sarb/GetTestData
//          Headers:
//             Authorization: Bearer  ADMIN_TOKEN
//
//            200 OK
//             ["FROM SARB","sad"]
//
//          *Using USER_TOKEN returns 403 because route is Admin-only.*


// GET  http://localhost:5000/api/sarb/GetTestData
// GET  http://localhost:5000/api/goaml/GetTestData
// POST http://localhost:5000/api/authentication/login     (Body > Raw )
//{
//    "username": "admin",
//    "password": "password"
//}

// Test this endpoint https://localhost:5010/WeatherForecast (GET) With The Jtw Token


#endregion
#region Other
// Test - jwt.io

// First run Consul with this command "consul agent -dev -ui"
// Consul ui will be available at http://localhost:8500

// First register a user with the following endpoint:
// http://localhost:5000/api/identity/admin@admin.com/Admin@123  --> Register First Then You will see Success Message  (POST)
// http://localhost:5000/api/identity/admin@admin.com/Admin@123  --> Hit this again to get the token                   (POST)  

// Now Copy The Token and use it in http://localhost:5000/api/product
// and in Headers -> Key = Authorization, Value = Bearer <token>
// As Admin Can Access Product, you can access the product API with the token.

// http://localhost:5000/api/identity/user@user.com/User@123  --> Register First Then You will see Success Message     (POST)
// http://localhost:5000/api/identity/user@user.com/User@123  --> Hit this again to get the token                      (POST)

// Now Copy The Token and use it in http://localhost:5000/api/order
// and in Headers -> Key = Authorization, Value = Bearer <token>
// As User Can Access Order, you can access the Order API with the token.



// http://localhost:5000/api/product  routes to ProductApi

// http://localhost:5000/api/order    routes to OrderApi.

// GET http://localhost:5000/api/order/3 

// POST http://localhost:5000/api/order 


// Test - jwt.io
// http://localhost:5000/api/identity/admin@admin.com/Admin@123  --> Register First Then You will see Success Message
// http://localhost:5000/api/identity/admin@admin.com/Admin@123  --> Hit this again to get the token 
// Now Copy The Token adn use it in jwt.io and check it
// Now go to http://localhost:5000/api/product (will show unauthorized if not used  token)
// and in Heareds -> Key = Authorization, Value = Bearer <token>
// User can access order
// Admin can acces product
#endregion