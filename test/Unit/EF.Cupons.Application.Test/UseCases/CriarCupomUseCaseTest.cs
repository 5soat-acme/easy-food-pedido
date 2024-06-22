using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Cupons.Domain.Repository;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Cupons.Application.UseCases;
using EF.Cupons.Domain.Models;
using FluentAssertions;
using EF.Cupons.Application.Test.Fixtures;
using Moq;
using EF.Cupons.Application.DTOs.Requests;

namespace EF.Cupons.Application.Test.UseCases;

public class CriarCupomUseCaseTest
{
    private readonly IFixture _fixture;

    public CriarCupomUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize(new CriarCupomDtoFixtureCustom());
        _fixture.Customize(new CupomFixtureCustom());
        
        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        _fixture.Register<ICriarCupomUseCase>(() => new CriarCupomUseCase(cupomRepositoryMock.Object)); 
    }

    [Fact]
    public async Task DeveCriarUmCupom()
    {
        // Arrange
        var cupomDto = _fixture.Create<CriarCupomDto>();

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Criar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Cupom>());
        cupomRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        var useCase = _fixture.Create<ICriarCupomUseCase>();

        // Act
        var resultado = await useCase.Handle(cupomDto);

        // Assert
        cupomRepositoryMock.Verify(x => x.Criar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>()), Times.Once);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCriarCupomComOutroIgualJaVigente()
    {
        // Arrange
        var cupomDto = _fixture.Create<CriarCupomDto>();
        var cuponsRetorno = _fixture.CreateMany<Cupom>(1).ToList();

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Criar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Cupom>());
        cupomRepositoryMock.Setup(x => x.BuscarCupomVigenteEmPeriodo(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(cuponsRetorno);

        var useCase = _fixture.Create<ICriarCupomUseCase>();

        // Act
        var resultado = await useCase.Handle(cupomDto);

        // Assert
        cupomRepositoryMock.Verify(x => x.Criar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>()), Times.Never);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var cupomDto = _fixture.Create<CriarCupomDto>();

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Criar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Cupom>());
        cupomRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        var useCase = _fixture.Create<ICriarCupomUseCase>();

        // Act
        var resultado = await useCase.Handle(cupomDto);

        // Assert
        cupomRepositoryMock.Verify(x => x.Criar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>()), Times.Once);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
