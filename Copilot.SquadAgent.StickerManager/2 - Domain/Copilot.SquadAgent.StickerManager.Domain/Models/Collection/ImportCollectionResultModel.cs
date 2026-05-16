using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class ImportCollectionResultModel
{
    public int Imported { get; set; }
    public int Failed { get; set; }
    public IReadOnlyList<ImportCollectionErrorModel> Errors { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public class ImportCollectionErrorModel
{
    public int Line { get; set; }
    public string Reason { get; set; } = string.Empty;
}
