using System.Security.Claims;

namespace ApiGateway.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Json;
    using Microsoft.IdentityModel.Tokens;

    public class RoleClaimMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly ILogger<RoleClaimMiddleware> logger;

        public RoleClaimMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<RoleClaimMiddleware> logger)
        {
            _next = next;
            _config = configuration;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!authHeader.IsNullOrEmpty())
            {
                var tokenStr = authHeader.Split(" ")[1];

                if (ValidateToken(tokenStr))
                {
                    var handler = new JwtSecurityTokenHandler();

                    try
                    {
                        var jwtToken = handler.ReadJwtToken(tokenStr);

                        var claims = jwtToken.Claims
                            .Where(c => c.Type == "role")
                            .Select(c => new Claim(ClaimTypes.Role, c.Value))
                            .ToList();

                        if (claims.Any())
                        {
                            var identity = new ClaimsIdentity(claims, "HeaderBased");
                            context.User = new ClaimsPrincipal(identity);
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorResponse = new
                        {
                            error = "Access Restricted",
                            message = "Need permission for accessing the API."
                        };
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                        return;
                    }

                }
                else
                { 
                    var errorResponse = new
                    {
                        error = "Access Restricted",
                        message = "Need permission for accessing the API."
                    };
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                    return;
                }
            }




            await _next(context);
        }

        private bool ValidateToken(string token)
        {
            if (token == null)
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var name = jwtToken.Claims.First(x => x.Type == "unique_name").Value;
                if (name.IsNullOrEmpty())
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }




}
