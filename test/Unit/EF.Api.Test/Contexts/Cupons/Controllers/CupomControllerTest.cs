using AutoFixture.AutoMoq;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Api.Contexts.Cupons.Controllers;
using EF.Core.Commons.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using EF.Cupons.Application.DTOs.Responses;
using EF.Cupons.Application.DTOs.Requests;

namespace EF.Api.Test.Contexts.Cupons.Controllers;

public class CupomControllerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IAtualizarCupomUseCase> _atualizarCupomUseCaseMock;
    private readonly Mock<IConsultarCupomUseCase> _consultarCupomUseCaseMock;
    private readonly Mock<ICriarCupomUseCase> _criarCupomUseCaseMock;
    private readonly Mock<IInativarCupomUseCase> _inativarCupomUseCaseMock;
    private readonly Mock<IInserirProdutoUseCase> _inserirProdutoUseCaseMock;
    private readonly Mock<IRemoverProdutoUseCase> _removerProdutoUseCaseMock;
    private readonly CupomController _cupomController;

    public CupomControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
        _atualizarCupomUseCaseMock = _fixture.Freeze<Mock<IAtualizarCupomUseCase>>();                
        _consultarCupomUseCaseMock = _fixture.Freeze<Mock<IConsultarCupomUseCase>>();
        _criarCupomUseCaseMock = _fixture.Freeze<Mock<ICriarCupomUseCase>>();
        _inativarCupomUseCaseMock = _fixture.Freeze<Mock<IInativarCupomUseCase>>();
        _inserirProdutoUseCaseMock = _fixture.Freeze<Mock<IInserirProdutoUseCase>>();
        _removerProdutoUseCaseMock = _fixture.Freeze<Mock<IRemoverProdutoUseCase>>();
        _cupomController = _fixture.Create<CupomController>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoBuscarCupomProduto()
    {
        // Arrange
        var codigoCupom = _fixture.Create<string>();
        var cupomDto = _fixture.Create<CupomDto>();

        _consultarCupomUseCaseMock.Setup(x => x.ObterCupom(codigoCupom, default)).ReturnsAsync(cupomDto);
        // Act
        var resultado = await _cupomController.BuscarCupomProduto(codigoCupom, default);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().Be(cupomDto);
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoCriarCupom()
    {      
        // Arrange
        var criarCupomDto = _fixture.Create<CriarCupomDto>();
        var operationResult = OperationResult<Guid>.Success(Guid.NewGuid());

        _criarCupomUseCaseMock.Setup(x => x.Handle(It.IsAny<CriarCupomDto>())).ReturnsAsync(operationResult);

        // Act
        var resultado = await _cupomController.CriarCupom(criarCupomDto, default);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(operationResult);
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoAtualizarCupom()
    {
        // Arrange
        var cupomId = Guid.NewGuid();
        var atualizarCupomDto = _fixture.Create<AtualizarCupomDto>();
        var operationResult = OperationResult<Guid>.Success(Guid.NewGuid());

        _atualizarCupomUseCaseMock.Setup(x => x.Handle(atualizarCupomDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _cupomController.AtualizarCupom(cupomId, atualizarCupomDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(operationResult);
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoInativarCupom()
    {
        // Arrange
        var cupomId = Guid.NewGuid();
        var operationResult = OperationResult<Guid>.Success(Guid.NewGuid());

        _inativarCupomUseCaseMock.Setup(x => x.Handle(cupomId)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _cupomController.InativarCupom(cupomId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(operationResult);
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoRemoverProdutosDoCupom()
    {
        // Arrange
        var cupomId = Guid.NewGuid();
        var removerCupomProdutoDto = _fixture.CreateMany<AdicionarRemoverCupomProdutoDto>(5).ToList();
        var operationResult = OperationResult<Guid>.Success(Guid.NewGuid());

        _removerProdutoUseCaseMock.Setup(x => x.Handle(It.IsAny<RemoverProdutoDto>())).ReturnsAsync(operationResult);

        // Act
        var resultado = await _cupomController.RemoverCupomProduto(cupomId, removerCupomProdutoDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(operationResult);
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoInserirProdutosDoCupom()
    {
        // Arrange
        var cupomId = Guid.NewGuid();
        var inserirCupomProdutoDto = _fixture.CreateMany<AdicionarRemoverCupomProdutoDto>(5).ToList();
        var operationResult = OperationResult<Guid>.Success(Guid.NewGuid());

        _inserirProdutoUseCaseMock.Setup(x => x.Handle(It.IsAny<InserirProdutoDto>())).ReturnsAsync(operationResult);

        // Act
        var resultado = await _cupomController.InserirCupomProduto(cupomId, inserirCupomProdutoDto, default);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(operationResult);
    }
}
