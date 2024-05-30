namespace EF.Cupons.Application.DTOs.Requests;

public record AtualizarCupomDto
{
    public Guid CupomId { get; set; }
    public DateTime DataInicio { get; init; }
    public DateTime DataFim { get; init; }
    public string CodigoCupom { get; init; }
    public decimal PorcentagemDesconto { get; init; }
}