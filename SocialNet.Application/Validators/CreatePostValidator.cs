using FluentValidation;
using SocialNet.Application.DTOs.Post;

namespace SocialNet.Application.Validators;

public class CreatePostValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("El contenido no puede estar vacío.")
            .MaximumLength(500)
            .WithMessage("Máximo 500 caracteres.");
    }
}
