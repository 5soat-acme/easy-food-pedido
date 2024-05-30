namespace EF.Carrinho.Application.UseCases.Interfaces;

public interface IRemoverCarrinhoUseCase
{
    Task RemoverCarrinho(Guid carrinhoId);
    Task RemoverCarrinhoPorClienteId(Guid clienteId);
}