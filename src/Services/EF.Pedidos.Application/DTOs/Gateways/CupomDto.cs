namespace EF.Pedidos.Application.DTOs.Gateways;

public class CupomDto
{
    public Guid Id { get; set; }
    public decimal PorcentagemDesconto { get; set; }
    public List<CupomProdutoDto> Produtos { get; init; }

    public class CupomProdutoDto
    {
        public Guid ProdutoId { get; init; }
    }
}