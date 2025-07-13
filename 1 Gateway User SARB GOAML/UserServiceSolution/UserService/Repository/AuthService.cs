using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Model;

namespace UserService.Repository
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

        public async Task<AuthResultDto?> LoginAsync(LoginRequestDto req)
        {
            // 1️⃣  Validate credentials in DB
            var user = await _repo.SignInAsync(req.Username, req.Password);
            if (user is null) return null;

            // 2️⃣  Build claims (Name, Roles, custom perms, etc.)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,  user.LoginName),
                //new Claim("uid",            user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var g in user.Group)
                foreach (var r in g.Role)
                    claims.Add(new Claim(ClaimTypes.Role, r.RoleName));

            //foreach (var g in user.Group)
            //    foreach (var r in g.Role)
            //        foreach (var p in r.Permission)
            //            claims.Add(new Claim("perm", p.PermissionType));

            // 3️⃣  Issue the JWT
            string token = GenerateToken(claims);

            // 4️⃣  Return both objects together
            return new AuthResultDto(token, user);
        }


        private string GenerateToken(IEnumerable<Claim> claims)
        {
            //var keyBytes = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            //var jwt = new JwtSecurityToken(

            //    issuer: _configuration["Jwt:Issuer"],
            //    audience: _configuration["Jwt:Audience"],
            //    claims: claims,
            //    expires: DateTime.UtcNow.AddHours(3),
            //    signingCredentials: new SigningCredentials(keyBytes, SecurityAlgorithms.HmacSha256));

            //return new JwtSecurityTokenHandler().WriteToken(jwt);

            //---------------------------Both OF These Code Works------------------------------------

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddHours(0.1),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
