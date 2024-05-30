using EF.Carrinho.Domain.Models;

namespace EF.Carrinho.Application.Gateways;

public interface IProdutoService
{
    Task<Item> ObterItemPorProdutoId(Guid id);
}