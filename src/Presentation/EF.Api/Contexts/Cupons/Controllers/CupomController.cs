using EF.Core.Commons.Communication;
using EF.Cupons.Application.DTOs.Requests;
using EF.Cupons.Application.DTOs.Responses;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.WebApi.Commons.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EF.Api.Contexts.Cupons.Controllers;

[Route("api/cupons")]
public class CupomController : CustomControllerBase
{
    private readonly IAtualizarCupomUseCase _atualizarCupomUseCase;
    private readonly IConsultarCupomUseCase _consultarCupomUseCase;
    private readonly ICriarCupomUseCase _criarCupomUseCase;
    private readonly IInativarCupomUseCase _inativarCupomUseCase;
    private readonly IInserirProdutoUseCase _inserirProdutoUseCase;
    private readonly IRemoverProdutoUseCase _removerProdutoUseCase;

    public CupomController(IConsultarCupomUseCase consultarCupomUseCase, IAtualizarCupomUseCase atualizarCupomUseCase,
        ICriarCupomUseCase criarCupomUseCase, IInativarCupomUseCase inativarCupomUseCase,
        IInserirProdutoUseCase inserirProdutoUseCase, IRemoverProdutoUseCase removerProdutoUseCase)
    {
        _consultarCupomUseCase = consultarCupomUseCase;
        _atualizarCupomUseCase = atualizarCupomUseCase;
        _criarCupomUseCase = criarCupomUseCase;
        _inativarCupomUseCase = inativarCupomUseCase;
        _inserirProdutoUseCase = inserirProdutoUseCase;
        _removerProdutoUseCase = removerProdutoUseCase;
    }

    /// <summary>
    ///     Busca cupom de desconto através do código do cupom
    /// </summary>
    /// <remarks>
    ///     Será retornada as informações do cupom caso ele esteja vigente.
    /// </remarks>
    /// <response code="200">Retorna as informações do cupom.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CupomDto))]
    [Produces("application/json")]
    [HttpGet("{codigoCupom}")]
    public async Task<IActionResult> BuscarCupomProduto(string codigoCupom, CancellationToken cancellationToken)
    {
        return Respond(await _consultarCupomUseCase.ObterCupom(codigoCupom, cancellationToken));
    }

    /// <summary>
    ///     Cria cupom de desconto.
    /// </summary>
    /// <response code="200">Indica que o cupom de desconto foi criado com sucesso.</response>
    /// <response code="400">A solicitação está malformada e não pode ser processada.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [Produces("application/json")]
    [HttpPost]
    public async Task<IActionResult> CriarCupom([FromBody] CriarCupomDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CriarCupomDto
        {
            DataInicio = dto.DataInicio,
            DataFim = dto.DataFim,
            CodigoCupom = dto.CodigoCupom,
            PorcentagemDesconto = dto.PorcentagemDesconto,
            Produtos = dto.Produtos
        };

        return Respond(await _criarCupomUseCase.Handle(command));
    }

    /// <summary>
    ///     Atualiza cupom de desconto.
    /// </summary>
    /// <response code="200">Indica que o cupom de desconto foi atualizado com sucesso.</response>
    /// <response code="400">A solicitação está malformada e não pode ser processada.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [Produces("application/json")]
    [HttpPut("{cupomId}")]
    public async Task<IActionResult> AtualizarCupom(Guid cupomId, [FromBody] AtualizarCupomDto dto)
    {
        dto.CupomId = cupomId;
        return Respond(await _atualizarCupomUseCase.Handle(dto));
    }

    /// <summary>
    ///     Inativa cupom de desconto.
    /// </summary>
    /// <response code="200">Indica que o cupom de desconto foi inativado com sucesso.</response>
    /// <response code="400">A solicitação está malformada e não pode ser processada.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [Produces("application/json")]
    [HttpPut("inativar/{cupomId}")]
    public async Task<IActionResult> InativarCupom(Guid cupomId)
    {
        return Respond(await _inativarCupomUseCase.Handle(cupomId));
    }

    /// <summary>
    ///     Remove produtos do cupom de desconto.
    /// </summary>
    /// <response code="200">Indica que os produtos foram removidos do cupom de desconto.</response>
    /// <response code="400">A solicitação está malformada e não pode ser processada.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [Produces("application/json")]
    [HttpDelete("{cupomId}/remover-produtos")]
    public async Task<IActionResult> RemoverCupomProduto(Guid cupomId,
        [FromBody] IList<AdicionarRemoverCupomProdutoDto> produtos)
    {
        var dto = new RemoverProdutoDto
        {
            CupomId = cupomId,
            Produtos = produtos.Select(x => x.ProdutoId).ToList()
        };

        return Respond(await _removerProdutoUseCase.Handle(dto));
    }

    /// <summary>
    ///     Adiciona produtos no cupom de desconto.
    /// </summary>
    /// <response code="200">Indica que os produtos foram adicionados no cupom de desconto.</response>
    /// <response code="400">A solicitação está malformada e não pode ser processada.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [Produces("application/json")]
    [HttpPut("{cupomId}/inserir-produtos")]
    public async Task<IActionResult> InserirCupomProduto(Guid cupomId,
        [FromBody] IList<AdicionarRemoverCupomProdutoDto> produtos,
        CancellationToken cancellationToken)
    {
        var dto = new InserirProdutoDto
        {
            CupomId = cupomId,
            Produtos = produtos.Select(x => x.ProdutoId).ToList()
        };

        return Respond(await _inserirProdutoUseCase.Handle(dto));
    }
}