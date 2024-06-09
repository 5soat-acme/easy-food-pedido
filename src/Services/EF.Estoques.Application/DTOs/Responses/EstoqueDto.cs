namespace EF.Estoques.Application.DTOs.Responses;

public class EstoqueDto
{
    public Guid ProdutoId { get; init; }
    public int Quantidade { get; init; }
}