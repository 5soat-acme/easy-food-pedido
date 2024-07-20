using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Identidade.Domain.Models;
using EF.Identidade.Domain.Repository;
using EF.Identidade.Infra.Data.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EF.Identidade.Infra.Test.Data.Repository;

public class SolicitacaoExclusaoRepositoryTest : IDisposable
{
    private readonly IdentidadeDbContext _context;
    private readonly IFixture _fixture;
    private bool disposed = false;

    public SolicitacaoExclusaoRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<IdentidadeDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDb")
            .Options;

        _context = new IdentidadeDbContext(options);

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Register<ISolicitacaoExclusaoRepository>(() => new SolicitacaoExclusaoRepository(_context));
    }

    [Fact]
    public async Task DeveCriarUmaSolicitacaoExclusao()
    {
        // Arrange
        var solicitacao = _fixture.Create<SolicitacaoExclusao>();
        var repository = _fixture.Create<ISolicitacaoExclusaoRepository>();

        // Act
        repository.Criar(solicitacao);
        bool commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.SolicitacaoExclusao.Should().Contain(solicitacao);
        var produtoSalvo = await _context.SolicitacaoExclusao.FindAsync(solicitacao.Id);
        produtoSalvo.Should().NotBeNull();
        produtoSalvo.Should().BeEquivalentTo(solicitacao);
        repository.UnitOfWork.Should().Be(_context);
    }

    

    [Fact]
    public async Task DeveBuscarPorClienteId()
    {
        // Arrange
        var solicitacao = _fixture.Create<SolicitacaoExclusao>();
        var repository = _fixture.Create<ISolicitacaoExclusaoRepository>();
        repository.Criar(solicitacao);
        await _context.Commit();

        // Act
        var result = await repository.BuscarPorClienteId(solicitacao.ClienteId);

        // Assert
        result.Should().BeEquivalentTo(solicitacao);
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