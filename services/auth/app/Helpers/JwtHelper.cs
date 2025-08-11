using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using app.Models.Register;
using Microsoft.IdentityModel.Tokens;

namespace app.Helpers
{
    using Microsoft.Extensions.Configuration;
    public static class JwtHelper
    {
        public static string SecretKey { get; private set; } = string.Empty;
        public static string Issuer { get; private set; } = string.Empty;
        public static string Audience { get; private set; } = string.Empty;

        public static void Initialize(IConfiguration configuration)
        {
            SecretKey = configuration["JWT:SecretKey"] ?? throw new ArgumentNullException("JWT:SecretKey is not configured.");
            Issuer = configuration["JWT:Issuer"] ?? throw new ArgumentNullException("JWT:Issuer is not configured.");
            Audience = configuration["JWT:Audience"] ?? throw new ArgumentNullException("JWT:Audience is not configured.");
        }

        public static string GenerateJwtToken(User user, Guid sessionId, int expireHours = 8)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("sessionId", sessionId.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(expireHours),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Método opcional para validar tokens
        public static ClaimsPrincipal ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                // Token inválido
                return null;
            }
        }
    }
}