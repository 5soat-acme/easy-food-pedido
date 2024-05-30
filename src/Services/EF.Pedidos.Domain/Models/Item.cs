using EF.Core.Commons.DomainObjects;

namespace EF.Pedidos.Domain.Models;

public class Item : Entity
{
    // EF
    protected Item()
    {
    }

    public Item(Guid produtoId, string nome, decimal valorUnitario, int quantidade)
    {
        if (!ValidarProduto(produtoId)) throw new DomainException("Produto inválido");
        if (!ValidarNome(nome)) throw new DomainException("Nome do item inválido");
        if (!ValidarValorUnitario(valorUnitario)) throw new DomainException("Valor unitário do item inválido");
        if (!ValidarQuantidade(quantidade)) throw new DomainException("Quantidade do item inválida");

        ProdutoId = produtoId;
        ValorUnitario = valorUnitario;
        Quantidade = quantidade;
    }

    public decimal ValorUnitario { get; private set; }
    public decimal? Desconto { get; private set; }
    public decimal ValorFinal { get; private set; }
    public int Quantidade { get; private set; }
    public Guid ProdutoId { get; private set; }
    public Guid PedidoId { get; private set; }
    public Pedido Pedido { get; private set; }

    public bool ValidarProduto(Guid produtoId)
    {
        if (produtoId == Guid.Empty) return false;

        return true;
    }

    public bool ValidarNome(string nome)
    {
        if (string.IsNullOrEmpty(nome)) return false;

        return true;
    }

    public bool ValidarValorUnitario(decimal valorUnitario)
    {
        if (valorUnitario <= 0) return false;

        return true;
    }

    public bool ValidarQuantidade(int quantidade)
    {
        if (quantidade <= 0) return false;

        return true;
    }

    public void CalcularValorFinal()
    {
        if (Desconto is not null)
            ValorFinal = ValorUnitario - ValorUnitario * Desconto.Value;
        else
            ValorFinal = ValorUnitario;
    }

    public void AplicarDesconto(decimal desconto)
    {
        if (desconto <= 0) throw new DomainException("Desconto inválido");
        Desconto = desconto;
    }
}