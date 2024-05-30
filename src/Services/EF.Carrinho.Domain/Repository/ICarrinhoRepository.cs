using EF.Carrinho.Domain.Models;
using EF.Core.Commons.Repository;

namespace EF.Carrinho.Domain.Repository;

public interface ICarrinhoRepository : IRepository<CarrinhoCliente>
{
    Task<CarrinhoCliente?> ObterPorClienteId(Guid clienteId);
    Task<CarrinhoCliente?> ObterPorId(Guid id);
    void Criar(CarrinhoCliente carrinho);
    void Atualizar(CarrinhoCliente carrinho);
    void Remover(CarrinhoCliente carrinho);
    void AdicionarItem(Item item);
    void AtualizarItem(Item item);
    void RemoverItem(Item item);
}