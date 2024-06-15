using AutoFixture.AutoMoq;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using EF.Api.Contexts.Pedidos.Controllers;
using EF.Pedidos.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EF.Pedidos.Application.DTOs.Responses;
using FluentAssertions;
using EF.Pedidos.Application.DTOs.Requests;
using EF.WebApi.Commons.Users;
using EF.Core.Commons.Communication;

namespace EF.Api.Test.Contexts.Pedidos.Controllers;

public class PedidoControllerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IConsultarPedidoUseCase> _consultarPedidoUseCaseMock;
    private readonly Mock<ICriarPedidoUseCase> _criarPedidoUseCaseMock;
    private readonly Mock<IUserApp> _userAppMock;
    private readonly PedidoController _pedidoController;

    public PedidoControllerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
        _consultarPedidoUseCaseMock = _fixture.Freeze<Mock<IConsultarPedidoUseCase>>();
        _criarPedidoUseCaseMock = _fixture.Freeze<Mock<ICriarPedidoUseCase>>();
        _userAppMock = _fixture.Freeze<Mock<IUserApp>>();
        _pedidoController = _fixture.Create<PedidoController>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoObterPedidoPorId()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedido = _fixture.Create<PedidoDto>();

        _consultarPedidoUseCaseMock.Setup(x => x.ObterPedidoPorId(pedidoId)).ReturnsAsync(pedido);

        // Act
        var resultado = await _pedidoController.ObterPedido(pedidoId);

        // Assert
        _consultarPedidoUseCaseMock.Verify(x => x.ObterPedidoPorId(pedidoId), Times.Once);
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(pedido);
    }

    [Fact]
    public async Task DeveRetornarNotFound_QuandoNaoHouverPedidoPorId()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        _consultarPedidoUseCaseMock.Setup(x => x.ObterPedidoPorId(pedidoId)).ReturnsAsync((PedidoDto?)null);

        // Act
        var result = await _pedidoController.ObterPedido(pedidoId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeveRetornarOk_QuandoFizerCheckoutPedido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var criarPedidoDto = _fixture.Create<CriarPedidoDto>();
        var operationResult = OperationResult<Guid>.Success(pedidoId);

        _userAppMock.Setup(x => x.GetSessionId()).Returns(Guid.NewGuid());
        _userAppMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _userAppMock.Setup(x => x.GetUserCpf()).Returns(_fixture.Create<string>());
        _criarPedidoUseCaseMock.Setup(x => x.Handle(criarPedidoDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _pedidoController.Checkout(criarPedidoDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(new { pedidoId = pedidoId });
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoFalharCheckoutPedido()
    {
        // Arrange
        var criarPedidoDto = _fixture.Create<CriarPedidoDto>();
        var operationResult = OperationResult<Guid>.Failure("Erro ao efetuar checkout");

        _userAppMock.Setup(x => x.GetSessionId()).Returns(Guid.NewGuid());
        _userAppMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        _userAppMock.Setup(x => x.GetUserCpf()).Returns(_fixture.Create<string>());
        _criarPedidoUseCaseMock.Setup(x => x.Handle(criarPedidoDto)).ReturnsAsync(operationResult);

        // Act
        var resultado = await _pedidoController.Checkout(criarPedidoDto);

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
