using EF.Pedidos.Domain.Models;

namespace EF.Pedidos.Application.DTOs.Responses;

public class PedidoDto
{
    public Guid Id { get; set; }
    public Guid? ClienteId { get; set; }
    public string? Cpf { get; set; }
    public Status Status { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
    public IEnumerable<ItemPedidoDto> Itens { get; set; }
}