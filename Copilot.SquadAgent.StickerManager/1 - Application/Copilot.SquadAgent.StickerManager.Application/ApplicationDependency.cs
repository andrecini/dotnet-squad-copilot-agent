using Copilot.SquadAgent.StickerManager.Application.Services.User;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Copilot.SquadAgent.StickerManager.Application;

public static class ApplicationDependency
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(
            typeof(ApplicationDependency).Assembly,
            typeof(Copilot.SquadAgent.StickerManager.Domain.Mappings.UserProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(ApplicationDependency).Assembly);

        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
