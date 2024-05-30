using EF.Core.Commons.DomainObjects;
using EF.Core.Commons.ValueObjects;

namespace EF.Pedidos.Domain.Models;

public class Pedido : Entity, IAggregateRoot
{
    private readonly List<Item> _itens;

    public Pedido()
    {
        Status = Status.AguardandoPagamento;
        _itens = new List<Item>();
    }

    public Guid? ClienteId { get; private set; }
    public Cpf? Cpf { get; private set; }
    public Status Status { get; private set; }
    public decimal ValorTotal { get; private set; }
    public Guid? CupomId { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    public IReadOnlyCollection<Item> Itens => _itens;

    public void AdicionarItem(Item item)
    {
        _itens.Add(item);
    }

    public void AtualizarStatus(Status status)
    {
        Status = status;
    }

    public void CalcularValorTotal()
    {
        foreach (var item in _itens) item.CalcularValorFinal();

        ValorTotal = _itens.Sum(i => i.ValorFinal * i.Quantidade);
    }

    public void AssociarCpf(Cpf cpf)
    {
        Cpf = cpf;
    }

    public void AssociarCupom(Guid cupomId)
    {
        CupomId = cupomId;
    }

    public void AplicarDescontoItem(Guid itemId, decimal desconto)
    {
        var item = _itens.FirstOrDefault(i => i.Id == itemId);
        if (item is null) return;

        item.AplicarDesconto(desconto);
    }

    public void AssociarCliente(Guid clienteId)
    {
        ClienteId = clienteId;
    }

    public void ConfirmarPagamento()
    {
        Status = Status.Recebido;
    }
}