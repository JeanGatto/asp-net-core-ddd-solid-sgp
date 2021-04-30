using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SGP.Infrastructure.Context;
using SGP.Shared.AppSettings;
using System;

namespace SGP.Infrastructure.Migrations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services,
            IConfiguration configuration, IHealthChecksBuilder healthChecksBuilder)
        {
            Guard.Against.Null(services, nameof(services));
            Guard.Against.Null(configuration, nameof(configuration));
            Guard.Against.Null(healthChecksBuilder, nameof(healthChecksBuilder));

            // Obtendo o tipo de ambiente.
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Obtendo a string de conexão.
            var connectionString = configuration.GetConnectionString(
                nameof(ConnectionStrings.DefaultConnection));

            services.AddDbContext<SgpContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(connectionString, sqlServerBuilder
                    => sqlServerBuilder.MigrationsAssembly(MigrationsAssembly.Name));

                // Configurando para exibir os errados mais detalhados.
                if (environment == Environments.Development)
                {
                    optionsBuilder.EnableDetailedErrors();
                    optionsBuilder.EnableSensitiveDataLogging();
                }
            });

            // Verificador de saúde da base de dados.
            healthChecksBuilder.AddDbContextCheck<SgpContext>(
                tags: new[] { "database" },
                customTestQuery: (context, cancellationToken)
                    => context.Cidades.AsNoTracking().AnyAsync(cancellationToken));

            return services;
        }
    }
}