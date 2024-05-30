namespace EF.Carrinho.Application.DTOs.Requests;

public class CarrinhoSessaoDto
{
    public Guid CarrinhoId { get; set; }
    public Guid? ClienteId { get; set; }
}