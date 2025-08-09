using FluentValidation;
using app.Models.Register;

namespace app.Validators
{
    public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
    {
        public UserRegistrationRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres.")
                .MaximumLength(50).WithMessage("Nome deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório.")
                .EmailAddress().WithMessage("Email inválido.")
                .MinimumLength(8).WithMessage("Email deve ter pelo menos 8 caracteres.")
                .MaximumLength(99).WithMessage("Email deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatória.")
                .MinimumLength(8).WithMessage("Senha deve ter pelo menos 8 caracteres.")
                .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres.");
        }
    }
}