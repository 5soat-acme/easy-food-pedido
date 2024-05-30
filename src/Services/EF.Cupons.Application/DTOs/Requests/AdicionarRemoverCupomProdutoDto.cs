namespace EF.Cupons.Application.DTOs.Requests;

public record AdicionarRemoverCupomProdutoDto
{
    public Guid ProdutoId { get; init; }
}