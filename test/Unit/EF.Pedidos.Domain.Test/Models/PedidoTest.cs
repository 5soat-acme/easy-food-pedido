using EF.Core.Commons.DomainObjects;
using EF.Core.Commons.ValueObjects;
using EF.Pedidos.Domain.Models;
using EF.Pedidos.Domain.Test.Fixtures;
using FluentAssertions;

namespace EF.Pedidos.Domain.Test.Models;

[Collection(nameof(PedidoCollection))]
public class PedidoTest(PedidoFixture fixture)
{
    [Fact]
    public void DeveCriarPedidoAssociadoAoCliente()
    {
        // Arrange
        var pedido = fixture.GerarPedido();
        pedido.AssociarCliente(Guid.NewGuid());

        // Act & Assert 
        pedido.ClienteId.Should().NotBeEmpty();
    }

    [Fact]
    public void DeveGerarExcecao_QuandoAssociarClienteInvalidoAoPedido()
    {
        // Arrange
        var pedido = fixture.GerarPedido();

        // Act & Assert 
        Assert.Throws<DomainException>(() => pedido.AssociarCliente(Guid.Empty));
    }

    [Fact]
    public void DeveAssociarCupomAoPedido()
    {
        // Arrange
        var pedido = fixture.GerarPedido();

        // Act
        pedido.AssociarCupom(Guid.NewGuid());

        // Act & Assert 
        pedido.CupomId.Should().NotBeEmpty();
    }

    [Fact]
    public void DeveAssociarCPFAoPedido()
    {
        // Arrange
        var pedido = fixture.GerarPedido();
        var cpf = new Cpf("114.331.700-94");
        
        // Act
        pedido.AssociarCpf(cpf);

        // Act & Assert 
        pedido.Cpf.Should().Be(cpf);
    }

    [Fact]
    public void DeveAtualizarStatusDoPedido()
    {
        // Arrange
        var pedido = fixture.GerarPedido();
        var status = Status.Recebido;

        // Act
        pedido.AtualizarStatus(status);

        // Act & Assert 
        pedido.Status.Should().Be(status);
    }

    [Fact]
    public void DeveAdicionarItemAoPedido()
    {
        // Arrange
        var pedido = fixture.GerarPedido();
        var item = fixture.GerarItem();

        // Act
        pedido.AdicionarItem(item);

        // Assert 
        pedido.Itens.Should().Contain(item);
    }

    [Fact]
    public void DeveAtualizarValorTotal()
    {
        // Arrange
        var pedido = fixture.GerarPedido();
        var itens = fixture.GerarItens(10);
        decimal valorTotal = 0;
        foreach (var item in itens)
        {
            pedido.AdicionarItem(item);
            valorTotal += item.ValorUnitario * item.Quantidade;
        }

        // Act
        pedido.CalcularValorTotal();

        // Assert
        pedido.ValorTotal.Should().Be(valorTotal);
        pedido.ValorTotal.Should().Be(pedido.ValorTotal);
    }

    [Fact]
    public void DeveAplicarDesconto()
    {
        // Arrange
        var pedido = fixture.GerarPedido();
        var itens = fixture.GerarItens(10);
        decimal valorTotal = 0;
        decimal percentDesconto = 0.5M;
        foreach (var item in itens)
        {
            pedido.AdicionarItem(item);
            pedido.AplicarDescontoItem(item.Id, percentDesconto);
            valorTotal += (item.ValorUnitario - (item.ValorUnitario * percentDesconto)) * item.Quantidade;
        }

        // Act
        pedido.CalcularValorTotal();

        // Assert
        pedido.ValorTotal.Should().Be(valorTotal);
    }

    [Fact]
    public void DeveGerarExcecao_QuandoAplicarDescontoInvalido()
    {
        // Arrange
        var pedido = fixture.GerarPedido();
        var item = fixture.GerarItem();
        pedido.AdicionarItem(item);

        // Act & Assert 
        Assert.Throws<DomainException>(() => pedido.AplicarDescontoItem(item.Id, 0));
    }

    [Fact]
    public void DeveConfirmarPagamento()
    {
        // Arrange
        var pedido = fixture.GerarPedido();

        // Act
        pedido.ConfirmarPagamento();

        // Assert 
        pedido.Status.Should().Be(Status.Recebido);
    }

    [Fact]
    public void DeveCancelarPedido()
    {
        // Arrange
        var pedido = fixture.GerarPedido();

        // Act
        pedido.CancelarPedido();

        // Assert 
        pedido.Status.Should().Be(Status.Cancelado);
    }
}
