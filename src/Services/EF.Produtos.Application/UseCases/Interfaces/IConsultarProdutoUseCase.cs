using EF.Produtos.Application.DTOs.Responses;
using EF.Produtos.Domain.Models;

namespace EF.Produtos.Application.UseCases.Interfaces;

public interface IConsultarProdutoUseCase
{
    Task<ProdutoDto?> BuscarPorId(Guid id);
    Task<IEnumerable<ProdutoDto>?> Buscar(ProdutoCategoria? categoria);
}