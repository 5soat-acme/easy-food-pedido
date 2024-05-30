using EF.Carrinho.Application.Events;
using EF.Core.Commons.Messages;
using EF.Core.Commons.Messages.Integrations;
using EF.Infra.Commons.EventBus;
using EF.Pedidos.Application.Events;
using EF.Pedidos.Application.Events.Messages;

namespace EF.Api.Commons.Config;

public static class EventBusConfig
{
    public static IServiceCollection AddEventBusConfig(this IServiceCollection services)
    {
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        
        // Carrinho
        services.AddScoped<IEventHandler<PedidoCriadoEvent>, CarrinhoEventHandler>();

        // Preparo entrega
        services.AddScoped<IEventHandler<PedidoRecebidoEvent>, PedidoEventHandler>();

        return services;
    }

    public static WebApplication SubscribeEventHandlers(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var bus = services.GetRequiredService<IEventBus>();
        
        services.GetRequiredService<IEnumerable<IEventHandler<PedidoCriadoEvent>>>().ToList()
            .ForEach(e => bus.Subscribe(e));

        services.GetRequiredService<IEnumerable<IEventHandler<PedidoRecebidoEvent>>>().ToList()
            .ForEach(e => bus.Subscribe(e));

        return app;
    }
}