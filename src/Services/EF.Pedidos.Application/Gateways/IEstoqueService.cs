namespace EF.Pedidos.Application.Gateways;

public interface IEstoqueService
{
    Task<bool> VerificarEstoque(Guid produtoId, int quantidade);
}