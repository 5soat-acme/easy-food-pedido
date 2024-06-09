using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Application.UseCases;
using EF.Pedidos.Domain.Repository;
using Moq;
using EF.Pedidos.Application.Gateways;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Domain.Models;
using FluentAssertions;
using EF.Pedidos.Application.DTOs.Gateways;
using EF.Core.Commons.DomainObjects;

namespace EF.Pedidos.Application.Test.UseCases;

public class ReceberPedidoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly Mock<IProdutoService> _produtoService;
    private readonly IReceberPedidoUsecase _receberPedidoUseCase;

    public ReceberPedidoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _pedidoRepository = _fixture.Freeze<Mock<IPedidoRepository>>();
        _produtoService = _fixture.Freeze<Mock<IProdutoService>>();
        _receberPedidoUseCase = _fixture.Create<ReceberPedidoUsecase>();
    }

    [Fact]
    public async Task DeveReceberPedido()
    {
        // Arrange
        var receberPedidoDto = _fixture.Create<ReceberPedidoDto>();
        var pedido = _fixture.Create<Pedido>();
        var item = _fixture.Create<Item>();
        pedido.AdicionarItem(item);
        var produtoDto = _fixture.Build<ProdutoDto>().With(x => x.Id, item.ProdutoId).Create();

        _pedidoRepository.Setup(x => x.ObterPorId(receberPedidoDto.PedidoId)).ReturnsAsync(pedido);
        _produtoService.Setup(x => x.ObterPorId(item.ProdutoId)).ReturnsAsync(produtoDto);
        _pedidoRepository.Setup(x => x.Atualizar(pedido));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _receberPedidoUseCase.Handle(receberPedidoDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPorId(receberPedidoDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(pedido), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoRecceberPedidoInexistente()
    {
        // Arrange
        var receberPedidoDto = _fixture.Create<ReceberPedidoDto>();

        _pedidoRepository.Setup(x => x.ObterPorId(receberPedidoDto.PedidoId))!.ReturnsAsync((Pedido?) null);

        // Act
        Func<Task> act = async () => await _receberPedidoUseCase.Handle(receberPedidoDto);

        // Assert
        await act.Should().ThrowAsync<DomainException>().WithMessage("Pedido inválido");
        _pedidoRepository.Verify(x => x.ObterPorId(receberPedidoDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Never);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Never);        
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var receberPedidoDto = _fixture.Create<ReceberPedidoDto>();
        var pedido = _fixture.Create<Pedido>();
        var item = _fixture.Create<Item>();
        pedido.AdicionarItem(item);
        var produtoDto = _fixture.Build<ProdutoDto>().With(x => x.Id, item.ProdutoId).Create();

        _pedidoRepository.Setup(x => x.ObterPorId(receberPedidoDto.PedidoId)).ReturnsAsync(pedido);
        _produtoService.Setup(x => x.ObterPorId(item.ProdutoId)).ReturnsAsync(produtoDto);
        _pedidoRepository.Setup(x => x.Atualizar(pedido));
        _pedidoRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        // Act
        var resultado = await _receberPedidoUseCase.Handle(receberPedidoDto);

        // Assert
        _pedidoRepository.Verify(x => x.ObterPorId(receberPedidoDto.PedidoId), Times.Once);
        _pedidoRepository.Verify(x => x.Atualizar(pedido), Times.Once);
        _pedidoRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
