using EF.Carrinho.Domain.Test.Fixtures;
using EF.Core.Commons.DomainObjects;
using FluentAssertions;

namespace EF.Carrinho.Domain.Test.Models;

[Collection(nameof(CarrinhoCollection))]
public class CarrinhoClienteTest(CarrinhoClienteFixture fixture)
{
    [Fact]
    public void DeveCriarCarrinhoAssociadoAoCliente()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho();
        carrinhoCliente.AssociarCliente(Guid.NewGuid());

        // Act & Assert 
        carrinhoCliente.ClienteId.Should().NotBeEmpty();
    }

    [Fact]
    public void DeveGerarExcecao_QuandoAssociarClienteInvalidoAoCarrinho()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho();

        // Act & Assert 
        Assert.Throws<DomainException>(() => carrinhoCliente.AssociarCliente(Guid.Empty));
    }

    [Fact]
    public void DeveAssociarCarrinho()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho();

        // Act
        carrinhoCliente.AssociarCarrinho(Guid.NewGuid());

        //Assert
        carrinhoCliente.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void DeveGerarExcecao_QuandoAssociarCarrinhoInvalido()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho(); ;

        // Act & Assert 
        Assert.Throws<DomainException>(() => carrinhoCliente.AssociarCarrinho(Guid.Empty));
    }

    [Fact]
    public void DeveAdicionarItemAoCarrinho()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho();
        var item = fixture.GerarItem();

        // Act
        carrinhoCliente.AdicionarItem(item);

        // Assert 
        carrinhoCliente.Itens.Should().Contain(item);
    }

    [Fact]
    public void DeveAtualizarQuantidade()
    {
        // Arrange
        var qtdInicial = 2;
        var qtdFinal = 5;
        var carrinhoCliente = fixture.GerarCarrinho();
        var item = fixture.GerarItem();
        item.AtualizarQuantidade(qtdInicial);
        carrinhoCliente.AdicionarItem(item);        

        // Act
        carrinhoCliente.AtualizarQuantidadeItem(item.Id, qtdFinal);

        // Assert 
        carrinhoCliente.Itens.Should().ContainSingle(x => x.ProdutoId == item.ProdutoId);
        carrinhoCliente.Itens.FirstOrDefault(x => x.ProdutoId == item.ProdutoId)!.Quantidade.Should().Be(qtdFinal);
    }

    [Fact]
    public void DeveGerarExcecao_QuandoAtualizarQuantidadeDeitemInvalido()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho();
        var item = fixture.GerarItem();

        // Act
        Action act = () => carrinhoCliente.AtualizarQuantidadeItem(item.Id, 2);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Item não encontrado");
    }

    [Fact]
    public void DeveRemoverItemDoCarrinho()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho();
        var item = fixture.GerarItem();
        carrinhoCliente.AdicionarItem(item);

        // Act
        carrinhoCliente.RemoverItem(item);

        // Assert 
        carrinhoCliente.Itens.Should().NotContain(item);
        carrinhoCliente.ValorTotal.Should().Be(0);
    }

    [Fact]
    public void DeveAtualizarValorTotalDoCarrinho()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho();
        var itens = fixture.GerarItens(10);
        decimal valorTotal = 0;
        foreach (var item in itens)
        {
            carrinhoCliente.AdicionarItem(item);
            valorTotal += item.ValorUnitario * item.Quantidade;
        }

        // Act
        carrinhoCliente.CalcularValorTotal();

        // Assert
        carrinhoCliente.ValorTotal.Should().Be(valorTotal);
    }

    [Fact]
    public void DeveChecarSeProdutoExiste()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho();
        var produtoId = Guid.NewGuid();
        var item = fixture.GerarItem(produtoId: produtoId);
        carrinhoCliente.AdicionarItem(item);

        // Act
        var existe = carrinhoCliente.ProdutoExiste(produtoId);

        // Assert
        Assert.True(existe);
    }

    [Fact]
    public void DeveObterItemPorProdutoId()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho();
        var produtoId = Guid.NewGuid();
        var item = fixture.GerarItem(produtoId: produtoId);
        carrinhoCliente.AdicionarItem(item);

        // Act
        var itemObtido = carrinhoCliente.ObterItemPorProdutoId(produtoId);

        // Assert
        item.Should().Be(itemObtido);
    }

    [Fact]
    public void DeveObterItemPoId()
    {
        // Arrange
        var carrinhoCliente = fixture.GerarCarrinho();
        var item = fixture.GerarItem();
        carrinhoCliente.AdicionarItem(item);

        // Act
        var itemObtido = carrinhoCliente.ObterItemPorId(item.Id);

        // Assert
        item.Should().Be(itemObtido);
    }
}