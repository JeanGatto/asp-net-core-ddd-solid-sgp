using System;
using System.Reflection;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SGP.Infrastructure.Context;
using SGP.Shared.AppSettings;

namespace SGP.Infrastructure.Migrations
{
    public static class ServicesCollectionExtensions
    {
        private static readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        public static IServiceCollection AddDbContext(this IServiceCollection services,
            IHealthChecksBuilder healthChecksBuilder)
        {
            Guard.Against.Null(healthChecksBuilder, nameof(healthChecksBuilder));

            services.AddDbContext<SgpContext>((provider, builder) =>
            {
                builder.UseSqlServer(provider.GetConnectionString(),
                    options => options.MigrationsAssembly(AssemblyName));

                // NOTE: Quando for ambiente de desenvolvimento será logado informações detalhadas.
                var environment = provider.GetRequiredService<IHostEnvironment>();
                if (environment.IsDevelopment()) builder.EnableDetailedErrors().EnableSensitiveDataLogging();
            });

            // Verificador de saúde da base de dados.
            healthChecksBuilder.AddDbContextCheck<SgpContext>(
                tags: new[] { "database" },
                customTestQuery: (context, cancellationToken) =>
                    context.Estados.AsNoTracking().AnyAsync(cancellationToken));

            return services;
        }

        private static string GetConnectionString(this IServiceProvider provider)
            => provider.GetRequiredService<IOptions<ConnectionStrings>>().Value.DefaultConnection;
    }
}