using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;
using Copilot.SquadAgent.StickerManager.Domain.Enums;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class ListCollectionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/collection", async (
            [AsParameters] CollectionQueryRequest query,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            return await appService.ListCollectionAsync(userId, query, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<CollectionQueryRequest>>()
        .WithName("ListCollection")
        .WithSummary("Lista a coleção do usuário autenticado com filtros e paginação")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}
