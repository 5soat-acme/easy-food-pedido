using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Core.Commons.Messages;
using EF.Core.Commons.Messages.Integrations;
using Microsoft.Extensions.DependencyInjection;

namespace EF.Carrinho.Application.Events;

public class CarrinhoEventHandler : IEventHandler<PedidoCriadoEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CarrinhoEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(PedidoCriadoEvent @event)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var removerCarrinhoUseCase = scope.ServiceProvider.GetRequiredService<IRemoverCarrinhoUseCase>();

        if (@event.ClienteId.HasValue)
        {
            await removerCarrinhoUseCase.RemoverCarrinhoPorClienteId(@event.ClienteId.Value);
            return;
        }

        await removerCarrinhoUseCase.RemoverCarrinho(@event.SessionId);
    }
}