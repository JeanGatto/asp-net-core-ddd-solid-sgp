using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SGP.PublicApi.Options;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        // Add a swagger document for each discovered API version.
        // NOTE: you might choose to skip or document deprecated API versions differently.
        foreach (var description in _provider.ApiVersionDescriptions)
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var openApiInfo = new OpenApiInfo
        {
            Title = "Sistema Gerenciador de Pedidos (SGP)",
            Description = "ASP.NET Core C# REST API, DDD, Princípios SOLID e Clean Architecture",
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact
            {
                Name = "Jean Gatto",
                Email = "jean_gatto@hotmail.com",
#pragma warning disable S1075
                Url = new Uri("https://www.linkedin.com/in/jeangatto/")
#pragma warning restore S1075
            },
            License = new OpenApiLicense
            {
                Name = "MIT License",
#pragma warning disable S1075
                Url = new Uri("https://github.com/jeangatto/ASP.NET-Core-API-DDD-SOLID/blob/main/LICENSE")
#pragma warning restore S1075
            }
        };

        if (description.IsDeprecated)
            openApiInfo.Description += " - Esta versão da API foi descontinuada.";

        return openApiInfo;
    }
}
