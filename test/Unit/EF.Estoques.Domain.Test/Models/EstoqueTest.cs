using EF.Core.Commons.DomainObjects;
using EF.Estoques.Domain.Models;
using EF.Estoques.Domain.Test.Fixtures;
using FluentAssertions;

namespace EF.Estoques.Domain.Test.Models;

[Collection(nameof(EstoqueCollection))]
public class EstoqueTest(EstoqueFixture fixture)
{
    [Fact(DisplayName = "Novo estoque válido")]
    [Trait("Category", "Domain - Estoque")]
    public void DeveCriarUmaInstanciaDeEstoque()
    {
        // Arrange
        var estoque = fixture.GerarEstoque();

        // Act - Assert
        estoque.Should().BeOfType<Estoque>();
    }

    [Fact(DisplayName = "Novo estoque inválido")]
    [Trait("Category", "Domain - Estoque")]
    public void DeveGerarExcecao_QuandoCriarEstoqueInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarEstoqueInvalido();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Produto inválido");
    }

    [Fact(DisplayName = "Novo estoque com quantidade inválida")]
    [Trait("Category", "Domain - Estoque")]
    public void DeveGerarExcecao_QuandoCriarEstoqueComQuantidadeInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarEstoque(qtdEstoque: -1);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Quantidade inválida");
    }

    [Fact(DisplayName = "Calcular nova quantidade do estoque após entrada")]
    [Trait("Category", "Domain - Estoque")]
    public void DeveCalcularQuantidadeAposEntrada()
    {
        // Arrange
        var qtdEstoque = 6;
        var qtdEntrada = 3;
        var estoque = fixture.GerarEstoque(qtdEstoque);
        qtdEstoque += qtdEntrada;

        // Act
        estoque.AtualizarQuantidade(qtdEntrada, TipoMovimentacaoEstoque.Entrada);

        // Assert
        estoque.Quantidade.Should().Be(qtdEstoque);
    }

    [Fact(DisplayName = "Calcular nova quantidade do estoque após saída")]
    [Trait("Category", "Domain - Estoque")]
    public void DeveCalcularQuantidadeAposSaida()
    {
        // Arrange
        var qtdEstoque = 6;
        var qtdSaida = 3;
        var estoque = fixture.GerarEstoque(qtdEstoque);
        qtdEstoque -= qtdSaida;

        // Act
        estoque.AtualizarQuantidade(qtdSaida, TipoMovimentacaoEstoque.Saida);

        // Assert
        estoque.Quantidade.Should().Be(qtdEstoque);
    }

    [Fact(DisplayName = "Atualizar quantidade inválida, estoque insuficiente")]
    [Trait("Category", "Domain - Estoque")]
    public void DeveGerarExcecao_QuandoAtualizarQuantidadeInvalidaPorMotivosDeEstoqueInsuficiente()
    {
        // Arrange
        var estoque = fixture.GerarEstoque(6);

        // Act
        Action act = () => estoque.AtualizarQuantidade(7, TipoMovimentacaoEstoque.Saida);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Produto não possui estoque suficiente");
    }

    [Fact(DisplayName = "Adicionar movimentação ao estoque")]
    [Trait("Category", "Domain - Estoque")]
    public void DeveAdicionarMovimentacaoEstoque()
    {
        // Arrange
        var estoque = fixture.GerarEstoque();
        var movimentacao = fixture.GerarMovimentacao(estoque.Id);

        // Act
        estoque.AdicionarMovimentacao(movimentacao);

        // Assert
        estoque.Movimentacoes.Should().Contain(movimentacao);
    }
}