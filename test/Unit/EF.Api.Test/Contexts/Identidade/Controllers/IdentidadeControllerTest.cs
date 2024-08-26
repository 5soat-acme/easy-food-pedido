using AutoFixture.AutoMoq;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using EF.Api.Contexts.Identidade.Controllers;
using EF.Identidade.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EF.Core.Commons.Communication;
using EF.Identidade.Application.DTOs.Responses;
using FluentAssertions;
using EF.Identidade.Application.DTOs.Requests;
using EF.WebApi.Commons.Users;
using EF.Api.Contexts.Pedidos.Controllers;
using EF.Pedidos.Application.DTOs.Requests;

namespace EF.Api.Test.Contexts.Identidade.Controllers;

public class IdentidadeControllerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IIdentidadeUseCase> _identidadeUseCaseMock;
    private readonly Mock<ICriarSolicitacaoExclusaoUseCase> _criarSolicitacaoExclusaoUseCase;
    private readonly Mock<IUserApp> _userAppMock;
    private readonly IdentidadeController _identidadeController;

    public IdentidadeControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
        _identidadeUseCaseMock = _fixture.Freeze<Mock<IIdentidadeUseCase>>();
        _criarSolicitacaoExclusaoUseCase = _fixture.Freeze<Mock<ICriarSolicitacaoExclusaoUseCase>>();
        _userAppMock = _fixture.Freeze<Mock<IUserApp>>();
        _identidadeController = _fixture.Create<IdentidadeController>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoAcessarSistemaParaGerarToken()
    {
        // Arrange
        var operationResult = OperationResult<RespostaTokenAcesso>.Success(_fixture.Create<RespostaTokenAcesso>());
        _identidadeUseCaseMock.Setup(x => x.AcessarSistema()).ReturnsAsync(operationResult);

        // Act
        var resultado = await _identidadeController.Acessar();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().Be(operationResult.Data);
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoFalharAoGerarToken()
    {
        // Arrange
        var operationResult = OperationResult<RespostaTokenAcesso>.Failure("Erro");
        _identidadeUseCaseMock.Setup(x => x.AcessarSistema()).ReturnsAsync(operationResult);

        // Act
        var resultado = await _identidadeController.Acessar();

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequestResult.Value.Should().BeEquivalentTo(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", operationResult.GetErrorMessages().ToArray() }
        }));
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoSolicitarExclusao()
    {
        // Arrange
        var solicitacaoId = Guid.NewGuid();
        var criarSolicitacaoExclusaoDto = _fixture.Create<CriarSolicitacaoExclusaoDto>();
        var operationResult = OperationResult<Guid>.Success(solicitacaoId);

        _userAppMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _criarSolicitacaoExclusaoUseCase.Setup(x => x.Handle(criarSolicitacaoExclusaoDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _identidadeController.SolicitarExclusao(criarSolicitacaoExclusaoDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(new { solicitacaoId = solicitacaoId });
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoFalharSolicitarExclusao()
    {
        // Arrange
        var solicitacaoId = Guid.NewGuid();
        var criarSolicitacaoExclusaoDto = _fixture.Create<CriarSolicitacaoExclusaoDto>();
        var operationResult = OperationResult<Guid>.Failure("Erro ao solicitar exclusão");

        _userAppMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _criarSolicitacaoExclusaoUseCase.Setup(x => x.Handle(criarSolicitacaoExclusaoDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _identidadeController.SolicitarExclusao(criarSolicitacaoExclusaoDto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequestResult.Value.Should().BeEquivalentTo(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", operationResult.GetErrorMessages().ToArray() }
        }));
    }
}
