namespace EF.Cupons.Application.DTOs.Requests;

public class RemoverProdutoDto
{
    public Guid CupomId { get; set; }
    public IReadOnlyCollection<Guid> Produtos { get; set; }
}