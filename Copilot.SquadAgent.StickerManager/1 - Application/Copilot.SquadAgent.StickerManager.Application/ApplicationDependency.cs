using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Copilot.SquadAgent.StickerManager.Application;

public static class ApplicationDependency
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationDependency).Assembly);
        services.AddValidatorsFromAssembly(typeof(ApplicationDependency).Assembly);

        return services;
    }
}
