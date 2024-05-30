using EF.Produtos.Domain.Models;

namespace EF.Produtos.Application.DTOs.Requests;

public record CriarProdutoDto
{
    public string Nome { get; init; }
    public decimal ValorUnitario { get; init; }
    public ProdutoCategoria Categoria { get; init; }
    public int TempoPreparoEstimado { get; init; }
    public string Descricao { get; init; }
}