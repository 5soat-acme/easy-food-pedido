using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Infra.Commons.EventBus;
using EF.Pedidos.Domain.Models;
using EF.Pedidos.Domain.Repository;
using EF.Pedidos.Infra.Data;
using EF.Pedidos.Infra.Data.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EF.Pedidos.Infra.Test.Data.Repository;

public class PedidoRepositoryTest : IDisposable
{
    private readonly PedidoDbContext _context;
    private readonly IFixture _fixture;
    private bool disposed = false;

    public PedidoRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<PedidoDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDb")
            .Options;

        _context = new PedidoDbContext(options, Mock.Of<IEventBus>());

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Register<IPedidoRepository>(() => new PedidoRepository(_context));
    }

    [Fact]
    public async Task DeveBuscarPedidoPorId()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        pedido.AdicionarItem(_fixture.Create<Item>());
        var repository = _fixture.Create<IPedidoRepository>();
        repository.Criar(pedido);
        await _context.Commit();

        // Act
        var result = await repository.ObterPorId(pedido.Id);

        // Assert
        result.Should().BeEquivalentTo(pedido);
    }

    [Fact]
    public async Task DeveCriarPedido()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        pedido.AdicionarItem(_fixture.Create<Item>());
        var repository = _fixture.Create<IPedidoRepository>();


        // Act
        repository.Criar(pedido);
        var commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.Pedidos.Should().Contain(pedido);
        var pedidoSalvo = await _context.Pedidos!.FindAsync(pedido.Id);
        pedidoSalvo.Should().NotBeNull();
        pedidoSalvo.Should().BeEquivalentTo(pedido);
        pedidoSalvo!.Itens.Should().HaveCount(1);
    }

    [Fact]
    public async Task DeveAtualizarPedido()
    {
        // Arrange
        var pedido = _fixture.Create<Pedido>();
        pedido.AdicionarItem(_fixture.Create<Item>());
        var repository = _fixture.Create<IPedidoRepository>();
        repository.Criar(pedido);
        await _context.Commit();
        _context.Entry(pedido).State = EntityState.Detached;


        // Act
        var pedidoAtualizar = await repository.ObterPorId(pedido.Id);
        var item = _fixture.Create<Item>();
        pedidoAtualizar!.AdicionarItem(item);
        pedidoAtualizar.CalcularValorTotal();
        _context.Entry(pedidoAtualizar).State = EntityState.Modified;
        repository.Atualizar(pedidoAtualizar!);
        bool commit = await _context.Commit();


        // Assert
        commit.Should().BeTrue();
        _context.Entry(pedidoAtualizar).State = EntityState.Detached;
        _context.Pedidos.Should().Contain(pedidoAtualizar);
        var pedidoSalvo = await _context.Pedidos!.FindAsync(pedidoAtualizar.Id);
        pedidoSalvo!.ValorTotal.Should().Be(pedidoAtualizar.ValorTotal);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Database.EnsureDeleted();
                _context.Dispose();
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
