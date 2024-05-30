using EF.Core.Commons.Communication;
using EF.Produtos.Application.DTOs.Requests;
using EF.Produtos.Application.DTOs.Responses;
using EF.Produtos.Application.UseCases.Interfaces;
using EF.Produtos.Domain.Models;
using EF.WebApi.Commons.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EF.Api.Contexts.Produtos.Controllers;

[Route("api/produtos")]
public class ProdutoController : CustomControllerBase
{
    private readonly IAtualizarProdutoUseCase _atualizarProdutoUseCase;
    private readonly IConsultarProdutoUseCase _consultarProdutoUseCase;
    private readonly ICriarProdutoUseCase _criarProdutoUseCase;
    private readonly IRemoverProdutoUseCase _removerProdutoUseCase;

    public ProdutoController(IConsultarProdutoUseCase consultarProdutoUseCase,
        IAtualizarProdutoUseCase atualizarProdutoUseCase, ICriarProdutoUseCase criarProdutoUseCase,
        IRemoverProdutoUseCase removerProdutoUseCase)
    {
        _consultarProdutoUseCase = consultarProdutoUseCase;
        _atualizarProdutoUseCase = atualizarProdutoUseCase;
        _criarProdutoUseCase = criarProdutoUseCase;
        _removerProdutoUseCase = removerProdutoUseCase;
    }

    /// <summary>
    ///     Obtém o produto por categoria
    /// </summary>
    /// <response code="200">Lista de produtos por categoria.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProdutoDto>))]
    [Produces("application/json")]
    [HttpGet]
    public async Task<IActionResult> Obter([FromQuery] ProdutoCategoria? categoria)
    {
        var pedidos = await _consultarProdutoUseCase.Buscar(categoria);
        return pedidos is null ? NotFound() : Respond(pedidos);
    }

    /// <summary>
    ///     Cadastra um produto
    /// </summary>
    /// <response code="200">Produto cadastrado.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [Produces("application/json")]
    [HttpPost]
    public async Task<IActionResult> Criar(CriarProdutoDto produto)
    {
        var result = await _criarProdutoUseCase.Handle(produto);
        return Respond(result);
    }

    /// <summary>
    ///     Atualiza um produto
    /// </summary>
    /// <response code="200">Produto cadastrado.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [Produces("application/json")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, AtualizarProdutoDto produto)
    {
        if (!ModelState.IsValid) return Respond(ModelState);

        produto.ProdutoId = id;

        var result = await _atualizarProdutoUseCase.Handle(produto);
        return Respond(result);
    }

    /// <summary>
    ///     Remove um produto
    /// </summary>
    /// <response code="200">Produto removido.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [Produces("application/json")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        if (!ModelState.IsValid) return Respond(ModelState);
        var result = await _removerProdutoUseCase.Handle(id);

        return Respond(result);
    }
}