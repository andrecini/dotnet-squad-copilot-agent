using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.Queries;
using FluentValidation;

namespace Copilot.SquadAgent.StickerManager.Api.Validators.Collection;

public class CollectionQueryRequestValidator : AbstractValidator<CollectionQueryRequest>
{
    private static readonly string[] AllowedSortValues = ["number", "player_name", "acquired_at"];

    public CollectionQueryRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 500)
            .WithMessage("O limite deve estar entre 1 e 500.");

        RuleFor(x => x.Sort)
            .Must(sort => sort is null || AllowedSortValues.Contains(sort))
            .WithMessage($"O campo sort deve ser um dos valores: {string.Join(", ", AllowedSortValues)}.");
    }
}
