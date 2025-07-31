using System.Security.Claims;

namespace SARB_Reporting.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text.Json;

    public class RoleClaimMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleClaimMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/health")
            {
                await _next(context);
                return;
            }
            var authHeader = context.Request.Headers["Authorization"].ToString();

            var tokenStr = authHeader.Split(" ")[1];

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


            await _next(context);
        }
    }


}
