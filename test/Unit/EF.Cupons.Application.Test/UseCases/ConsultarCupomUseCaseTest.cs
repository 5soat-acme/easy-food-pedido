using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Cupons.Application.DTOs.Responses;
using EF.Cupons.Application.Test.Fixtures;
using EF.Cupons.Application.UseCases;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Repository;
using FluentAssertions;
using Moq;

namespace EF.Cupons.Application.Test.UseCases;

public class ConsultarCupomUseCaseTest
{
    private readonly IFixture _fixture;

    public ConsultarCupomUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize(new CupomFixtureCustom());

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        _fixture.Register<IConsultarCupomUseCase>(() => new ConsultarCupomUseCase(cupomRepositoryMock.Object));
    }

    [Fact]
    public async Task DeveConsultarCupom()
    {
        // Arrange
        var cupom = _fixture.Create<Cupom>();
        var cupomEsperado = new CupomDto()
        {
            Id = cupom.Id,
            DataInicio = cupom.DataInicio,
            DataFim = cupom.DataFim,
            PorcentagemDesconto = cupom.PorcentagemDesconto,
            Produtos = new List<CupomProdutoDto>()
        };

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.BuscarCupomVigente(cupom.CodigoCupom, It.IsAny<CancellationToken>())).ReturnsAsync(cupom);

        var useCase = _fixture.Create<IConsultarCupomUseCase>();

        // Act
        var resultado = await useCase.ObterCupom(cupom.CodigoCupom, It.IsAny<CancellationToken>());

        // Assert
        cupomRepositoryMock.Verify(x => x.BuscarCupomVigente(cupom.CodigoCupom, It.IsAny<CancellationToken>()), Times.Once);
        resultado.Should().BeEquivalentTo(cupomEsperado);
    }
}
