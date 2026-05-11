using Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.Filters;

namespace Copilot.SquadAgent.StickerManager.Api.Endpoints.Auth;

public static class RegisterUserEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/register", async (
            RegisterUserRequest request,
            IUserAppService appService,
            CancellationToken cancellationToken) =>
        {
            return await appService.RegisterAsync(request, cancellationToken);
        })
        .AddEndpointFilter<ValidationFilter<RegisterUserRequest>>()
        .WithName("RegisterUser")
        .WithSummary("Cadastra um novo usuário")
        .WithTags("Auth")
        .WithOpenApi();
    }
}
