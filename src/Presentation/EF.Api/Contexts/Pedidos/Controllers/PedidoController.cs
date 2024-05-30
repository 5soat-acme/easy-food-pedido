using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Application.DTOs.Responses;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.WebApi.Commons.Controllers;
using EF.WebApi.Commons.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EF.Api.Contexts.Pedidos.Controllers;

[Route("api/pedidos")]
public class PedidoController(
    IConsultarPedidoUseCase consultarPedidoUseCase,
    ICriarPedidoUseCase criarPedidoUseCase,
    IUserApp userApp)
    : CustomControllerBase
{
    /// <summary>
    ///     Obtém um pedido.
    /// </summary>
    /// <param name="id">Id do pedido</param>
    /// <response code="200">Dados do pedido.</response>
    /// <response code="401">Não autorizado.</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PedidoDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPedido([FromRoute] Guid id)
    {
        var pedido = await consultarPedidoUseCase.ObterPedidoPorId(id);
        return pedido is not null ? Respond(pedido) : NotFound();
    }

    /// <summary>
    ///     Faz o checkout do pedido. Esse processo efetiva o pedido que fica disponível para pagamento.
    /// </summary>
    /// <response code="200">Retorna o Id do pedido.</response>
    /// <response code="401">Não autorizado.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    [Authorize]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CriarPedidoDto dto)
    {
        dto.SessionId = userApp.GetSessionId();
        dto.ClienteId = userApp.GetUserId();
        dto.ClienteCpf = userApp.GetUserCpf();

        var result = await criarPedidoUseCase.Handle(dto);

        if (!result.IsValid) return Respond(result.GetErrorMessages());

        return Respond(new { pedidoId = result.Data });
    }
}