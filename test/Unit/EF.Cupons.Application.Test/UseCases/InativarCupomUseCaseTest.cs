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

public class InativarCupomUseCaseTest
{
    private readonly IFixture _fixture;

    public InativarCupomUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize(new CupomFixtureCustom());

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        _fixture.Register<IInativarCupomUseCase>(() => new InativarCupomUseCase(cupomRepositoryMock.Object));
    }

    [Fact]
    public async Task DeveInativarCupom()
    {
        // Arrange
        var cupom = _fixture.Create<Cupom>();

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Atualizar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>())).Returns(It.IsAny<Cupom>());
        cupomRepositoryMock.Setup(x => x.Buscar(cupom.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cupom);
        cupomRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        var useCase = _fixture.Create<IInativarCupomUseCase>();

        // Act
        var resultado = await useCase.Handle(cupom.Id);

        // Assert
        cupomRepositoryMock.Verify(x => x.Atualizar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>()), Times.Once);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
        cupom.Status.Should().Be(CupomStatus.Inativo);
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoInativarCupomInexistente()
    {
        // Arrange
        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Buscar(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cupom?)null);

        var useCase = _fixture.Create<IInativarCupomUseCase>();

        // Act
        var resultado = await useCase.Handle(_fixture.Create<Guid>());

        // Assert
        cupomRepositoryMock.Verify(x => x.Atualizar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>()), Times.Never);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var cupom = _fixture.Create<Cupom>();

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Atualizar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>())).Returns(It.IsAny<Cupom>());
        cupomRepositoryMock.Setup(x => x.Buscar(cupom.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cupom);
        cupomRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        var useCase = _fixture.Create<IInativarCupomUseCase>();

        // Act
        var resultado = await useCase.Handle(cupom.Id);

        // Assert
        cupomRepositoryMock.Verify(x => x.Atualizar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>()), Times.Once);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
