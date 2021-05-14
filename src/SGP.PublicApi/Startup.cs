using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SGP.Application;
using SGP.GraphQL;
using SGP.Infrastructure;
using SGP.Infrastructure.Migrations;
using SGP.PublicApi.Extensions;

namespace SGP.PublicApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddHttpContextAccessor();

            services.AddResponseCompression();

            services.AddApiVersioningAndApiExplorer();

            services.AddOpenApi();

            services.AddJwtBearer(Configuration);

            services.AddServices();

            services.AddInfrastructure();

            services.ConfigureAppSettings(Configuration);

            var healthChecksBuilder = services.AddHealthChecks();

            services.AddDbContext(Configuration, healthChecksBuilder);

            services.AddGraphQLWithSchemas();

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.AddServerHeader = false;
            });

            services.Configure<IISServerOptions>(options
                => options.AllowSynchronousIO = true);

            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressMapClientErrors = true;
                    options.SuppressModelStateInvalidFilter = true;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.None;
                    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters = new[] { new StringEnumConverter(new CamelCaseNamingStrategy()) };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IMapper mapper,
            IApiVersionDescriptionProvider apiVersionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
            mapper.ConfigurationProvider.CompileMappings();

            app.UseOpenApi(apiVersionProvider);

            app.UseHealthChecks();

            app.UseHttpsRedirection();

            app.UseGraphQL();

            app.UseHsts();

            app.UseRouting();

            app.UseResponseCompression();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
            });

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
