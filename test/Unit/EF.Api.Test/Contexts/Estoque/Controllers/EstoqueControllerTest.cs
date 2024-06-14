using AutoFixture.AutoMoq;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using EF.Api.Contexts.Estoques.Controllers;
using EF.Estoques.Application.UseCases.Interfaces;
using EF.Core.Commons.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EF.Estoques.Application.DTOs.Requests;
using FluentAssertions;
using EF.Estoques.Application.DTOs.Responses;

namespace EF.Api.Test.Contexts.Estoque.Controllers;

public class EstoqueControllerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IAtualizarEstoqueUseCase> _atualizarEstoqueUseCaseMock;
    private readonly Mock<IConsultaEstoqueUseCase> _consultaEstoqueUseCaseMock;
    private readonly EstoqueController _estoqueController;

    public EstoqueControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
        _atualizarEstoqueUseCaseMock = _fixture.Freeze<Mock<IAtualizarEstoqueUseCase>>();
        _consultaEstoqueUseCaseMock = _fixture.Freeze<Mock<IConsultaEstoqueUseCase>>();
        _estoqueController = _fixture.Create<EstoqueController>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoAtualizarEstoque()
    {
        // Arrange
        var atualizarEstoqueDto = _fixture.Create<AtualizarEstoqueDto>();
        var operationResult = OperationResult<Guid>.Success(Guid.NewGuid());

        _atualizarEstoqueUseCaseMock.Setup(x => x.Handle(atualizarEstoqueDto)).ReturnsAsync(operationResult);
        // Act
        var resultado = await _estoqueController.AtualizarEstoque(atualizarEstoqueDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().Be(operationResult);
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoBuscarEstoque()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var estoqueDto = _fixture.Create<EstoqueDto>();

        _consultaEstoqueUseCaseMock.Setup(x => x.ObterEstoqueProduto(produtoId, default)).ReturnsAsync(estoqueDto);
        // Act
        var resultado = await _estoqueController.BuscarEstoque(produtoId, default);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().Be(estoqueDto);
    }
}
