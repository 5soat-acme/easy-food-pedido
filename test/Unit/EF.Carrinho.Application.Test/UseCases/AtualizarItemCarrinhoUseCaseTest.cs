using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Carrinho.Application.DTOs.Requests;
using EF.Carrinho.Application.Gateways;
using EF.Carrinho.Application.UseCases;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Domain.Models;
using EF.Carrinho.Domain.Repository;
using EF.Core.Commons.DomainObjects;
using FluentAssertions;
using Moq;

namespace EF.Carrinho.Application.Test.UseCases;

public class AtualizarItemCarrinhoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<ICarrinhoRepository> _carrinhoRepositoryMock;
    private readonly Mock<IEstoqueService> _estoqueServiceMock;
    private readonly Mock<IConsultarCarrinhoUseCase> _consultarCarrinhoUseCaseMock;
    private readonly IAtualizarItemCarrinhoUseCase _atualizarItemCarrinhoUseCase;

    public AtualizarItemCarrinhoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _carrinhoRepositoryMock = _fixture.Freeze<Mock<ICarrinhoRepository>>();
        _estoqueServiceMock = _fixture.Freeze<Mock<IEstoqueService>>();
        _consultarCarrinhoUseCaseMock = _fixture.Freeze<Mock<IConsultarCarrinhoUseCase>>();
        _atualizarItemCarrinhoUseCase = _fixture.Create<AtualizarItemCarrinhoUseCase>();
    }

    [Fact]
    public async Task DeveAtualizarItem()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var item = _fixture.Create<Item>();
        var atualizarItemDto = new AtualizarItemDto()
        {
            ItemId = item.Id,
            Quantidade = _fixture.Create<int>()
        };
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AdicionarItem(item);
        carrinho.AssociarCarrinho(carrinhoSessao.CarrinhoId);

        _consultarCarrinhoUseCaseMock.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);
        _estoqueServiceMock.Setup(x => x.VerificarEstoque(item.ProdutoId, atualizarItemDto.Quantidade)).ReturnsAsync(true);
        _carrinhoRepositoryMock.Setup(x => x.Atualizar(It.IsAny<CarrinhoCliente>()));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _atualizarItemCarrinhoUseCase.AtualizarItem(atualizarItemDto, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<CarrinhoCliente>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoNaoExistirCarrinho()
    {
        // Arrange
        _consultarCarrinhoUseCaseMock.Setup(x => x.ObterCarrinho(It.IsAny<CarrinhoSessaoDto>())).ReturnsAsync((CarrinhoCliente?)null);

        // Act
        var resultado = await _atualizarItemCarrinhoUseCase.AtualizarItem(It.IsAny<AtualizarItemDto>(), It.IsAny<CarrinhoSessaoDto>());

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<CarrinhoCliente>()), Times.Never);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.GetErrorMessages().Count(x => x == "O carrinho está vazio").Should().Be(1);
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoNaoExistirItemNoCarrinho()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var item = _fixture.Create<Item>();
        var atualizarItemDto = new AtualizarItemDto()
        {
            ItemId = item.Id,
            Quantidade = _fixture.Create<int>()
        };
        var carrinho = _fixture.Create<CarrinhoCliente>();

        _consultarCarrinhoUseCaseMock.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);

        // Act
        Func<Task> act = async () => await _atualizarItemCarrinhoUseCase.AtualizarItem(atualizarItemDto, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<CarrinhoCliente>()), Times.Never);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        await act.Should().ThrowAsync<DomainException>().WithMessage("Item não encontrado");
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoProdutoNaoPossuirEstoque()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var item = _fixture.Create<Item>();
        var atualizarItemDto = new AtualizarItemDto()
        {
            ItemId = item.Id,
            Quantidade = _fixture.Create<int>()
        };
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AdicionarItem(item);

        _consultarCarrinhoUseCaseMock.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);
        _estoqueServiceMock.Setup(x => x.VerificarEstoque(item.ProdutoId, atualizarItemDto.Quantidade)).ReturnsAsync(false);
        _carrinhoRepositoryMock.Setup(x => x.Atualizar(It.IsAny<CarrinhoCliente>()));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _atualizarItemCarrinhoUseCase.AtualizarItem(atualizarItemDto, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<CarrinhoCliente>()), Times.Never);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.GetErrorMessages().Count(x => x == "Produto sem estoque").Should().Be(1);
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var item = _fixture.Create<Item>();
        var atualizarItemDto = new AtualizarItemDto()
        {
            ItemId = item.Id,
            Quantidade = _fixture.Create<int>()
        };
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AdicionarItem(item);

        _consultarCarrinhoUseCaseMock.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);
        _estoqueServiceMock.Setup(x => x.VerificarEstoque(item.ProdutoId, atualizarItemDto.Quantidade)).ReturnsAsync(true);
        _carrinhoRepositoryMock.Setup(x => x.Atualizar(It.IsAny<CarrinhoCliente>()));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        // Act
        var resultado = await _atualizarItemCarrinhoUseCase.AtualizarItem(atualizarItemDto, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<CarrinhoCliente>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
