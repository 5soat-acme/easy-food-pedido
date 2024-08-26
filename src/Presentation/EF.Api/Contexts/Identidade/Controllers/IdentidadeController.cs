using EF.Identidade.Application.DTOs.Requests;
using EF.Identidade.Application.DTOs.Responses;
using EF.Identidade.Application.UseCases.Interfaces;
using EF.WebApi.Commons.Controllers;
using EF.WebApi.Commons.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EF.Api.Contexts.Identidade.Controllers;

[Authorize]
[Route("api/identidade")]
public class IdentidadeController(
    IIdentidadeUseCase useCase, 
    IUserApp userApp, 
    ICriarSolicitacaoExclusaoUseCase criarSolicitacaoExclusaoUseCase)
    : CustomControllerBase
{
    /// <summary>
    ///     Gera token de acesso para o cliente utilizar o sistema sem cadastro.
    /// </summary>
    /// <remarks>
    ///     Este método gera um token JWT (JSON Web Token) que deve ser usado em cabeçalhos de autenticação para futuras requisições.
    /// </remarks>
    /// <response code="200">Acesso realizado com sucesso.</response>
    /// <response code="400">A solicitação está malformada e não pode ser processada.</response>
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespostaTokenAcesso))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [Produces("application/json")]
    [HttpGet("acessar")]
    public async Task<IActionResult> Acessar()
    {
        var result = await useCase.AcessarSistema();

        if (!result.IsValid) AddErrors(result.GetErrorMessages());

        return Respond(result.Data);
    }

    /// <summary>
    ///     Solicita exclusão dos dados pessoais - LGPD.
    /// </summary>
    /// <response code="200">Retorna o Id da solicitação.</response>
    /// <response code="401">Não autorizado.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    [Authorize]
    [HttpPost("solicitar-exclusao")]
    public async Task<IActionResult> SolicitarExclusao(CriarSolicitacaoExclusaoDto dto)
    {
        dto.ClienteId = userApp.GetUserId();

        var result = await criarSolicitacaoExclusaoUseCase.Handle(dto);

        if (!result.IsValid) return Respond(result.GetErrorMessages());

        return Respond(new { solicitacaoId = result.Data });
    }
}