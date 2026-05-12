using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using FluentValidation;

namespace Copilot.SquadAgent.StickerManager.Api.Validators.User;

public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("E-mail inválido.")
            .When(x => x.Email is not null);

        RuleFor(x => x.Name)
            .MinimumLength(2)
            .WithMessage("O nome deve ter ao menos 2 caracteres.")
            .MaximumLength(256)
            .WithMessage("O nome deve ter no máximo 256 caracteres.")
            .When(x => x.Name is not null);

        RuleFor(x => x)
            .Must(x => x.Email is not null || x.Name is not null)
            .WithName("request")
            .WithMessage("Ao menos um campo (email ou nome) deve ser informado.");
    }
}
