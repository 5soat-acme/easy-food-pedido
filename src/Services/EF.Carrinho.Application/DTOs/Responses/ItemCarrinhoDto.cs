namespace EF.Carrinho.Application.DTOs.Responses;

public class ItemCarrinhoDto
{
    public Guid Id { get; set; }
    public decimal ValorUnitario { get; set; }
    public int Quantidade { get; set; }
    public Guid ProdutoId { get; set; }
    public string Nome { get; set; }
    public int TempoEstimadoPreparo { get; set; }
}