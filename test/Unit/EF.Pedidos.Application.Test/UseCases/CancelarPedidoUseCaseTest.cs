using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Domain.Repository;
using Moq;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Domain.Models;
using FluentAssertions;
using EF.Pedidos.Application.UseCases;
using EF.Core.Commons.DomainObjects;

namespace EF.Pedidos.Application.Test.UseCases;

public class CancelarPedidoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly ICancelarPedidoUseCase _cancelarPedidoUseCase;

    public CancelarPedidoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _pedidoRepository = _fixture.Freeze<Mock<IPedidoRepository>>();
        _cancelarPedidoUseCase = _fixture.Create<CancelarPedidoUseCase>();
    }

    [Fact]
    public async Task DeveCancelarPedido()
    {
        // Arrange
        var cancelarPedidoDto = _fixture.Create<CancelarPedidoDto>();
        var pedido = _fixture.Create<Pedido>();
        var item = _fixture.Create<Item>();
        pedido.AdicionarItem(item);

        _pedidoRepository.Setup(x => x.ObterPorId(cancelarPedidoDto.PedidoId)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(x => x.Atualizar(pedido));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _cancelarPedidoUseCase.Handle(cancelarPedidoDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPorId(cancelarPedidoDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(pedido), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoRecceberPedidoInexistente()
    {
        // Arrange
        var cancelarPedidoDto = _fixture.Create<CancelarPedidoDto>();

        _pedidoRepository.Setup(x => x.ObterPorId(cancelarPedidoDto.PedidoId))!.ReturnsAsync((Pedido?)null);

        // Act
        Func<Task> act = async () => await _cancelarPedidoUseCase.Handle(cancelarPedidoDto);

        // Assert
        await act.Should().ThrowAsync<DomainException>().WithMessage("Pedido inválido");
        _pedidoRepository.Verify(x => x.ObterPorId(cancelarPedidoDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Never);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Never);
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var cancelarPedidoDto = _fixture.Create<CancelarPedidoDto>();
        var pedido = _fixture.Create<Pedido>();
        var item = _fixture.Create<Item>();
        pedido.AdicionarItem(item);

        _pedidoRepository.Setup(x => x.ObterPorId(cancelarPedidoDto.PedidoId)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(x => x.Atualizar(pedido));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        // Act
        var resultado = await _cancelarPedidoUseCase.Handle(cancelarPedidoDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPorId(cancelarPedidoDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(pedido), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}