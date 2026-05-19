using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using FluentValidation;

namespace Copilot.SquadAgent.StickerManager.Api.Validators.Collection;

public class AddToCollectionRequestValidator : AbstractValidator<AddToCollectionRequest>
{
    public AddToCollectionRequestValidator()
    {
        RuleFor(x => x.StickerId)
            .NotEmpty()
            .WithMessage("O identificador da figurinha é obrigatório.");
    }
}
