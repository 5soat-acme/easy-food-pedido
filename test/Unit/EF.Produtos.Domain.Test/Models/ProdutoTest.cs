using EF.Core.Commons.DomainObjects;
using EF.Produtos.Domain.Models;
using EF.Produtos.Domain.Test.Fixtures;
using FluentAssertions;

namespace EF.Produtos.Domain.Test.Models;

[Collection(nameof(ProdutoCollection))]
public class ProdutoTest(ProdutoFixture fixture)
{
    [Fact(DisplayName = "Novo produto válido")]
    [Trait("Category", "Domain - Produto")]
    public void DeveCriarUmaInstanciaDeProduto()
    {
        // Arrange
        var produto = fixture.GerarProduto();

        // Act - Assert
        produto.Should().BeOfType<Produto>();
    }

    [Fact(DisplayName = "Novo produto com nome inválido")]
    [Trait("Category", "Domain - Produto")]
    public void DeveGerarExcecao_QuandoCriarProdutoComNomeInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarProduto(nome: "");

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Nome inválido");
    }

    [Fact(DisplayName = "Novo produto com valor unitário inválido")]
    [Trait("Category", "Domain - Produto")]
    public void DeveGerarExcecao_QuandoCriarProdutoComValorUnitarioInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarProduto(valorUnitario: 0);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Valor unitário inválido");
    }

    [Fact(DisplayName = "Novo produto com categoria inválida")]
    [Trait("Category", "Domain - Produto")]
    public void DeveGerarExcecao_QuandoCriarProdutoComCategoriaInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarProduto(categoria: (ProdutoCategoria) 999);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Categoria inválida");
    }

    [Fact(DisplayName = "Novo produto com descricao inválida")]
    [Trait("Category", "Domain - Produto")]
    public void DeveGerarExcecao_QuandoCriarProdutoComDescricaoInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarProduto(descricao: "");

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Descrição inválida");
    }

    [Fact(DisplayName = "Novo produto com tempo de preparo inválido")]
    [Trait("Category", "Domain - Produto")]
    public void DeveGerarExcecao_QuandoCriarProdutoComTempoDePreparoInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarProduto(tempoPreparoEstimado: 0);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("TempoPreparoEstimado inválido");
    }

    [Fact(DisplayName = "Ativar produto")]
    [Trait("Category", "Domain - Produto")]
    public void DeveAtivarProduto()
    {
        // Arrange
        var produto = fixture.GerarProduto();

        // Act
        produto.Ativar();

        // Assert
        produto.Ativo.Should().BeTrue();
    }

    [Fact(DisplayName = "Desativar produto")]
    [Trait("Category", "Domain - Produto")]
    public void DeveDesativarProduto()
    {
        // Arrange
        var produto = fixture.GerarProduto();

        // Act
        produto.Desativar();

        // Assert
        produto.Ativo.Should().BeFalse();
    }

    [Theory(DisplayName = "Alterar produto")]
    [Trait("Category", "Domain - Produto")]
    [InlineData(true)]
    [InlineData(false)]
    public void DeveAlterarProduto(bool ativo)
    {
        // Arrange
        var produto = fixture.GerarProduto();

        // Act
        var nome = "produto alterado";
        var valorUnitario = 15.52M; ;
        var categoria = ProdutoCategoria.Bebida;
        var descricao = "descrição alterada";
        var tempoPreparoEstimado = 13;
        produto.AlterarProduto(nome, valorUnitario, categoria, descricao, tempoPreparoEstimado, ativo);

        // Assert
        produto.Nome.Should().Be(nome);
        produto.ValorUnitario.Should().Be(valorUnitario);
        produto.Categoria.Should().Be(categoria);
        produto.Descricao.Should().Be(descricao);
        produto.TempoPreparoEstimado.Should().Be(tempoPreparoEstimado);
        produto.Ativo.Should().Be(ativo);
    }
}
