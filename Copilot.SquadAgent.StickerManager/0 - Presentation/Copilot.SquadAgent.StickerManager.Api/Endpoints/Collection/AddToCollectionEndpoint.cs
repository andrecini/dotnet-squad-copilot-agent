using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class AddToCollectionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/collection", async (
            AddToCollectionRequest request,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            return await appService.AddStickerAsync(userId, request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<AddToCollectionRequest>>()
        .WithName("AddToCollection")
        .WithSummary("Adiciona uma figurinha à coleção do usuário autenticado")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}
