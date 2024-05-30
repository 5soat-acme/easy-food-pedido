using EF.Core.Commons.DomainObjects;

namespace EF.Cupons.Domain.Models;

public class CupomProduto : Entity
{
    private CupomProduto()
    {
    }

    public CupomProduto(Guid cupomId, Guid produtoId)
    {
        if (!ValidarCupom(cupomId)) throw new DomainException("Um CupomProduto deve estar associado a um Cupom");
        if (!ValidarProduto(produtoId)) throw new DomainException("Produto inválido");

        CupomId = cupomId;
        ProdutoId = produtoId;
    }

    public Guid CupomId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public Cupom Cupom { get; private set; }

    private bool ValidarCupom(Guid cupomId)
    {
        if (cupomId == Guid.Empty) return false;

        return true;
    }

    private bool ValidarProduto(Guid produtoId)
    {
        if (produtoId == Guid.Empty) return false;

        return true;
    }
}