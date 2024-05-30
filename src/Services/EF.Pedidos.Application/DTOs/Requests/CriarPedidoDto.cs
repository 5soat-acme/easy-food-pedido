using System.Text.Json.Serialization;

namespace EF.Pedidos.Application.DTOs.Requests;

public class CriarPedidoDto
{
    [JsonIgnore] public Guid SessionId { get; set; }
    [JsonIgnore] public Guid? ClienteId { get; set; }
    [JsonIgnore] public string? ClienteCpf { get; set; }
    public string? CodigoCupom { get; set; }
    public List<ItemPedido> Itens { get; set; }

    public class ItemPedido
    {
        public int Quantidade { get; set; }
        public Guid ProdutoId { get; set; }
    }
}