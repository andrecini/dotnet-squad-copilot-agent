using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class GetTeamProgressEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/album/{teamId:guid}/progress", async (
            Guid teamId,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            return await appService.GetTeamProgressAsync(userId, teamId, cancellationToken);
        })
        .WithName("GetTeamProgress")
        .WithSummary("Retorna o progresso de conclusão do álbum para um time específico")
        .WithTags("Album")
        .RequireAuthorization()
        .WithOpenApi();
    }
}
