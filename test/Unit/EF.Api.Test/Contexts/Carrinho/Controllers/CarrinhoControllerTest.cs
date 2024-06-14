using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Api.Contexts.Carrinho.Controllers;
using EF.Carrinho.Application.DTOs.Requests;
using EF.Carrinho.Application.DTOs.Responses;
using EF.Carrinho.Application.UseCases;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Core.Commons.Communication;
using EF.WebApi.Commons.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace EF.Api.Test.Contexts.Carrinho.Controllers;

public class CarrinhoControllerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserApp> _userAppMock;
    private readonly Mock<IAdicionarItemCarrinhoUseCase> _adicionarItemCarrinhoUseCaseMock;
    private readonly Mock<IAtualizarItemCarrinhoUseCase> _atualizarItemCarrinhoUseCaseMock;
    private readonly Mock<IConsultarCarrinhoUseCase> _consultarCarrinhoUseCaseMock;
    private readonly Mock<IRemoverItemCarrinhoUseCase> _removerItemCarrinhoUseCaseMock;
    private readonly CarrinhoController _carrinhoController;

    public CarrinhoControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
        _userAppMock = _fixture.Freeze<Mock<IUserApp>>();
        _userAppMock.Setup(x => x.GetSessionId()).Returns(Guid.NewGuid());
        _userAppMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _adicionarItemCarrinhoUseCaseMock = _fixture.Freeze<Mock<IAdicionarItemCarrinhoUseCase>>();
        _atualizarItemCarrinhoUseCaseMock = _fixture.Freeze<Mock<IAtualizarItemCarrinhoUseCase>>();
        _consultarCarrinhoUseCaseMock = _fixture.Freeze<Mock<IConsultarCarrinhoUseCase>>();
        _removerItemCarrinhoUseCaseMock = _fixture.Freeze<Mock<IRemoverItemCarrinhoUseCase>>();
        _carrinhoController = _fixture.Create<CarrinhoController>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoObterCarrinho()
    {
        // Arrange
        var carrinhoDto = _fixture.Create<CarrinhoClienteDto>();
                
        _consultarCarrinhoUseCaseMock.Setup(x => x.ConsultarCarrinho(It.IsAny<CarrinhoSessaoDto>())).ReturnsAsync(carrinhoDto);

        // Act
        var resultado = await _carrinhoController.ObterCarrinho();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().Be(carrinhoDto);
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoAdicionarItemAoCarrinho()
    {
        // Arrange
        var adicionarItemDto = _fixture.Create<AdicionarItemDto>();
        var operationResult = OperationResult.Success();

        _adicionarItemCarrinhoUseCaseMock.Setup(x => x.AdicionarItemCarrinho(adicionarItemDto, It.IsAny<CarrinhoSessaoDto>())).ReturnsAsync(operationResult);

        // Act
        var resultado = await _carrinhoController.AdicionarItem(adicionarItemDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoModelStateFalharAoAdicionarItemAoCarrinho()
    {
        // Arrange
        var adicionarItemDto = _fixture.Create<AdicionarItemDto>();
        _carrinhoController.ModelState.AddModelError("Error", "Error model state");

        // Act
        var resultado = await _carrinhoController.AdicionarItem(adicionarItemDto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequestResult.Value.Should().BeEquivalentTo(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", _carrinhoController.ModelState.Values.SelectMany(e => e.Errors).Select(x => x.ErrorMessage).ToArray() }
        }));
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoFalharAoAdicionarItemAoCarrinho()
    {
        // Arrange
        var adicionarItemDto = _fixture.Create<AdicionarItemDto>();
        var operationResult = OperationResult.Failure("Erro");

        _adicionarItemCarrinhoUseCaseMock.Setup(x => x.AdicionarItemCarrinho(adicionarItemDto, It.IsAny<CarrinhoSessaoDto>())).ReturnsAsync(operationResult);

        // Act
        var resultado = await _carrinhoController.AdicionarItem(adicionarItemDto);

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
    public async Task DeveRetornarOk_QuandoAtualizarItemDoCarrinho()
    {
        // Arrange
        var atualizarItemDto = _fixture.Create<AtualizarItemDto>();
        var itemId = atualizarItemDto.ItemId;        
        var operationResult = OperationResult.Success();

        _atualizarItemCarrinhoUseCaseMock.Setup(x => x.AtualizarItem(atualizarItemDto, It.IsAny<CarrinhoSessaoDto>())).ReturnsAsync(operationResult);

        // Act
        var resultado = await _carrinhoController.AtualizarItem(itemId, atualizarItemDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoIdDoItemForDiferenteAoAtualizarItemDoCarrinho()
    {
        // Arrange
        var atualizarItemDto = _fixture.Create<AtualizarItemDto>();
        var itemId = Guid.NewGuid();

        // Act
        var resultado = await _carrinhoController.AtualizarItem(itemId, atualizarItemDto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequestResult.Value.Should().BeEquivalentTo(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", ["O item não corresponde ao informado"] }
        }));
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoModelStateFalharAoAtualizarItemDoCarrinho()
    {
        // Arrange
        var atualizarItemDto = _fixture.Create<AtualizarItemDto>();
        var itemId = atualizarItemDto.ItemId;
        _carrinhoController.ModelState.AddModelError("Error", "Error model state");

        // Act
        var resultado = await _carrinhoController.AtualizarItem(itemId, atualizarItemDto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequestResult.Value.Should().BeEquivalentTo(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", _carrinhoController.ModelState.Values.SelectMany(e => e.Errors).Select(x => x.ErrorMessage).ToArray() }
        }));
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoFalharAoAtualizarItemDoCarrinho()
    {
        // Arrange
        var atualizarItemDto = _fixture.Create<AtualizarItemDto>();
        var itemId = atualizarItemDto.ItemId;
        var operationResult = OperationResult.Failure("Erro");

        _atualizarItemCarrinhoUseCaseMock.Setup(x => x.AtualizarItem(atualizarItemDto, It.IsAny<CarrinhoSessaoDto>())).ReturnsAsync(operationResult);

        // Act
        var resultado = await _carrinhoController.AtualizarItem(itemId, atualizarItemDto);

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
    public async Task DeveRetornarOk_QuandoRemoverItemDoCarrinho()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var operationResult = OperationResult.Success();

        _removerItemCarrinhoUseCaseMock.Setup(x => x.RemoverItemCarrinho(itemId, It.IsAny<CarrinhoSessaoDto>())).ReturnsAsync(operationResult);

        // Act
        var resultado = await _carrinhoController.RemoverItem(itemId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }
}
