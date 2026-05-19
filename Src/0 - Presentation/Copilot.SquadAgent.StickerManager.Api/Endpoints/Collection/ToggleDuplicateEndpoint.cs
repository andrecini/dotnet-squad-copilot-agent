using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;
using Copilot.SquadAgent.StickerManager.Api.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

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
            var userIdResult = AuthorizationHelper.GetUserIdFromClaims(principal);

            if (userIdResult.IsFailure) return Results.Unauthorized();

            return await appService.ToggleDuplicateAsync(userIdResult.Value, id, request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<ToggleDuplicateRequest>>()
        .WithName("ToggleDuplicate")
        .WithSummary("Marca ou desmarca uma figurinha da coleção como duplicata")
        .WithTags("Collection")
        .RequireAuthorization()
        .WithOpenApi();
    }
}
