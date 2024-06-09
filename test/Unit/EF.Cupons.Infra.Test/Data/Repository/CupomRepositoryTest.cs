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
    public async Task DeveBuscarCupom()
    { 
        // Arrange
        var cupom = _fixture.Build<Cupom>()
                             .FromFactory(() => new Cupom(
                                    dataInicio: DateTime.Now.AddDays(5),
                                    dataFim: DateTime.Now.AddDays(10),
                                    codigoCupom: _fixture.Create<string>().PadRight(4, 'a'),
                                    porcentagemDesconto: 0.59M,
                                    status: CupomStatus.Ativo))
                             .Create();
        var repository = _fixture.Create<ICupomRepository>();
        await repository.Criar(cupom);
        await _context.Commit();

        // Act
        var result = await repository.Buscar(cupom.Id);

        // Assert
        result.Should().BeEquivalentTo(cupom);
    }

    [Fact]
    public async Task DeveBuscarCupomVigente()
    {
        // Arrange
        var cupom = _fixture.Build<Cupom>()
                             .FromFactory(() => new Cupom(
                                    dataInicio: DateTime.Now,
                                    dataFim: DateTime.Now.AddDays(10),
                                    codigoCupom: _fixture.Create<string>().PadRight(4, 'a'),
                                    porcentagemDesconto: 0.59M,
                                    status: CupomStatus.Ativo))
                             .Create();
        var cupomProduto = _fixture.Create<CupomProduto>();
        cupom.AdicionarProduto(cupomProduto);
        var repository = _fixture.Create<ICupomRepository>();
        await repository.Criar(cupom);
        await _context.Commit();

        // Act
        var result = await repository.BuscarCupomVigente(cupom.CodigoCupom);

        // Assert
        result.Should().BeEquivalentTo(cupom);
    }

    [Fact]
    public async Task DeveBuscarCupomVigenteEmPeriodo()
    {
        // Arrange
        var cupom = _fixture.Build<Cupom>()
                             .FromFactory(() => new Cupom(
                                    dataInicio: DateTime.Now,
                                    dataFim: DateTime.Now.AddDays(10),
                                    codigoCupom: _fixture.Create<string>().PadRight(4, 'a'),
                                    porcentagemDesconto: 0.59M,
                                    status: CupomStatus.Ativo))
                             .Create();
        var cupomProduto = _fixture.Create<CupomProduto>();
        cupom.AdicionarProduto(cupomProduto);
        var repository = _fixture.Create<ICupomRepository>();
        await repository.Criar(cupom);
        await _context.Commit();

        // Act
        var result = await repository.BuscarCupomVigenteEmPeriodo(cupom.CodigoCupom, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(2));

        // Assert
        result.Should().Contain(cupom);
    }

    [Fact]
    public async Task DeveBuscarCupomProduto()
    {
        // Arrange
        var cupom = _fixture.Create<Cupom>();
        var cupomProduto = _fixture.Create<CupomProduto>();
        cupom.AdicionarProduto(cupomProduto);
        var repository = _fixture.Create<ICupomRepository>();
        await repository.Criar(cupom);
        await _context.Commit();

        // Act
        var result = await repository.BuscarCupomProduto(cupom.Id, cupomProduto.ProdutoId);

        // Assert
        result.Should().BeEquivalentTo(cupomProduto);
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
        cupomSalvo!.CupomProdutos.First().Cupom.Should().NotBeNull();
        repository.UnitOfWork.Should().Be(_context);
    }

    [Fact]
    public async Task DeveAtualizarUmCupom()
    {
        // Arrange
        var cupom = _fixture.Create<Cupom>();
        var repository = _fixture.Create<ICupomRepository>();
        await repository.Criar(cupom);
        await _context.Commit();
        _context.Entry(cupom).State = EntityState.Detached;

        // Act
        var cupomAtualizar = await _context.Cupons.FindAsync(cupom.Id);
        cupomAtualizar!.AlterarCodigoCupom("codigo_teste");
        cupomAtualizar!.AlterarPorcentagemDesconto(0.52M);
        cupomAtualizar!.AlterarDatas(DateTime.Now.AddDays(5), DateTime.Now.AddDays(10));
        repository.Atualizar(cupomAtualizar!);
        bool commit = await _context.Commit();


        // Assert
        commit.Should().BeTrue();
        _context.Entry(cupomAtualizar).State = EntityState.Detached;
        _context.Cupons.Should().Contain(cupomAtualizar);
        var cupomSalvo = await _context.Cupons.FindAsync(cupomAtualizar.Id);
        cupomSalvo!.CodigoCupom.Should().Be(cupomAtualizar.CodigoCupom);
        cupomSalvo!.PorcentagemDesconto.Should().Be(cupomAtualizar.PorcentagemDesconto);
        cupomSalvo!.DataInicio.Should().Be(cupomAtualizar.DataInicio);
        cupomSalvo!.DataFim.Should().Be(cupomAtualizar.DataFim);
    }

    [Fact]
    public async Task DeveInserirProdutosCupom()
    {
        // Arrange
        var cupomProdutos = _fixture.CreateMany<CupomProduto>(5).ToList();
        var repository = _fixture.Create<ICupomRepository>();


        // Act
        await repository.InserirProdutos(cupomProdutos);
        bool commit = await _context.Commit();


        // Assert
        commit.Should().BeTrue();
        _context.CupomProdutos.Count().Should().Be(5);
    }

    [Fact]
    public async Task DeveRemoverProdutoCupom()
    {
        // Arrange
        var cupomProdutos = _fixture.CreateMany<CupomProduto>(5).ToList();
        var repository = _fixture.Create<ICupomRepository>();
        await repository.InserirProdutos(cupomProdutos);
        await _context.Commit();
        cupomProdutos.ForEach(x => _context.Entry(x).State = EntityState.Detached);

        // Act
        var cupomProdutosRemover = cupomProdutos.Take(2).ToList();
        repository.RemoverProdutos(cupomProdutosRemover);
        bool commit = await _context.Commit();

        // Assert
        commit.Should().BeTrue();
        _context.CupomProdutos.Count().Should().Be(3);
        cupomProdutosRemover.ForEach(x => _context.CupomProdutos.Should().NotContain(x));
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
