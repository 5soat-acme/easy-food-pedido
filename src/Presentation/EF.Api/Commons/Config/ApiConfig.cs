using System.Text.Json.Serialization;
using EF.Api.Commons.Extensions;
using EF.Api.Contexts.Carrinho.Config;
using EF.Api.Contexts.Cupons.Config;
using EF.Api.Contexts.Estoques.Config;
using EF.Api.Contexts.Pedidos.Config;
using EF.Api.Contexts.Produtos.Config;
using EF.Pedidos.Application.Events.Consumers;
using EF.WebApi.Commons.Identity;
using EF.WebApi.Commons.Users;

namespace EF.Api.Commons.Config;

public static class ApiConfig
{
    public static IServiceCollection AddApiConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfig();

        services.AddEventBusConfig();

        services.RegisterServicesCarrinho(configuration);
        services.RegisterServicesCupons(configuration);
        services.RegisterServicesEstoques(configuration);
        services.RegisterServicesPedidos(configuration);
        services.RegisterServicesProdutos(configuration);

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IUserApp, UserApp>();
        services.AddJwtConfiguration(configuration);
        services.AddMessageriaConfig(configuration);
        services.AddHostedService<PagamentoAprovadoConsumer>();
        services.AddHostedService<PreparoPedidoIniciadoConsumer>();
        services.AddHostedService<PreparoPedidoFinalizadoConsumer>();
        services.AddHostedService<EntregaPedidoRealizadaConsumer>();

        return services;
    }

    public static WebApplication UseApiConfig(this WebApplication app)
    {
        app.UseSwaggerConfig();

        app.UseHttpsRedirection();

        app.MapControllers();

        app.UseMiddleware<ExceptionMiddleware>();

        app.SubscribeEventHandlers();

        return app;
    }
}