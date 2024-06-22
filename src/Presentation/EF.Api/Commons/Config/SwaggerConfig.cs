using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EF.Api.Commons.Config;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Easy Food - Pedido", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description =
                    "Insira o token JWT com o prefixo 'Bearer '. O cliente tem a opção de se autenticar ou não, porém é obrigatório o envio de um access token para identificação do carrinho.",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });

            if (!env.IsDevelopment())
            {
                c.DocumentFilter<PrefixDocumentFilter>();
            }
        });

        return services;
    }

    public static WebApplication UseSwaggerConfig(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}

public class PrefixDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = new OpenApiPaths();

        foreach (var path in swaggerDoc.Paths)
        {
            paths.Add("/pedido" + path.Key, path.Value);
        }

        swaggerDoc.Paths = paths;
    }
}