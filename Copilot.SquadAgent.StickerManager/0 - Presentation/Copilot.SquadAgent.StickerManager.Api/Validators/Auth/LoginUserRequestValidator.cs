using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using FluentValidation;

namespace Copilot.SquadAgent.StickerManager.Api.Validators.Auth;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("E-mail inválido.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatória.");
    }
}
