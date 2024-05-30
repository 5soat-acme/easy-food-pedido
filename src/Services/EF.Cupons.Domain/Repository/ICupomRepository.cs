using EF.Core.Commons.Repository;
using EF.Cupons.Domain.Models;

namespace EF.Cupons.Domain.Repository;

public interface ICupomRepository : IRepository<Cupom>
{
    Task<Cupom?> Buscar(Guid cupomId, CancellationToken cancellationToken = default);
    Task<CupomProduto?> BuscarCupomProduto(Guid cupomId, Guid produtoId, CancellationToken cancellationToken = default);
    Task<Cupom?> BuscarCupomVigente(string codigoCupom, CancellationToken cancellationToken = default);

    Task<IList<Cupom>> BuscarCupomVigenteEmPeriodo(string codigoCupom, DateTime dataInicio, DateTime dataFim,
        CancellationToken cancellationToken);

    Task<Cupom> Criar(Cupom cupom, CancellationToken cancellationToken = default);
    Cupom Atualizar(Cupom cupom, CancellationToken cancellationToken = default);
    Task InserirProdutos(IList<CupomProduto> produtos, CancellationToken cancellationToken = default);
    void RemoverProdutos(IList<CupomProduto> produtos, CancellationToken cancellationToken = default);
}