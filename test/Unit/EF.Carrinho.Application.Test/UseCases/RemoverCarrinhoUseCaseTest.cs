using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Application.UseCases;
using EF.Carrinho.Domain.Repository;
using Moq;
using EF.Carrinho.Domain.Models;

namespace EF.Carrinho.Application.Test.UseCases;

public class RemoverCarrinhoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<ICarrinhoRepository> _carrinhoRepositoryMock;
    private readonly IRemoverCarrinhoUseCase _removerCarrinhoUseCase;

    public RemoverCarrinhoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _carrinhoRepositoryMock = _fixture.Freeze<Mock<ICarrinhoRepository>>();
        _removerCarrinhoUseCase = _fixture.Create<RemoverCarrinhoUseCase>();
    }

    [Fact]
    public async Task DeveRemoverCarrinhoPorId()
    {
        // Arrange
        var carrinho = _fixture.Create<CarrinhoCliente>();
        _carrinhoRepositoryMock.Setup(x => x.ObterPorId(carrinho.Id)).ReturnsAsync(carrinho);
        _carrinhoRepositoryMock.Setup(x => x.Remover(It.IsAny<CarrinhoCliente>()));

        // Act
        await _removerCarrinhoUseCase.RemoverCarrinho(carrinho.Id);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.Remover(It.IsAny<CarrinhoCliente>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task DeveRemoverCarrinhoPorClienteId()
    {
        // Arrange
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AssociarCliente(_fixture.Create<Guid>());
        _carrinhoRepositoryMock.Setup(x => x.ObterPorId(carrinho.ClienteId!.Value)).ReturnsAsync(carrinho);
        _carrinhoRepositoryMock.Setup(x => x.Remover(It.IsAny<CarrinhoCliente>()));

        // Act
        await _removerCarrinhoUseCase.RemoverCarrinhoPorClienteId(carrinho.ClienteId!.Value);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.Remover(It.IsAny<CarrinhoCliente>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
    }
}
