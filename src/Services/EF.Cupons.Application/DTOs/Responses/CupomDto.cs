namespace EF.Cupons.Application.DTOs.Responses;

public record CupomDto
{
    public Guid Id { get; init; }
    public DateTime DataInicio { get; init; }
    public DateTime DataFim { get; init; }
    public decimal PorcentagemDesconto { get; init; }
    public IReadOnlyCollection<CupomProdutoDto> Produtos { get; init; }
}