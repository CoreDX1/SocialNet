using FluentValidation;
using SocialNet.Application.DTOs.Message;

namespace SocialNet.Application.Validators;

public class SendMessageValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.ReceiverId).NotEmpty();
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
