using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SGP.Domain.Repositories;
using SGP.Infrastructure.Data;
using SGP.Infrastructure.Data.Repositories.Cached;
using SGP.Infrastructure.Services;
using SGP.Shared.Interfaces;

namespace SGP.Infrastructure;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        => services
            .AddScoped<ICacheService, MemoryCacheService>()
            .AddScoped<IDateTimeService, DateTimeService>()
            .AddScoped<IHashService, BCryptHashService>()
            .AddScoped<ITokenClaimsService, JwtClaimService>()
            .AddScoped<IUnitOfWork, UnitOfWork>();

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
               .Scan(scan => scan
                   .FromCallingAssembly()
                   .AddClasses(classes => classes.AssignableTo<IRepository>())
                   .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                   .AsImplementedInterfaces()
                   .WithScopedLifetime());

        // The decorator pattern
        // REF: https://andrewlock.net/adding-decorated-classes-to-the-asp.net-core-di-container-using-scrutor/
        services
            .Decorate<ICidadeRepository, CidadeCachedRepository>()
            .Decorate<IEstadoRepository, EstadoCachedRepository>()
            .Decorate<IRegiaoRepository, RegiaoCachedRepository>();

        return services;
    }
}