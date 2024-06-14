using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Produtos.Application.UseCases.Interfaces;
using Moq;
using EF.Api.Contexts.Produtos.Controllers;
using EF.Produtos.Application.DTOs.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EF.Produtos.Domain.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using EF.Produtos.Application.DTOs.Requests;
using EF.Core.Commons.Communication;

namespace EF.Api.Test.Contexts.Produtos.Controllers;

public class ProdutoControllerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IAtualizarProdutoUseCase> _atualizarProdutoUseCaseMock;
    private readonly Mock<IConsultarProdutoUseCase> _consultarProdutoUseCaseMock;
    private readonly Mock<ICriarProdutoUseCase> _criarProdutoUseCaseMock;
    private readonly Mock<IRemoverProdutoUseCase> _removerProdutoUseCaseMock;
    private readonly ProdutoController _produtoController;

    public ProdutoControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
        _atualizarProdutoUseCaseMock = _fixture.Freeze<Mock<IAtualizarProdutoUseCase>>();
        _consultarProdutoUseCaseMock = _fixture.Freeze<Mock<IConsultarProdutoUseCase>>();
        _criarProdutoUseCaseMock = _fixture.Freeze<Mock<ICriarProdutoUseCase>>();
        _removerProdutoUseCaseMock = _fixture.Freeze<Mock<IRemoverProdutoUseCase>>();
        _produtoController = _fixture.Create<ProdutoController>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoObterProdutos()
    {
        // Arrange
        var produtos = _fixture.CreateMany<ProdutoDto>(5).ToList();

        _consultarProdutoUseCaseMock.Setup(x => x.Buscar(null)).ReturnsAsync(produtos);

        // Act
        var resultado = await _produtoController.Obter(null);

        // Assert
        _consultarProdutoUseCaseMock.Verify(x => x.Buscar(null), Times.Once);
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(produtos);
    }

    [Fact]
    public async Task DeveRetornarNotFound_QuandoNaoHouverProdutos()
    {
        // Arrange
        _consultarProdutoUseCaseMock.Setup(x => x.Buscar(It.IsAny<ProdutoCategoria?>()))
            .ReturnsAsync((IEnumerable<ProdutoDto>?)null);

        // Act
        var result = await _produtoController.Obter(null);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoCriarProduto()
    {
        // Arrange
        var criarProdutoDto = _fixture.Create<CriarProdutoDto>();
        var operationResult = OperationResult<Guid>.Success(Guid.NewGuid());

        _criarProdutoUseCaseMock.Setup(x => x.Handle(criarProdutoDto)).ReturnsAsync(operationResult);
        // Act
        var resultado = await _produtoController.Criar(criarProdutoDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().Be(operationResult);
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoAtualizarProduto()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var atualizarProdutoDto = _fixture.Create<AtualizarProdutoDto>();
        var operationResult = OperationResult<Guid>.Success(produtoId);

        _atualizarProdutoUseCaseMock.Setup(x => x.Handle(atualizarProdutoDto)).ReturnsAsync(operationResult);
        // Act
        var resultado = await _produtoController.Atualizar(produtoId, atualizarProdutoDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().Be(operationResult);
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoRemoverProduto()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var operationResult = OperationResult<Guid>.Success(produtoId);

        _removerProdutoUseCaseMock.Setup(x => x.Handle(produtoId)).ReturnsAsync(operationResult);
        // Act
        var resultado = await _produtoController.Remover(produtoId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().Be(operationResult);
    }
}
