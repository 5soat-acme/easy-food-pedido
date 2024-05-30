using EF.Core.Commons.DomainObjects;

namespace EF.Estoques.Domain.Models;

public class Estoque : Entity, IAggregateRoot
{
    private readonly List<MovimentacaoEstoque> _movimentacoes;

    private Estoque()
    {
        _movimentacoes = new List<MovimentacaoEstoque>();
    }

    public Estoque(Guid produtoId, int quantidade)
    {
        _movimentacoes = new List<MovimentacaoEstoque>();

        if (!ValidarProduto(produtoId)) throw new DomainException("Produto inválido");
        if (!ValidarQuantidade(quantidade)) throw new DomainException("Quantidade inválida");

        ProdutoId = produtoId;
        Quantidade = quantidade;
    }

    public Guid ProdutoId { get; private set; }
    public int Quantidade { get; private set; }
    public IReadOnlyCollection<MovimentacaoEstoque> Movimentacoes => _movimentacoes;

    public void AdicionarMovimentacao(MovimentacaoEstoque movimentacao)
    {
        _movimentacoes.Add(movimentacao);
    }

    public void AtualizarQuantidade(int quantidade, TipoMovimentacaoEstoque tipoMovimentacaoEstoque)
    {
        if (tipoMovimentacaoEstoque == TipoMovimentacaoEstoque.Entrada)
            Quantidade += quantidade;
        else if (tipoMovimentacaoEstoque == TipoMovimentacaoEstoque.Saida)
            Quantidade -= quantidade;

        if (!ValidarQuantidade(Quantidade)) throw new DomainException("Produto não possui estoque suficiente");
    }

    private bool ValidarProduto(Guid produtoId)
    {
        if (produtoId == Guid.Empty) return false;

        return true;
    }

    private bool ValidarQuantidade(int quantidade)
    {
        if (quantidade < 0) return false;

        return true;
    }
}