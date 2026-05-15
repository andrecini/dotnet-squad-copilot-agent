using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class GetMissingStickersEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/collection/missing", async (
            string? sort,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            return await appService.ListMissingStickersAsync(userId, sort, cancellationToken);
        })
        .WithName("GetMissingStickers")
        .WithSummary("Lista todas as figurinhas que o usuário autenticado ainda não possui")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}
