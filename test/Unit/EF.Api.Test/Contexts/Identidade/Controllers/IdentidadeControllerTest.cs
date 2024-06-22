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
using EF.Api.Contexts.Carrinho.Controllers;
using EF.Carrinho.Application.DTOs.Requests;

namespace EF.Api.Test.Contexts.Identidade.Controllers;

public class IdentidadeControllerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IIdentidadeUseCase> _identidadeUseCaseMock;
    private readonly IdentidadeController _identidadeController;

    public IdentidadeControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
        _identidadeUseCaseMock = _fixture.Freeze<Mock<IIdentidadeUseCase>>();
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
}
