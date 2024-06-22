using EF.Core.Commons.DomainObjects;
using EF.Estoques.Domain.Models;
using EF.Estoques.Domain.Test.Fixtures;
using FluentAssertions;

namespace EF.Estoques.Domain.Test.Models;

[Collection(nameof(EstoqueCollection))]
public class MovimentacaoEstoqueTest(EstoqueFixture fixture)
{
    [Fact]
    public void DeveCriarUmaInstanciaDeMovimentacaoEstoque()
    {
        // Arrange
        var movimentacao = fixture.GerarMovimentacao(Guid.NewGuid());

        // Act - Assert
        movimentacao.Should().BeOfType<MovimentacaoEstoque>();
    }

    [Fact]
    public void DeveGerarExcecao_QuandoCriarMovimentacaoEstoqueInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarMovimentacao(Guid.Empty);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Uma movimentação deve estar associada a um estoque");
    }

    [Fact]
    public void DeveGerarExcecao_QuandoCriarMovimentacaoComTipoMovimentacaoInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarMovimentacao(Guid.NewGuid(), (TipoMovimentacaoEstoque)999);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("TipoMovimentacao inválido");
    }

    [Fact]
    public void DeveGerarExcecao_QuandoCriarMovimentacaoComOrigemMovimentacaoInvalida()
    {
        // Arrange - Act
        Action act = () =>
            fixture.GerarMovimentacao(Guid.NewGuid(), origemMovimentacao: (OrigemMovimentacaoEstoque)999);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("OrigemMovimentacao inválida");
    }

    [Theory]
    [InlineData(TipoMovimentacaoEstoque.Entrada, OrigemMovimentacaoEstoque.Venda)]
    [InlineData(TipoMovimentacaoEstoque.Saida, OrigemMovimentacaoEstoque.Compra)]
    public void DeveGerarExcecao_QuandoCriarMovimentacaoComOrigemMovimentacaoIncompativelComTipoMovimentacao(
        TipoMovimentacaoEstoque tipoMovimentacao,
        OrigemMovimentacaoEstoque origemMovimentacao)
    {
        // Arrange - Act
        Action act = () => fixture.GerarMovimentacao(Guid.NewGuid(), tipoMovimentacao, origemMovimentacao);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("OrigemMovimentacao incompatível com TipoMovimentacao");
    }
}