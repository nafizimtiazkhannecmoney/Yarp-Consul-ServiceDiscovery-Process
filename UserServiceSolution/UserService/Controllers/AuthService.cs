using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Model;
using UserService.Repository;

namespace UserService.Controllers
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserRepository _repo;
        public AuthService(IConfiguration configuration, UserRepository repo)
        {
            _configuration = configuration;
            _repo = repo;

        }
        
        public async Task<(int, string)> Login(LoginRequestDto model)
        {
            var user = await _repo.SignInAsync(model.Username, model.Password);
            if (user == null)
                return (0, "Invalid login credentials!");

            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, model.Username),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var g in user.Group)
                foreach (var r in g.Role)
                    authClaims.Add(new Claim(ClaimTypes.Role, r.RoleName));

            string token = GenerateToken(authClaims);
            return (1, token);
        }


        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
