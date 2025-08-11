using app.Models;
using app.Models.Login;
using app.Models.TokenValidation;
using app.Helpers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace app.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IValidator<UserLoginRequest> _validator;

        public AuthService(AppDbContext context, IValidator<UserLoginRequest> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<UserLoginResult> LoginUserAsync(UserLoginRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return new UserLoginResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)),
                    JwtToken = null
                };
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new UserLoginResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Credenciais inválidas.",
                    JwtToken = null
                };
            }

            // Verifique o hash da senha!
            if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
            {
                return new UserLoginResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Credenciais inválidas.",
                    JwtToken = null
                };
            }

            // Crie a sessão
            var id = Guid.NewGuid();
            var jwtToken = JwtHelper.GenerateJwtToken(user, id);
            var session = new UserSession
            {
                Id = id,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                JwtToken = jwtToken
            };
            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            return new UserLoginResult
            {
                IsAuthenticated = true,
                JwtToken = jwtToken
            };
        }

        public TokenValidationResult ValidateJwtToken(TokenValidationRequest request)
        {
            var principal = JwtHelper.ValidateJwtToken(request.JwtToken);

            if (principal != null)
            {
                var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);
                return new TokenValidationResult
                {
                    IsValid = true,
                    Claims = claims
                };
            }
            else
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token inválido ou expirado."
                };
            }
        }
    }
}
