using EF.Core.Commons.DomainObjects;
using EF.Pedidos.Domain.Models;
using EF.Pedidos.Domain.Test.Fixtures;
using FluentAssertions;

namespace EF.Pedidos.Domain.Test.Models;

[Collection(nameof(PedidoCollection))]
public class ItemTest(PedidoFixture fixture)
{
    [Fact(DisplayName = "Novo item válido")]
    [Trait("Category", "Domain - Pedido")]
    public void DeveCriarItem()
    {
        // Arrange - Act
        var item = fixture.GerarItem();

        // Act - Assert
        item.Should().BeOfType<Item>();
    }

    [Fact(DisplayName = "Novo item com produto inválido")]
    [Trait("Category", "Domain - Pedido")]
    public void DeveGerarExcecao_QuandoCriarItemComProdutoInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarItem(produtoId: Guid.Empty);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Produto inválido");
    }

    [Fact(DisplayName = "Novo item com nome inválido")]
    [Trait("Category", "Domain - Pedido")]
    public void DeveGerarExcecao_QuandoCriarItemComNomeInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarItem(nome: "");

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Nome do item inválido");
    }

    [Fact(DisplayName = "Novo item com valor unitário inválido")]
    [Trait("Category", "Domain - Pedido")]
    public void DeveGerarExcecao_QuandoCriarItemComValorUnitarioInvalido()
    {
        // Arrange - Act
        Action act = () => fixture.GerarItem(valorUnitario: 0);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Valor unitário do item inválido");
    }

    [Fact(DisplayName = "Novo item com quantidade inválida")]
    [Trait("Category", "Domain - Pedido")]
    public void DeveGerarExcecao_QuandoCriarItemComQuantidadeInvalida()
    {
        // Arrange - Act
        Action act = () => fixture.GerarItem(quantidade: 0);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Quantidade do item inválida");
    }
}
