using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain;

[ExcludeFromCodeCoverage]
public static class DomainDependency
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services;
    }
}
