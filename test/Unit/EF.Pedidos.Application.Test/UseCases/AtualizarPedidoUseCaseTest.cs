using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Application.UseCases;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Domain.Models;
using EF.Pedidos.Domain.Repository;
using FluentAssertions;
using Moq;

namespace EF.Pedidos.Application.Test.UseCases;

public class AtualizarPedidoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly IAtualizarPedidoUseCase _atualizarPedidoUseCase;

    public AtualizarPedidoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _pedidoRepository = _fixture.Freeze<Mock<IPedidoRepository>>();
        _atualizarPedidoUseCase = _fixture.Create<AtualizarPedidoUseCase>();
    }

    [Fact]
    public async Task DeveAtualizarPedido()
    {
        // Arrange
        var atualizarPedidoDto = _fixture.Create<AtualizarPedidoDto>();
        var pedido = _fixture.Build<Pedido>().With(x => x.Id, atualizarPedidoDto.PedidoId).Create();

        _pedidoRepository.Setup(x => x.ObterPorId(atualizarPedidoDto.PedidoId)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(x => x.Atualizar(pedido));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _atualizarPedidoUseCase.Handle(atualizarPedidoDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPorId(atualizarPedidoDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(pedido), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var atualizarPedidoDto = _fixture.Create<AtualizarPedidoDto>();
        var pedido = _fixture.Build<Pedido>().With(x => x.Id, atualizarPedidoDto.PedidoId).Create();

        _pedidoRepository.Setup(x => x.ObterPorId(atualizarPedidoDto.PedidoId)).ReturnsAsync(pedido);
        _pedidoRepository.Setup(x => x.Atualizar(pedido));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        // Act
        var resultado = await _atualizarPedidoUseCase.Handle(atualizarPedidoDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPorId(atualizarPedidoDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(pedido), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
