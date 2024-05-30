using EF.Produtos.Domain.Models;

namespace EF.Produtos.Application.DTOs.Requests;

public record AtualizarProdutoDto
{
    public Guid ProdutoId { get; set; }
    public string Nome { get; init; }
    public decimal ValorUnitario { get; init; }
    public ProdutoCategoria Categoria { get; init; }
    public string Descricao { get; init; }
    public int TempoPreparoEstimado { get; init; }
    public bool Ativo { get; init; }
}