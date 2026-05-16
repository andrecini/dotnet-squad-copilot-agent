using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using FluentValidation;

namespace Copilot.SquadAgent.StickerManager.Api.Validators.Collection;

public class ImportCollectionRequestValidator : AbstractValidator<ImportCollectionRequest>
{
    private const string AllowedContentType = "text/csv";
    private const string AllowedExtension = ".csv";
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    public ImportCollectionRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("O arquivo CSV é obrigatório.");

        When(x => x.File is not null, () =>
        {
            RuleFor(x => x.File)
                .Must(f => f.Length > 0)
                .WithMessage("O arquivo CSV não pode estar vazio.")
                .Must(f => f.Length <= MaxFileSizeBytes)
                .WithMessage("O arquivo CSV não pode ultrapassar 5 MB.")
                .Must(f => string.Equals(Path.GetExtension(f.FileName), AllowedExtension, StringComparison.OrdinalIgnoreCase)
                           || f.ContentType.Equals(AllowedContentType, StringComparison.OrdinalIgnoreCase))
                .WithMessage("Apenas arquivos CSV são aceitos.");
        });
    }
}
