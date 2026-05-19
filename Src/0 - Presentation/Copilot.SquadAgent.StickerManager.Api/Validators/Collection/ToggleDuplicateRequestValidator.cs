using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Domain.Enums;
using FluentValidation;

namespace Copilot.SquadAgent.StickerManager.Api.Validators.Collection;

public class ToggleDuplicateRequestValidator : AbstractValidator<ToggleDuplicateRequest>
{
    public ToggleDuplicateRequestValidator()
    {
        RuleFor(x => x.Action)
            .IsInEnum()
            .WithMessage("A ação deve ser 'Mark' ou 'Unmark'.");
    }
}
