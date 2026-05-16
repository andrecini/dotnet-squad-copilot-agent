using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class ImportCollectionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/collection/import", async (
            IFormFile file,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var request = new ImportCollectionRequest { File = file };
            return await appService.ImportCollectionAsync(userId, request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<ImportCollectionRequest>>()
        .WithName("ImportCollection")
        .WithSummary("Importa figurinhas em lote via arquivo CSV para a coleção do usuário autenticado")
        .WithTags("Collection")
        .RequireAuthorization()
        .DisableAntiforgery()
        .WithOpenApi();
    }
}
