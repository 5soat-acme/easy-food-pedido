using EF.Carrinho.Application.DTOs.Requests;
using EF.Carrinho.Application.DTOs.Responses;
using EF.Carrinho.Domain.Models;

namespace EF.Carrinho.Application.UseCases.Interfaces;

public interface IConsultarCarrinhoUseCase
{
    Task<CarrinhoClienteDto> ConsultarCarrinho(CarrinhoSessaoDto carrinhoSessao);
    Task<CarrinhoCliente?> ObterCarrinho(CarrinhoSessaoDto carrinhoSessao);
}