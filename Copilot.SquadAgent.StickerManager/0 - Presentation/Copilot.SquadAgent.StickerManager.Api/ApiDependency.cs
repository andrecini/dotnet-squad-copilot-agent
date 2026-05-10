using Microsoft.Extensions.DependencyInjection;

namespace Copilot.SquadAgent.StickerManager.Api;

public static class ApiDependency
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        return services;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        return app;
    }
}
