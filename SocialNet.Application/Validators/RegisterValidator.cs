using FluentValidation;
using SocialNet.Application.DTOs.Auth;

namespace SocialNet.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MinimumLength(3).WithMessage("Mínimo 3 caracteres.")
            .MaximumLength(30).WithMessage("Máximo 30 caracteres.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Solo letras, números y guión bajo.");

        // RuleFor(x => x.Email)
        //     .NotEmpty().WithMessage("El email es obligatorio.")
        //     .EmailAddress().WithMessage("Formato de email inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("Mínimo 8 caracteres.")
            .Matches("[A-Z]").WithMessage("Debe contener al menos una mayúscula.")
            .Matches("[0-9]").WithMessage("Debe contener al menos un número.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("El nombre visible es obligatorio.")
            .MaximumLength(50);
    }
}
