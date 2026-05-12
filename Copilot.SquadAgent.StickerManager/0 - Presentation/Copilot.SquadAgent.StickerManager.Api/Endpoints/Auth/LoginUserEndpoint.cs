using System.Diagnostics.CodeAnalysis;
using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Auth;

[ExcludeFromCodeCoverage]
public static class LoginUserEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/login", async (
            LoginUserRequest request,
            IUserAppService appService,
            CancellationToken cancellationToken) =>
        {
            return await appService.LoginAsync(request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<LoginUserRequest>>()
        .WithName("LoginUser")
        .WithSummary("Realiza login e retorna JWT")
        .WithTags("Auth")
        .WithOpenApi();
    }
}
