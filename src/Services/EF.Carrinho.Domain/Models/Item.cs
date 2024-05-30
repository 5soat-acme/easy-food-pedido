using EF.Core.Commons.DomainObjects;

namespace EF.Carrinho.Domain.Models;

public class Item : Entity
{
    public Item()
    {
    }

    public Item(Guid produtoId, string nome, decimal valorUnitario, int tempoEstimadoPreparo)
    {
        if (!ValidarProduto(produtoId)) throw new DomainException("Produto inválido");
        if (!ValidarValorUnitario(valorUnitario)) throw new DomainException("Valor unitário inválido");

        ProdutoId = produtoId;
        Nome = nome;
        ValorUnitario = valorUnitario;
        TempoEstimadoPreparo = tempoEstimadoPreparo;
    }

    public decimal ValorUnitario { get; private set; }
    public int Quantidade { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string Nome { get; private set; }
    public int TempoEstimadoPreparo { get; private set; }
    public Guid CarrinhoId { get; private set; }
    public CarrinhoCliente Carrinho { get; }

    public void AtualizarQuantidade(int quantidade)
    {
        if (!ValidarQuantidade(quantidade)) throw new DomainException("Quantidade inválida");
        Quantidade = quantidade;
    }

    public bool ValidarProduto(Guid produtoId)
    {
        if (produtoId == Guid.Empty) return false;

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

    public void AssociarCarrinho(Guid carrinhoId)
    {
        if (carrinhoId == Guid.Empty) throw new DomainException("Id do carrinho inválido");
        CarrinhoId = carrinhoId;
    }
}