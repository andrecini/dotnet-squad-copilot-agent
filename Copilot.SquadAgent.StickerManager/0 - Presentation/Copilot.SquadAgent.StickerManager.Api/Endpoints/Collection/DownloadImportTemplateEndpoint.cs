using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class DownloadImportTemplateEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/collection/import/template", async (
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            return await appService.DownloadImportTemplateAsync(cancellationToken);
        })
        .WithName("DownloadImportTemplate")
        .WithSummary("Baixa o template CSV para importação de figurinhas em lote")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}
