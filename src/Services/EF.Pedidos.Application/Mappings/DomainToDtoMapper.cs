using EF.Pedidos.Application.DTOs.Responses;
using EF.Pedidos.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EF.Pedidos.Application.Mappings;

public static class DomainToDtoMapper
{
    public static PedidoDto? Map(Pedido? model)
    {
        if (model is null) return null;

        return new PedidoDto
        {
            Id = model.Id,
            ClienteId = model.ClienteId,
            Cpf = model.Cpf?.Numero,
            Status = model.Status,
            ValorTotal = model.ValorTotal,
            DataCriacao = model.DataCriacao,
            DataUltimaAtualizacao = model.DataAtualizacao,
            Itens = model.Itens.Select(Map).ToList()
        };
    }

    public static ItemPedidoDto Map(Item model)
    {
        if (model is null) return null;

        return new ItemPedidoDto
        {
            Id = model.Id,
            ProdutoId = model.ProdutoId,
            Quantidade = model.Quantidade,
            ValorUnitario = model.ValorUnitario,
            Desconto = model.Desconto,
            ValorFinal = model.ValorFinal
        };
    }
}