using EF.Core.Commons.DomainObjects;

namespace EF.Carrinho.Domain.Models;

public class CarrinhoCliente : Entity, IAggregateRoot
{
    private readonly List<Item> _itens;

    public CarrinhoCliente()
    {
        _itens = new List<Item>();
    }

    public Guid? ClienteId { get; private set; }
    public decimal ValorTotal { get; private set; }
    public int TempoMedioPreparo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }
    public IReadOnlyCollection<Item> Itens => _itens;

    public void AssociarCliente(Guid clienteId)
    {
        if (!ValidarCliente(clienteId)) throw new DomainException("Cliente inválido");
        ClienteId = clienteId;
    }

    public void AssociarCarrinho(Guid id)
    {
        if (id == Guid.Empty) throw new DomainException("Id do carrinho inválido");

        Id = id;
    }

    public void AdicionarItem(Item item)
    {
        item.AssociarCarrinho(Id);
        var itemExistente = ObterItemPorProdutoId(item.ProdutoId);

        if (itemExistente is not null)
        {
            itemExistente.AtualizarQuantidade(itemExistente.Quantidade + item.Quantidade);
            item = itemExistente;
            _itens.Remove(itemExistente);
        }

        _itens.Add(item);
        CalcularValorTotal();
        CalcularTempoPreparo();
    }

    public bool ProdutoExiste(Guid produtoId)
    {
        return _itens.Any(p => p.ProdutoId == produtoId);
    }

    public Item? ObterItemPorProdutoId(Guid produtoId)
    {
        return _itens.FirstOrDefault(p => p.ProdutoId == produtoId);
    }

    public Item? ObterItemPorId(Guid id)
    {
        return _itens.FirstOrDefault(f => f.Id == id);
    }

    public void RemoverItem(Item item)
    {
        _itens.Remove(item);
        CalcularValorTotal();
        CalcularTempoPreparo();
    }

    public void CalcularValorTotal()
    {
        ValorTotal = Itens.Sum(i => i.ValorUnitario * i.Quantidade);
    }

    public bool ValidarCliente(Guid clienteId)
    {
        if (clienteId == Guid.Empty) return false;

        return true;
    }

    public void AtualizarQuantidadeItem(Guid itemId, int quantidade)
    {
        var item = _itens.FirstOrDefault(f => f.Id == itemId);

        if (item is null) throw new DomainException("Item não econtrado");

        item.AtualizarQuantidade(quantidade);

        CalcularValorTotal();
        CalcularTempoPreparo();
    }

    public void CalcularTempoPreparo()
    {
        if (Itens.Count == 0)
        {
            TempoMedioPreparo = 0;
            return;
        }

        TempoMedioPreparo = Itens.Sum(i => i.TempoEstimadoPreparo) / Itens.Count;
    }
}