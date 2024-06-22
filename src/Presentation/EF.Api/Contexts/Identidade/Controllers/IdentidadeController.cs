using EF.Identidade.Application.DTOs.Responses;
using EF.Identidade.Application.UseCases.Interfaces;
using EF.WebApi.Commons.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EF.Api.Contexts.Identidade.Controllers;

[Route("api/identidade")]
public class IdentidadeController(IIdentidadeUseCase useCase) : CustomControllerBase
{
    /// <summary>
    ///     Gera token de acesso para o cliente utilizar o sistema sem cadastro.
    /// </summary>
    /// <remarks>
    ///     Este método gera um token JWT (JSON Web Token) que deve ser usado em cabeçalhos de autenticação para futuras requisições.
    /// </remarks>
    /// <response code="200">Acessao realizado com sucesso.</response>
    /// <response code="400">A solicitação está malformada e não pode ser processada.</response>
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
}