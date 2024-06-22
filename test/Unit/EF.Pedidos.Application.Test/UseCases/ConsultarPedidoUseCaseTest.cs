using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Application.UseCases;
using EF.Pedidos.Domain.Repository;
using Moq;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Domain.Models;
using FluentAssertions;
using EF.Pedidos.Application.DTOs.Responses;

namespace EF.Pedidos.Application.Test.UseCases;

public class ConsultarPedidoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly IConsultarPedidoUseCase _consultarPedidoUseCase;

    public ConsultarPedidoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _pedidoRepository = _fixture.Freeze<Mock<IPedidoRepository>>();
        _consultarPedidoUseCase = _fixture.Create<ConsultarPedidoUseCase>();
    }

    [Fact]
    public async Task DeveObterPedidoPorId()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        pedido.AdicionarItem(_fixture.Create<Item>());

        _pedidoRepository.Setup(x => x.ObterPorId(pedido.Id)).ReturnsAsync(pedido);

        // Act
        var resultado = await _consultarPedidoUseCase.ObterPedidoPorId(pedido.Id);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPorId(pedido.Id), Times.Once);
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(pedido.Id);
    }

    [Fact]
    public async Task DeveObterPedidoNulo()
    {
        // Arrange
        var pedidoId = _fixture.Create<Guid>();

        _pedidoRepository.Setup(x => x.ObterPorId(pedidoId))!.ReturnsAsync((Pedido?)null);

        // Act
        var resultado = await _consultarPedidoUseCase.ObterPedidoPorId(pedidoId);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPorId(pedidoId), Times.Once);
        resultado.Should().BeNull();
    }
}
