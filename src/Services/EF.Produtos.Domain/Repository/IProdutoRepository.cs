using EF.Core.Commons.Repository;
using EF.Produtos.Domain.Models;

namespace EF.Produtos.Domain.Repository;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<Produto?> BuscarPorId(Guid produtoId);
    Task<IEnumerable<Produto>> Buscar(ProdutoCategoria? categoria);
    void Criar(Produto produto);
    void Atualizar(Produto produto);
    void Remover(Produto produto);
}