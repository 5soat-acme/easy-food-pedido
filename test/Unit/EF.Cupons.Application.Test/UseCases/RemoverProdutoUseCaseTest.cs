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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Cupons.Application.Test.UseCases;

public class RemoverProdutoUseCaseTest
{
    private readonly IFixture _fixture;

    public RemoverProdutoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize(new CupomFixtureCustom());

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        _fixture.Register<IRemoverProdutoUseCase>(() => new RemoverProdutoUseCase(cupomRepositoryMock.Object));
    }

    [Fact]
    public async Task DeveRemoverProdutosNoCupom()
    {
        // Arrange
        var cupom = _fixture.Create<Cupom>();
        var cupomProduto = _fixture.Create<CupomProduto>();
        var produtosDto = new RemoverProdutoDto()
        {
            CupomId = cupom.Id,
            Produtos = _fixture.CreateMany<Guid>(5).ToList()
        };


        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Buscar(cupom.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cupom);
        cupomRepositoryMock.Setup(x => x.BuscarCupomProduto(cupom.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(cupomProduto);
        cupomRepositoryMock.Setup(x => x.RemoverProdutos(It.IsAny<IList<CupomProduto>>(), It.IsAny<CancellationToken>()));
        cupomRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        var useCase = _fixture.Create<IRemoverProdutoUseCase>();

        // Act
        var resultado = await useCase.Handle(produtosDto);

        // Assert
        cupomRepositoryMock.Verify(x => x.RemoverProdutos(It.IsAny<IList<CupomProduto>>(), It.IsAny<CancellationToken>()), Times.Once);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoRemoverProdutosEmCupomInexistente()
    {
        // Arrange
        var produtosDto = _fixture.Create<RemoverProdutoDto>();

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Buscar(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cupom?)null);

        var useCase = _fixture.Create<IRemoverProdutoUseCase>();

        // Act
        var resultado = await useCase.Handle(produtosDto);

        // Assert
        cupomRepositoryMock.Verify(x => x.RemoverProdutos(It.IsAny<IList<CupomProduto>>(), It.IsAny<CancellationToken>()), Times.Never);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
