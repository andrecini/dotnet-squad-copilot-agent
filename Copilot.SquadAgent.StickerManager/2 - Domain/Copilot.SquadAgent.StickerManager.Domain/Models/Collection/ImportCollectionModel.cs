using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class ImportCollectionModel
{
    public Guid UserId { get; set; }
    public IReadOnlyList<CsvStickerRowModel> Rows { get; set; } = [];
}
