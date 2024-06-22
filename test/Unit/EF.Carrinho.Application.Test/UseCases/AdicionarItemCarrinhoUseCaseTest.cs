using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Application.UseCases;
using EF.Carrinho.Domain.Repository;
using EF.Carrinho.Application.Gateways;
using EF.Carrinho.Domain.Models;
using EF.Carrinho.Application.DTOs.Requests;
using FluentAssertions;
using EF.Core.Commons.DomainObjects;

namespace EF.Carrinho.Application.Test.UseCases;

public class AdicionarItemCarrinhoUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<ICarrinhoRepository> _carrinhoRepositoryMock;
    private readonly Mock<IEstoqueService> _estoqueServiceMock;
    private readonly Mock<IProdutoService> _produtoServiceMock;
    private readonly Mock<IConsultarCarrinhoUseCase> _consultarCarrinhoUseCaseMock;
    private readonly IAdicionarItemCarrinhoUseCase _adicionarItemCarrinhoUseCase;

    public AdicionarItemCarrinhoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _carrinhoRepositoryMock = _fixture.Freeze<Mock<ICarrinhoRepository>>();
        _estoqueServiceMock = _fixture.Freeze<Mock<IEstoqueService>>();
        _produtoServiceMock = _fixture.Freeze<Mock<IProdutoService>>();
        _consultarCarrinhoUseCaseMock = _fixture.Freeze<Mock<IConsultarCarrinhoUseCase>>();
        _adicionarItemCarrinhoUseCase = _fixture.Create<AdicionarItemCarrinhoUseCase>();
    }

    [Fact]
    public async Task DeveAdicionarItemECriarCarrinho()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var item = _fixture.Create<Item>();
        var addItemDto = new AdicionarItemDto()
        {
            ProdutoId = item.ProdutoId,
            Quantidade = _fixture.Create<int>()
        };

        _produtoServiceMock.Setup(x => x.ObterItemPorProdutoId(addItemDto.ProdutoId)).ReturnsAsync(item);
        _consultarCarrinhoUseCaseMock.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync((CarrinhoCliente?)null);
        _carrinhoRepositoryMock.Setup(x => x.AdicionarItem(It.IsAny<Item>()));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);
        _estoqueServiceMock.Setup(x => x.VerificarEstoque(addItemDto.ProdutoId, addItemDto.Quantidade)).ReturnsAsync(true);

        // Act
        var resultado = await _adicionarItemCarrinhoUseCase.AdicionarItemCarrinho(addItemDto, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.Criar(It.IsAny<CarrinhoCliente>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveAdicionarItem()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var item = _fixture.Create<Item>();
        var addItemDto = new AdicionarItemDto(){
            ProdutoId = item.ProdutoId,
            Quantidade = _fixture.Create<int>()
        };
        var carrinho = _fixture.Create<CarrinhoCliente>();

        _produtoServiceMock.Setup(x => x.ObterItemPorProdutoId(addItemDto.ProdutoId)).ReturnsAsync(item);
        _consultarCarrinhoUseCaseMock.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);
        _carrinhoRepositoryMock.Setup(x => x.AdicionarItem(It.IsAny<Item>()));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);
        _estoqueServiceMock.Setup(x => x.VerificarEstoque(addItemDto.ProdutoId, addItemDto.Quantidade)).ReturnsAsync(true);

        // Act
        var resultado = await _adicionarItemCarrinhoUseCase.AdicionarItemCarrinho(addItemDto, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.AdicionarItem(It.IsAny<Item>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }    

    [Fact]
    public async Task DeveAtualizarItem()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var item = _fixture.Create<Item>();
        var addItemDto = new AdicionarItemDto()
        {
            ProdutoId = item.ProdutoId,
            Quantidade = _fixture.Create<int>()
        };
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AdicionarItem(item);

        _produtoServiceMock.Setup(x => x.ObterItemPorProdutoId(addItemDto.ProdutoId)).ReturnsAsync(item);
        _consultarCarrinhoUseCaseMock.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);
        _carrinhoRepositoryMock.Setup(x => x.AdicionarItem(It.IsAny<Item>()));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);
        _estoqueServiceMock.Setup(x => x.VerificarEstoque(addItemDto.ProdutoId, addItemDto.Quantidade)).ReturnsAsync(true);

        // Act
        var resultado = await _adicionarItemCarrinhoUseCase.AdicionarItemCarrinho(addItemDto, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.AtualizarItem(It.IsAny<Item>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoProdutoForInvalido()
    {
        // Arrange
        var itemDto = _fixture.Create<AdicionarItemDto>();
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        _produtoServiceMock.Setup(p => p.ObterItemPorProdutoId(itemDto.ProdutoId)).ReturnsAsync((Item?)null);

        // Act
        Func<Task> act = async () => await _adicionarItemCarrinhoUseCase.AdicionarItemCarrinho(itemDto, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        await act.Should().ThrowAsync<DomainException>().WithMessage("Produto inválido");
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoProdutoNaoPossuirEstoque()
    {
        // Arrange
        var carrinhoSessao = _fixture.Create<CarrinhoSessaoDto>();
        var item = _fixture.Create<Item>();
        var addItemDto = new AdicionarItemDto()
        {
            ProdutoId = item.ProdutoId,
            Quantidade = _fixture.Create<int>()
        };
        var carrinho = _fixture.Create<CarrinhoCliente>();

        _produtoServiceMock.Setup(x => x.ObterItemPorProdutoId(addItemDto.ProdutoId)).ReturnsAsync(item);
        _consultarCarrinhoUseCaseMock.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);
        _carrinhoRepositoryMock.Setup(x => x.AdicionarItem(It.IsAny<Item>()));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);
        _estoqueServiceMock.Setup(x => x.VerificarEstoque(addItemDto.ProdutoId, addItemDto.Quantidade)).ReturnsAsync(false);

        // Act
        var resultado = await _adicionarItemCarrinhoUseCase.AdicionarItemCarrinho(addItemDto, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.AdicionarItem(It.IsAny<Item>()), Times.Once);
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
        var addItemDto = new AdicionarItemDto()
        {
            ProdutoId = item.ProdutoId,
            Quantidade = _fixture.Create<int>()
        };
        var carrinho = _fixture.Create<CarrinhoCliente>();

        _produtoServiceMock.Setup(x => x.ObterItemPorProdutoId(addItemDto.ProdutoId)).ReturnsAsync(item);
        _consultarCarrinhoUseCaseMock.Setup(x => x.ObterCarrinho(carrinhoSessao)).ReturnsAsync(carrinho);
        _carrinhoRepositoryMock.Setup(x => x.AdicionarItem(It.IsAny<Item>()));
        _carrinhoRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);
        _estoqueServiceMock.Setup(x => x.VerificarEstoque(addItemDto.ProdutoId, addItemDto.Quantidade)).ReturnsAsync(true);

        // Act
        var resultado = await _adicionarItemCarrinhoUseCase.AdicionarItemCarrinho(addItemDto, carrinhoSessao);

        // Assert
        _carrinhoRepositoryMock.Verify(x => x.AdicionarItem(It.IsAny<Item>()), Times.Once);
        _carrinhoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
