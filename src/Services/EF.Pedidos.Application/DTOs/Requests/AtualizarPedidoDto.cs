using EF.Pedidos.Domain.Models;

namespace EF.Pedidos.Application.DTOs.Requests;

public class AtualizarPedidoDto
{
    public Guid PedidoId { get; set; }
    public Status Status { get; set; }
}