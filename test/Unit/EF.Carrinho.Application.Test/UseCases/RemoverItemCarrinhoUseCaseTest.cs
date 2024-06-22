using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Application.UseCases;
using EF.Carrinho.Domain.Repository;
using Moq;
using EF.Carrinho.Domain.Models;
using EF.Carrinho.Application.DTOs.Requests;
using FluentAssertions;

namespace EF.Carrinho.Application.Test.UseCases;

public class RemoverItemCarrinhoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<ICarrinhoRepository> _carrinhoRepositoryMock;
    private readonly Mock<IConsultarCarrinhoUseCase> _consultarCarrinhoUseCase;
    private readonly IRemoverItemCarrinhoUseCase _removerItemCarrinhoUseCase;

    public RemoverItemCarrinhoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _carrinhoRepositoryMock = _fixture.Freeze<Mock<ICarrinhoRepository>>();
        _consultarCarrinhoUseCase = _fixture.Freeze<Mock<IConsultarCarrinhoUseCase>>();
        _removerItemCarrinhoUseCase = _fixture.Create<RemoverItemCarrinhoUseCase>();
    }

    [Fact]
    public async Task DeveRemoverItemCarrinho()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var carrinho = _fixture.Create<CarrinhoCliente>();
        var item1 = _fixture.Create<Item>();
        var item2 = _fixture.Create<Item>();
        carrinho.AdicionarItem(item1);
        carrinho.AdicionarItem(item2);

        _consultarCarrinhoUseCase.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);
        _carrinhoRepositoryMock.Setup(x => x.RemoverItem(item1));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _removerItemCarrinhoUseCase.RemoverItemCarrinho(item1.Id, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.RemoverItem(item1), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<CarrinhoCliente>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveRemoverItemCarrinhoECarrinho()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var carrinho = _fixture.Create<CarrinhoCliente>();
        var item = _fixture.Create<Item>();
        carrinho.AdicionarItem(item);

        _consultarCarrinhoUseCase.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);
        _carrinhoRepositoryMock.Setup(x => x.RemoverItem(item));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _removerItemCarrinhoUseCase.RemoverItemCarrinho(item.Id, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.RemoverItem(item), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.Remover(It.IsAny<CarrinhoCliente>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcessao_QuandoCarrinhoNAoExistir()
    {
        // Arrange
        var itemId = _fixture.Create<Guid>();
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();

        _consultarCarrinhoUseCase.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync((CarrinhoCliente?)null);
        
        // Act
        var resultado = await _removerItemCarrinhoUseCase.RemoverItemCarrinho(itemId, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.RemoverItem(It.IsAny<Item>()), Times.Never);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.GetErrorMessages().Count(x => x == "Carrinho não encontrado").Should().Be(1);
    }

    [Fact]
    public async Task DeveRetornarSucesso_QuandoRemoverItemNaoExistenteNoCarrinho()
    {
        // Arrange
        var itemId = _fixture.Create<Guid>();
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var carrinho = _fixture.Create<CarrinhoCliente>();

        _consultarCarrinhoUseCase.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);

        // Act
        var resultado = await _removerItemCarrinhoUseCase.RemoverItemCarrinho(itemId, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.RemoverItem(It.IsAny<Item>()), Times.Never);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var carrinho = _fixture.Create<CarrinhoCliente>();
        var item1 = _fixture.Create<Item>();
        var item2 = _fixture.Create<Item>();
        carrinho.AdicionarItem(item1);
        carrinho.AdicionarItem(item2);

        _consultarCarrinhoUseCase.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);
        _carrinhoRepositoryMock.Setup(x => x.RemoverItem(item1));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        // Act
        var resultado = await _removerItemCarrinhoUseCase.RemoverItemCarrinho(item1.Id, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.RemoverItem(item1), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<CarrinhoCliente>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
    }
}
