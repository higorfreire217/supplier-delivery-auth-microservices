using app.Models;
using app.Models.Register;
using app.Helpers;
using FluentValidation;

namespace app.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly IValidator<UserRegistrationRequest> _validator;

        public UserService(AppDbContext context, IValidator<UserRegistrationRequest> validator)
        {
            _context = context;
            _validator = validator;

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
                PasswordHash = PasswordHasher.Hash(request.Password) // Ideal: gerar hash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserRegistrationResult
            {
                IsSuccess = true,
                ErrorMessage = null
            };
        }
    }
}