using EF.Core.Commons.Communication;
using EF.Estoques.Application.DTOs.Requests;
using EF.Estoques.Application.DTOs.Responses;
using EF.Estoques.Application.UseCases.Interfaces;
using EF.WebApi.Commons.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EF.Api.Contexts.Estoques.Controllers;

[Route("api/estoques")]
public class EstoqueController : CustomControllerBase
{
    private readonly IAtualizarEstoqueUseCase _atualizarEstoqueUseCase;
    private readonly IConsultaEstoqueUseCase _consultaEstoqueUseCase;

    public EstoqueController(IConsultaEstoqueUseCase consultaEstoqueUseCase,
        IAtualizarEstoqueUseCase atualizarEstoqueUseCase)
    {
        _consultaEstoqueUseCase = consultaEstoqueUseCase;
        _atualizarEstoqueUseCase = atualizarEstoqueUseCase;
    }

    /// <summary>
    ///     Atualiza o estoque do produto.
    /// </summary>
    /// <remarks>
    ///     A quantidade em estoque será incrementada ou decrementada de acordo com os valores informados.
    /// </remarks>
    /// <response code="200">Indica que o estoque foi atualizado com sucesso.</response>
    /// <response code="400">A solicitação está malformada e não pode ser processada.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [Produces("application/json")]
    [HttpPost]
    public async Task<IActionResult> AtualizarEstoque([FromBody] AtualizarEstoqueDto dto,
        CancellationToken cancellationToken = default)
    {
        return Respond(await _atualizarEstoqueUseCase.Handle(dto));
    }

    /// <summary>
    ///     Obtém a quantidade em estoque do produto
    /// </summary>
    /// <response code="200">Retorna o estoque do produto.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EstoqueDto))]
    [Produces("application/json")]
    [HttpGet("{produtoId}")]
    public async Task<IActionResult> BuscarEstoque([FromRoute] Guid produtoId, CancellationToken cancellationToken)
    {
        return Respond(await _consultaEstoqueUseCase.ObterEstoqueProduto(produtoId, cancellationToken));
    }
}