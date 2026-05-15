using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Collection;

[ExcludeFromCodeCoverage]
public static class ToggleDuplicateEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/v1/collection/{id:guid}/duplicate", async (
            Guid id,
            ToggleDuplicateRequest request,
            ClaimsPrincipal principal,
            ICollectionAppService appService,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            return await appService.ToggleDuplicateAsync(userId, id, request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<ToggleDuplicateRequest>>()
        .WithName("ToggleDuplicate")
        .WithSummary("Marca ou desmarca uma figurinha da coleção como duplicata")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}
