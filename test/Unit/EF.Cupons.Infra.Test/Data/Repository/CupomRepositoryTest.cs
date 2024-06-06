using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Repository;
using EF.Cupons.Infra.Data.Repository;
using EF.Cupons.Infra.Test.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EF.Cupons.Infra.Test.Data.Repository;

public class CupomRepositoryTest : IDisposable
{
    private readonly CupomDbContext _context;
    private readonly IFixture _fixture;
    private bool disposed = false;

    public CupomRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<CupomDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDb")
            .Options;

        _context = new CupomDbContext(options);

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize(new CupomFixtureCustom());
        _fixture.Register<ICupomRepository>(() => new CupomRepository(_context));
    }

    [Fact]
    public async Task DeveCriarUmCupom()
    {
        // Arrange
        var cupom = _fixture.Create<Cupom>();
        var cupomProd = _fixture.Create<CupomProduto>();
        cupom.AdicionarProduto(cupomProd);
        var repository = _fixture.Create<ICupomRepository>();

        // Act
        await repository.Criar(cupom);
        bool commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.Cupons.Should().Contain(cupom);
        var cupomSalvo = await _context.Cupons.FindAsync(cupom.Id);
        cupomSalvo.Should().NotBeNull();
        cupomSalvo.Should().BeEquivalentTo(cupom);
        cupomSalvo!.CupomProdutos.Should().HaveCount(1);
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
