using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Cupons.Application.DTOs.Requests;
using EF.Cupons.Application.Test.Fixtures;
using EF.Cupons.Application.UseCases;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Repository;
using FluentAssertions;
using Moq;

namespace EF.Cupons.Application.Test.UseCases;

public class InserirProdutoUseCaseTest
{
    private readonly IFixture _fixture;

    public InserirProdutoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize(new CupomFixtureCustom());

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        _fixture.Register<IInserirProdutoUseCase>(() => new InserirProdutoUseCase(cupomRepositoryMock.Object));
    }

    [Fact]
    public async Task DeveInserirProdutosNoCupom()
    {
        // Arrange
        var cupom = _fixture.Create<Cupom>();
        var produtosDto = new InserirProdutoDto()
        {
            CupomId = cupom.Id,
            Produtos = _fixture.CreateMany<Guid>(5).ToList()
        };
        

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Buscar(cupom.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cupom);
        cupomRepositoryMock.Setup(x => x.BuscarCupomProduto(cupom.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((CupomProduto?)null);
        cupomRepositoryMock.Setup(x => x.InserirProdutos(It.IsAny<IList<CupomProduto>>(), It.IsAny<CancellationToken>()));
        cupomRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        var useCase = _fixture.Create<IInserirProdutoUseCase>();

        // Act
        var resultado = await useCase.Handle(produtosDto);

        // Assert
        cupomRepositoryMock.Verify(x => x.InserirProdutos(It.IsAny<IList<CupomProduto>>(), It.IsAny<CancellationToken>()), Times.Once);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoInserirProdutosEmCupomInexistente()
    {
        // Arrange
        var produtosDto = _fixture.Create<InserirProdutoDto>();

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Buscar(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cupom?)null);

        var useCase = _fixture.Create<IInserirProdutoUseCase>();

        // Act
        var resultado = await useCase.Handle(produtosDto);

        // Assert
        cupomRepositoryMock.Verify(x => x.InserirProdutos(It.IsAny<IList<CupomProduto>>(), It.IsAny<CancellationToken>()), Times.Never);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}