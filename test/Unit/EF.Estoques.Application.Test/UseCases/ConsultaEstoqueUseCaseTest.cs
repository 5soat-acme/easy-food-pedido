using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Estoques.Application.UseCases.Interfaces;
using EF.Estoques.Application.UseCases;
using EF.Estoques.Domain.Repository;
using Moq;
using EF.Estoques.Domain.Models;
using FluentAssertions;
using EF.Estoques.Application.DTOs.Responses;

namespace EF.Estoques.Application.Test.UseCases;

public class ConsultaEstoqueUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IEstoqueRepository> _estoqueRepository;
    private readonly IConsultaEstoqueUseCase _consultaEstoqueUseCase;

    public ConsultaEstoqueUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _estoqueRepository = _fixture.Freeze<Mock<IEstoqueRepository>>();
        _consultaEstoqueUseCase = _fixture.Create<ConsultaEstoqueUseCase>();
    }

    [Fact]
    public async Task DeveObterEstoqueProduto()
    {
        // Arrange
        var estoque = _fixture.Create<Estoque>();
        estoque.AdicionarMovimentacao(_fixture.Create<MovimentacaoEstoque>());

        var estoqueueDtoEsperado = new EstoqueDto()
        {
            ProdutoId = estoque.ProdutoId,
            Quantidade = estoque.Quantidade
        };

        _estoqueRepository.Setup(x => x.Buscar(estoque.ProdutoId, It.IsAny<CancellationToken>())).ReturnsAsync(estoque);

        // Act
        var resultado = await _consultaEstoqueUseCase.ObterEstoqueProduto(estoque.ProdutoId, It.IsAny<CancellationToken>());

        // Assert
        _estoqueRepository.Verify(x => x.Buscar(estoque.ProdutoId, It.IsAny<CancellationToken>()), Times.Once);
        resultado.Should().BeEquivalentTo(estoqueueDtoEsperado);
    }

    [Fact]
    public async Task DeveValidarEstoqueProdutoSucesso()
    {
        // Arrange
        var estoque = _fixture.Create<Estoque>();
        estoque.AtualizarQuantidade(10, TipoMovimentacaoEstoque.Entrada);
        estoque.AdicionarMovimentacao(_fixture.Create<MovimentacaoEstoque>());


        _estoqueRepository.Setup(x => x.Buscar(estoque.ProdutoId, It.IsAny<CancellationToken>())).ReturnsAsync(estoque);

        // Act
        var resultado = await _consultaEstoqueUseCase.ValidarEstoque(estoque.ProdutoId, 5, It.IsAny<CancellationToken>());

        // Assert
        _estoqueRepository.Verify(x => x.Buscar(estoque.ProdutoId, It.IsAny<CancellationToken>()), Times.Once);
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task DeveValidarEstoqueProdutoErro()
    {
        // Arrange
        var estoque = _fixture.Create<Estoque>();
        estoque.AtualizarQuantidade(10, TipoMovimentacaoEstoque.Entrada);
        estoque.AdicionarMovimentacao(_fixture.Create<MovimentacaoEstoque>());


        _estoqueRepository.Setup(x => x.Buscar(estoque.ProdutoId, It.IsAny<CancellationToken>())).ReturnsAsync(estoque);

        // Act
        var resultado = await _consultaEstoqueUseCase.ValidarEstoque(estoque.ProdutoId, estoque.Quantidade + 1, It.IsAny<CancellationToken>());

        // Assert
        _estoqueRepository.Verify(x => x.Buscar(estoque.ProdutoId, It.IsAny<CancellationToken>()), Times.Once);
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task DeveValidarEstoqueInexistente()
    {
        // Arrange
        var estoqueId = _fixture.Create<Guid>();

        _estoqueRepository.Setup(x => x.Buscar(estoqueId, It.IsAny<CancellationToken>())).ReturnsAsync((Estoque?)null);

        // Act
        var resultado = await _consultaEstoqueUseCase.ValidarEstoque(estoqueId, 1, It.IsAny<CancellationToken>());

        // Assert
        _estoqueRepository.Verify(x => x.Buscar(estoqueId, It.IsAny<CancellationToken>()), Times.Once);
        resultado.Should().BeFalse();
    }
}