using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using FluentValidation;

namespace Copilot.SquadAgent.StickerManager.Api.Validators.Auth;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("E-mail inválido.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("A senha deve conter ao menos uma letra maiúscula.")
            .Matches(@"\d").WithMessage("A senha deve conter ao menos um número.");
    }
}
