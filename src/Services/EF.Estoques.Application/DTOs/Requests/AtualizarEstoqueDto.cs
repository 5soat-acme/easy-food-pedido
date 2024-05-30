using EF.Estoques.Domain.Models;

namespace EF.Estoques.Application.DTOs.Requests;

public record AtualizarEstoqueDto
{
    public Guid ProdutoId { get; init; }
    public int Quantidade { get; init; }
    public TipoMovimentacaoEstoque TipoMovimentacao { get; init; }
    public OrigemMovimentacaoEstoque OrigemMovimentacao { get; init; }
}