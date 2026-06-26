using FluentValidation;
using SocialNet.Application.DTOs.Comment;

namespace SocialNet.Application.Validators;

public class CreateCommentValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(300);
    }
}
