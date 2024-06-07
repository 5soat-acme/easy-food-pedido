using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Produtos.Domain.Models;
using EF.Produtos.Domain.Repository;
using EF.Produtos.Infra.Data;
using EF.Produtos.Infra.Data.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EF.Produtos.Infra.Test.Data.Repository;

public class ProdutoRepositoryTest : IDisposable
{
    private readonly ProdutoDbContext _context;
    private readonly IFixture _fixture;
    private bool disposed = false;

    public ProdutoRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ProdutoDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDb")
            .Options;

        _context = new ProdutoDbContext(options);

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Register<IProdutoRepository>(() => new ProdutoRepository(_context));
    }

    [Fact]
    public async Task DeveCriarUmProduto()
    {
        // Arrange
        var produto = _fixture.Create<Produto>();
        var repository = _fixture.Create<IProdutoRepository>();

        // Act
        repository.Criar(produto);
        bool commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.Produtos.Should().Contain(produto);
        var produtoSalvo = await _context.Produtos.FindAsync(produto.Id);
        produtoSalvo.Should().NotBeNull();
        produtoSalvo.Should().BeEquivalentTo(produto);
    }

    [Fact]
    public async Task DeveAtualizarUmProduto()
    {
        // Arrange
        var produto = _fixture.Create<Produto>();
        var repository = _fixture.Create<IProdutoRepository>();
        repository.Criar(produto);
        await _context.Commit();
        _context.Entry(produto).State = EntityState.Detached;

        // Act
        var produtoAtualizar = await _context.Produtos.FindAsync(produto.Id);
        produtoAtualizar!.AlterarProduto(
            nome: _fixture.Create<string>(),
            valorUnitario: _fixture.Create<decimal>(),
            categoria: _fixture.Create<ProdutoCategoria>(),
            descricao: _fixture.Create<string>(),
            tempoPreparoEstimado: _fixture.Create<int>(),
            ativo: true);
        repository.Atualizar(produtoAtualizar!);
        bool commit = await _context.Commit();


        // Assert
        commit.Should().BeTrue();
        _context.Entry(produtoAtualizar).State = EntityState.Detached;
        _context.Produtos.Should().Contain(produtoAtualizar);
        var produtoSalvo = await _context.Produtos.FindAsync(produtoAtualizar.Id);
        produtoSalvo!.Nome.Should().Be(produtoAtualizar.Nome);
        produtoSalvo!.ValorUnitario.Should().Be(produtoAtualizar.ValorUnitario);
        produtoSalvo!.Categoria.Should().Be(produtoAtualizar.Categoria);
        produtoSalvo!.Descricao.Should().Be(produtoAtualizar.Descricao);
        produtoSalvo!.TempoPreparoEstimado.Should().Be(produtoAtualizar.TempoPreparoEstimado);
        produtoSalvo!.Ativo.Should().Be(produtoAtualizar.Ativo);
    }

    [Fact]
    public async Task DeveBuscarProdutoPorId()
    {
        // Arrange
        var produto = _fixture.Create<Produto>();
        var repository = _fixture.Create<IProdutoRepository>();
        repository.Criar(produto);
        await _context.Commit();

        // Act
        var result = await repository.BuscarPorId(produto.Id);

        // Assert
        result.Should().BeEquivalentTo(produto);
    }

    [Fact]
    public async Task DeveBuscarProdutosPorCategoria()
    {
        // Arrange
        var produtoLanche = _fixture.Build<Produto>()
                             .FromFactory(() => new Produto(
                                 nome: _fixture.Create<string>(),
                                 valorUnitario: _fixture.Create<decimal>(),
                                 categoria: ProdutoCategoria.Lanche,
                                 tempoPreparoEstimado: _fixture.Create<int>(),
                                 descricao: _fixture.Create<string>()))
                             .Create();

        var produtoLanche2 = _fixture.Build<Produto>()
                             .FromFactory(() => new Produto(
                                 nome: _fixture.Create<string>(),
                                 valorUnitario: _fixture.Create<decimal>(),
                                 categoria: ProdutoCategoria.Lanche,
                                 tempoPreparoEstimado: _fixture.Create<int>(),
                                 descricao: _fixture.Create<string>()))
                             .Create();

        var produtoSobremesa = _fixture.Build<Produto>()
                             .FromFactory(() => new Produto(
                                 nome: _fixture.Create<string>(),
                                 valorUnitario: _fixture.Create<decimal>(),
                                 categoria: ProdutoCategoria.Sobremesa,
                                 tempoPreparoEstimado: _fixture.Create<int>(),
                                 descricao: _fixture.Create<string>()))
                             .Create();

        var repository = _fixture.Create<IProdutoRepository>();
        repository.Criar(produtoLanche);
        repository.Criar(produtoLanche2);
        repository.Criar(produtoSobremesa);

        await _context.Commit();

        // Act
        var result = await repository.Buscar(ProdutoCategoria.Lanche);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeveRemoverProduto()
    {
        // Arrange
        var produto = _fixture.Create<Produto>();
        var repository = _fixture.Create<IProdutoRepository>();
        repository.Criar(produto);
        await _context.Commit();
        _context.Entry(produto).State = EntityState.Detached;

        // Act
        var prodRemover = await repository.BuscarPorId(produto.Id);
        repository.Remover(prodRemover!);
        bool commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.Produtos.Should().HaveCount(0);
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