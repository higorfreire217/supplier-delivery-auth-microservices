using app.Models;
using app.Models.Register;
using app.Models.Login;
using FluentValidation;

namespace app.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly IValidator<UserRegistrationRequest> _validator;
        private readonly IValidator<UserLoginRequest> _loginValidator;

        public UserService(AppDbContext context, IValidator<UserRegistrationRequest> validator, IValidator<UserLoginRequest> loginValidator)
        {
            _context = context;
            _validator = validator;
            _loginValidator = loginValidator;

        }

        public async Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return new UserRegistrationResult
                {
                    IsSuccess = false,
                    ErrorMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return new UserRegistrationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Este email já está em uso."
                };
            }

            // Registre o usuário na base
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = request.Password // Ideal: gerar hash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserRegistrationResult
            {
                IsSuccess = true,
                ErrorMessage = null
            };
        }

        public async Task<UserLoginResult> LoginUserAsync(UserLoginRequest request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);
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

            // Verifique a senha (ideal: hash/salt)
            if (user.PasswordHash != request.Password)
            {
                return new UserLoginResult
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Credenciais inválidas.",
                    JwtToken = null
                };
            }

            // Gerar token (exemplo simplificado)
            var jwtToken = JwtHelper.GenerateJwtToken(user);

            return new UserLoginResult
            {
                IsAuthenticated = true,
                ErrorMessage = null,
                JwtToken = jwtToken
            };
        }
    }
}