using AutoFixture.AutoMoq;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using EF.Estoques.Domain.Repository;
using EF.Estoques.Infra.Data.Repository;
using FluentAssertions;
using EF.Estoques.Domain.Models;

namespace EF.Estoques.Infra.Test.Data.Repository;

public class EstoqueRepositoryTest : IDisposable
{
    private readonly EstoqueDbContext _context;
    private readonly IFixture _fixture;
    private bool disposed = false;

    public EstoqueRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDb")
            .Options;

        _context = new EstoqueDbContext(options);

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Register<IEstoqueRepository>(() => new EstoqueRepository(_context));
    }

    [Fact]
    public async Task DeveBuscarEstoquePorProdutoId()
    {
        // Arrange
        var estoque = _fixture.Create<Estoque>();
        var repository = _fixture.Create<IEstoqueRepository>();
        await repository.Salvar(estoque);
        await _context.Commit();

        // Act
        var result = await repository.Buscar(estoque.ProdutoId);

        // Assert
        result.Should().BeEquivalentTo(estoque);
    }

    [Fact]
    public async Task DeveSalvarNovoEstoque()
    {
        // Arrange
        var estoque = _fixture.Create<Estoque>();
        var movEstoque = _fixture.CreateMany<MovimentacaoEstoque>(5).ToList();
        movEstoque.ForEach(estoque.AdicionarMovimentacao);
        var repository = _fixture.Create<IEstoqueRepository>();

        // Act
        await repository.Salvar(estoque);
        bool commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.Estoques.Should().Contain(estoque);
        var estoqueSalvo = await _context.Estoques.FindAsync(estoque.Id);
        estoqueSalvo.Should().NotBeNull();
        estoqueSalvo.Should().BeEquivalentTo(estoque);
        estoqueSalvo!.Movimentacoes.Should().HaveCount(5);
        estoqueSalvo!.Movimentacoes.First().Estoque.Should().NotBeNull();
        repository.UnitOfWork.Should().Be(_context);
    }

    [Fact]
    public async Task DeveSalvarEstoqueExistente()
    {
        // Arrange
        var estoque = _fixture.Create<Estoque>();
        var movEstoque = _fixture.CreateMany<MovimentacaoEstoque>(5).ToList();
        movEstoque.ForEach(estoque.AdicionarMovimentacao);
        var repository = _fixture.Create<IEstoqueRepository>();
        await repository.Salvar(estoque);
        await _context.Commit();
        _context.Entry(estoque).State = EntityState.Detached;

        // Act
        var estoqueAtualizar = await _context.Estoques.FindAsync(estoque.Id);
        var movEstoqueAdiciopnar = _fixture.CreateMany<MovimentacaoEstoque>(3).ToList();
        movEstoqueAdiciopnar.ForEach(estoqueAtualizar!.AdicionarMovimentacao);
        estoqueAtualizar.AtualizarQuantidade(10, TipoMovimentacaoEstoque.Entrada);
        await repository.Salvar(estoqueAtualizar);
        bool commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.Entry(estoqueAtualizar).State = EntityState.Detached;
        _context.Estoques.Should().Contain(estoqueAtualizar);
        var estoqueSalvo = await _context.Estoques.Include(x => x.Movimentacoes).FirstOrDefaultAsync();
        estoqueSalvo!.Quantidade.Should().Be(estoqueAtualizar.Quantidade);
        estoqueSalvo!.Movimentacoes.Should().HaveCount(8);
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
