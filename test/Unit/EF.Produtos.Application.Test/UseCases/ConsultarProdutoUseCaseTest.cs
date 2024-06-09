using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Produtos.Application.DTOs.Responses;
using EF.Produtos.Application.UseCases;
using EF.Produtos.Application.UseCases.Interfaces;
using EF.Produtos.Domain.Models;
using EF.Produtos.Domain.Repository;
using FluentAssertions;
using Moq;
using System.Collections.Generic;

namespace EF.Produtos.Application.Test.UseCases;

public class ConsultarProdutoUseCaseTest
{
    private readonly IFixture _fixture;

    public ConsultarProdutoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        _fixture.Register<IConsultarProdutoUseCase>(() => new ConsultarProdutoUseCase(produtoRepositoryMock.Object));
    }

    [Fact]
    public async Task DeveConsultarProdutoPorId()
    {
        // Arrange
        var produto = _fixture.Create<Produto>();
        var produtoEsperado = new ProdutoDto()
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Categoria = produto.Categoria,
            ValorUnitario = produto.ValorUnitario,
            TempoPreparoEstimado = produto.TempoPreparoEstimado
        };

        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        produtoRepositoryMock.Setup(x => x.BuscarPorId(produto.Id)).ReturnsAsync(produto);

        var useCase = _fixture.Create<IConsultarProdutoUseCase>();

        // Act
        var resultado = await useCase.BuscarPorId(produto.Id);

        // Assert
        produtoRepositoryMock.Verify(x => x.BuscarPorId(produto.Id), Times.Once);
        resultado.Should().BeEquivalentTo(produtoEsperado);
    }

    [Fact]
    public async Task DeveRetornarNulo_QuandoConsultarProdutoQueNaoExistePorId()
    {
        // Arrange
        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        produtoRepositoryMock.Setup(x => x.BuscarPorId(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        var useCase = _fixture.Create<IConsultarProdutoUseCase>();

        // Act
        var resultado = await useCase.BuscarPorId(Guid.NewGuid());

        // Assert
        produtoRepositoryMock.Verify(x => x.BuscarPorId(It.IsAny<Guid>()), Times.Once);
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task DeveConsultarProdutoPorCategoria()
    {
        // Arrange
        var produto = _fixture.Create<Produto>();
        var listaProdutos = new List<Produto>() { produto };
        var listaProdutosEsperados = new List<ProdutoDto>() { new ProdutoDto()
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Categoria = produto.Categoria,
            ValorUnitario = produto.ValorUnitario,
            TempoPreparoEstimado = produto.TempoPreparoEstimado
        }};

        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        produtoRepositoryMock.Setup(x => x.Buscar(produto.Categoria)).ReturnsAsync(listaProdutos);

        var useCase = _fixture.Create<IConsultarProdutoUseCase>();

        // Act
        var resultado = await useCase.Buscar(produto.Categoria);

        // Assert
        produtoRepositoryMock.Verify(x => x.Buscar(produto.Categoria), Times.Once);
        resultado.Should().BeEquivalentTo(listaProdutosEsperados);
    }

    [Fact]
    public async Task DeveRetornarNulo_QuandoConsultarProdutoQueNaoExistePorCategoria()
    {
        // Arrange
        List<Produto>? produtos = null;
        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        produtoRepositoryMock.Setup(x => x.Buscar(It.IsAny<ProdutoCategoria>())).ReturnsAsync(produtos!);

        var useCase = _fixture.Create<IConsultarProdutoUseCase>();

        // Act
        var resultado = await useCase.Buscar(_fixture.Create<ProdutoCategoria>());

        // Assert
        produtoRepositoryMock.Verify(x => x.Buscar(It.IsAny<ProdutoCategoria>()), Times.Once);
        resultado.Should().BeNull();
    }
}
