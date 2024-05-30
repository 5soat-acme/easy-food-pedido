using EF.Core.Commons.DomainObjects;

namespace EF.Estoques.Domain.Models;

public class MovimentacaoEstoque : Entity
{
    private MovimentacaoEstoque()
    {
    }

    public MovimentacaoEstoque(Guid estoqueId, Guid produtoId, int quantidade, TipoMovimentacaoEstoque tipoMovimentacao,
        OrigemMovimentacaoEstoque origemMovimentacao, DateTime dataLancamento)
    {
        if (!ValidarEstoque(estoqueId)) throw new DomainException("Uma movimentação deve estar associada a um estoque");
        if (!ValidarTipoMovimentacao(tipoMovimentacao)) throw new DomainException("TipoMovimentacao inválido");
        if (!ValidarOrigemMovimentacao(origemMovimentacao)) throw new DomainException("OrigemMovimentacao inválida");
        if (!ValidarOrigemMovimentacaoCompativelTipoMovimentacao(tipoMovimentacao, origemMovimentacao))
            throw new DomainException("OrigemMovimentacao incompatível com TipoMovimentacao");

        EstoqueId = estoqueId;
        ProdutoId = produtoId;
        Quantidade = quantidade;
        TipoMovimentacao = tipoMovimentacao;
        OrigemMovimentacao = origemMovimentacao;
        DataLancamento = dataLancamento;
    }

    public Guid ProdutoId { get; private set; }
    public int Quantidade { get; private set; }
    public TipoMovimentacaoEstoque TipoMovimentacao { get; private set; }
    public OrigemMovimentacaoEstoque OrigemMovimentacao { get; private set; }
    public DateTime DataLancamento { get; private set; }
    public Guid EstoqueId { get; private set; }
    public Estoque Estoque { get; private set; }

    private bool ValidarEstoque(Guid estoqueId)
    {
        if (estoqueId == Guid.Empty) return false;

        return true;
    }

    private bool ValidarTipoMovimentacao(TipoMovimentacaoEstoque tipoMovimentacao)
    {
        if (!Enum.IsDefined(typeof(TipoMovimentacaoEstoque), tipoMovimentacao)) return false;

        return true;
    }

    private bool ValidarOrigemMovimentacao(OrigemMovimentacaoEstoque origemMovimentacao)
    {
        if (!Enum.IsDefined(typeof(OrigemMovimentacaoEstoque), origemMovimentacao)) return false;

        return true;
    }

    private bool ValidarOrigemMovimentacaoCompativelTipoMovimentacao(TipoMovimentacaoEstoque tipoMovimentacao,
        OrigemMovimentacaoEstoque origemMovimentacao)
    {
        var origemEntrada = new[] { OrigemMovimentacaoEstoque.Compra };
        var origemSaida = new[] { OrigemMovimentacaoEstoque.Venda };

        return tipoMovimentacao == TipoMovimentacaoEstoque.Entrada
            ? origemEntrada.Contains(origemMovimentacao)
            : origemSaida.Contains(origemMovimentacao);
    }
}