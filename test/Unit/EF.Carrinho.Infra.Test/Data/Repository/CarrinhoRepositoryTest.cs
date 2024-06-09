using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Carrinho.Domain.Models;
using Microsoft.EntityFrameworkCore;
using EF.Carrinho.Infra.Data;
using EF.Carrinho.Infra.Data.Repository;
using EF.Carrinho.Domain.Repository;
using FluentAssertions;

namespace EF.Carrinho.Infra.Test.Data.Repository;

public class CarrinhoRepositoryTest : IDisposable
{
    private readonly CarrinhoDbContext _context;
    private readonly IFixture _fixture;
    private bool disposed = false;

    public CarrinhoRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<CarrinhoDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDb")
            .Options;

        _context = new CarrinhoDbContext(options);

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Register<ICarrinhoRepository>(() => new CarrinhoRepository(_context));
    }

    [Fact]
    public async Task DeveBuscarCarrinhoPorClienteId()
    {
        // Arrange
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AssociarCarrinho(Guid.NewGuid());
        carrinho.AssociarCliente(Guid.NewGuid());
        carrinho.AdicionarItem(_fixture.Create<Item>());
        var repository = _fixture.Create<ICarrinhoRepository>();
        repository.Criar(carrinho);
        await _context.Commit();

        // Act
        var result = await repository.ObterPorClienteId(carrinho.ClienteId!.Value);

        // Assert
        result.Should().BeEquivalentTo(carrinho);
        repository.UnitOfWork.Should().Be(_context);
    }

    [Fact]
    public async Task DeveBuscarCarrinhoPorId()
    {
        // Arrange
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AssociarCarrinho(Guid.NewGuid());
        carrinho.AssociarCliente(Guid.NewGuid());
        carrinho.AdicionarItem(_fixture.Create<Item>());
        var repository = _fixture.Create<ICarrinhoRepository>();
        repository.Criar(carrinho);
        await _context.Commit();

        // Act
        var result = await repository.ObterPorId(carrinho.Id);

        // Assert
        result.Should().BeEquivalentTo(carrinho);
    }

    [Fact]
    public async Task DeveCriarCarrinho()
    {
        // Arrange
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AssociarCarrinho(Guid.NewGuid());
        carrinho.AssociarCliente(Guid.NewGuid());
        carrinho.AdicionarItem(_fixture.Create<Item>());
        var repository = _fixture.Create<ICarrinhoRepository>();


        // Act
        repository.Criar(carrinho);
        var commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.Carrinhos.Should().Contain(carrinho);
        var carrinhoSalvo = await _context.Carrinhos!.FindAsync(carrinho.Id);
        carrinhoSalvo.Should().NotBeNull();
        carrinhoSalvo.Should().BeEquivalentTo(carrinho);
        carrinhoSalvo!.Itens.Should().HaveCount(1);
        carrinhoSalvo!.DataCriacao.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public async Task DeveAtualizarCarrinho()
    {
        // Arrange
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AssociarCarrinho(Guid.NewGuid());
        carrinho.AssociarCliente(Guid.NewGuid());
        var repository = _fixture.Create<ICarrinhoRepository>();
        repository.Criar(carrinho);
        await _context.Commit();
        _context.Entry(carrinho).State = EntityState.Detached;


        // Act
        var carrinhoAtualizar = await repository.ObterPorId(carrinho.Id);
        var item = _fixture.Create<Item>();
        item.AtualizarQuantidade(1);
        carrinhoAtualizar!.AdicionarItem(item);
        carrinhoAtualizar.CalcularValorTotal();
        _context.Entry(carrinhoAtualizar).State = EntityState.Modified;
        repository.Atualizar(carrinhoAtualizar!);
        bool commit = await _context.Commit();


        // Assert
        commit.Should().BeTrue();
        _context.Entry(carrinhoAtualizar).State = EntityState.Detached;
        _context.Carrinhos.Should().Contain(carrinhoAtualizar);
        var carrinhoSalvo = await _context.Carrinhos!.FindAsync(carrinhoAtualizar.Id);
        carrinhoSalvo!.ValorTotal.Should().Be(carrinhoAtualizar.ValorTotal);
        carrinhoSalvo!.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public async Task DeveRemoverCarrinho()
    {
        // Arrange
        var carrinho = _fixture.Create<CarrinhoCliente>();
        carrinho.AssociarCarrinho(Guid.NewGuid());
        carrinho.AssociarCliente(Guid.NewGuid());
        carrinho.AdicionarItem(_fixture.Create<Item>());
        var repository = _fixture.Create<ICarrinhoRepository>();
        repository.Criar(carrinho);
        await _context.Commit();
        _context.Entry(carrinho).State = EntityState.Detached;


        // Act
        var carrinhoRemover = await repository.ObterPorId(carrinho.Id);
        repository.Remover(carrinhoRemover!);
        bool commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.Carrinhos.Should().HaveCount(0);
    }

    [Fact]
    public async Task DeveAdicionarItem()
    {
        // Arrange
        var item = _fixture.Create<Item>();
        var repository = _fixture.Create<ICarrinhoRepository>();


        // Act
        repository.AdicionarItem(item);
        bool commit = await _context.Commit();


        // Assert
        commit.Should().BeTrue();
        _context.Itens!.Should().Contain(item);
    }

    [Fact]
    public async Task DeveAtualziarItem()
    {
        // Arrange
        var item = _fixture.Create<Item>();
        var repository = _fixture.Create<ICarrinhoRepository>();
        repository.AdicionarItem(item);
        await _context.Commit();

        // Act
        item.AtualizarQuantidade(10);
        repository.AtualizarItem(item);
        bool commit = await _context.Commit();


        // Assert
        commit.Should().BeTrue();
        _context.Itens!.Should().Contain(item);
        var itemSalvo = await _context.Itens!.FindAsync(item.Id);
        itemSalvo!.Quantidade.Should().Be(item.Quantidade);
    }

    [Fact]
    public async Task DeveRemoverItem()
    {
        // Arrange
        var item = _fixture.Create<Item>();
        var repository = _fixture.Create<ICarrinhoRepository>();
        repository.AdicionarItem(item);
        await _context.Commit();
        _context.Entry(item).State = EntityState.Detached;

        // Act
        repository.RemoverItem(item);
        bool commit = await _context.Commit();


        // Assert
        commit.Should().BeTrue();
        _context.Itens!.Should().NotContain(item);
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
