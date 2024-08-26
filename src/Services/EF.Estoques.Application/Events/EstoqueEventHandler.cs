using EF.Core.Commons.Messages.Integrations;
using EF.Core.Commons.Messages;
using Microsoft.Extensions.DependencyInjection;
using EF.Estoques.Application.UseCases.Interfaces;
using EF.Estoques.Application.DTOs.Requests;
using EF.Estoques.Domain.Models;

namespace EF.Estoques.Application.Events;

public class EstoqueEventHandler : IEventHandler<PedidoCriadoEvent>, IEventHandler<PedidoCanceladoEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EstoqueEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(PedidoCriadoEvent @event)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var atualizarEstoqueUseCase = scope.ServiceProvider.GetRequiredService<IAtualizarEstoqueUseCase>();

        foreach(var i in @event.Itens)
        {
            var atualizarEstoqueDto = new AtualizarEstoqueDto()
            {
                OrigemMovimentacao = OrigemMovimentacaoEstoque.Venda,
                TipoMovimentacao = TipoMovimentacaoEstoque.Saida,
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            };

            await atualizarEstoqueUseCase.Handle(atualizarEstoqueDto);
        }
    }

    public async Task Handle(PedidoCanceladoEvent @event)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var atualizarEstoqueUseCase = scope.ServiceProvider.GetRequiredService<IAtualizarEstoqueUseCase>();

        foreach (var i in @event.Itens)
        {
            
            var atualizarEstoqueDto = new AtualizarEstoqueDto()
            {
                OrigemMovimentacao = OrigemMovimentacaoEstoque.Cancelamento,
                TipoMovimentacao = TipoMovimentacaoEstoque.Entrada,
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            };

            await atualizarEstoqueUseCase.Handle(atualizarEstoqueDto);
        }
    }
}