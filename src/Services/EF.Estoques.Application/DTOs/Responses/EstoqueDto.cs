namespace EF.Estoques.Application.DTOs.Responses;

public record EstoqueDto
{
    public Guid ProdutoId { get; init; }
    public int Quantidade { get; init; }
}