using EF.Produtos.Application.DTOs.Responses;
using EF.Produtos.Application.Mappings;
using EF.Produtos.Application.UseCases.Interfaces;
using EF.Produtos.Domain.Models;
using EF.Produtos.Domain.Repository;

namespace EF.Produtos.Application.UseCases;

public class ConsultarProdutoUseCase : IConsultarProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public ConsultarProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<ProdutoDto?> BuscarPorId(Guid id)
    {
        var produto = await _produtoRepository.BuscarPorId(id);
        return DomainToDtoMapper.Map(produto);
    }

    public async Task<IEnumerable<ProdutoDto>?> Buscar(ProdutoCategoria? categoria)
    {
        var produtos = await _produtoRepository.Buscar(categoria);
        return DomainToDtoMapper.Map(produtos);
    }
}