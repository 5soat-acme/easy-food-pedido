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

public class AtualizarCupomUseCaseTest
{
    private readonly IFixture _fixture;

    public AtualizarCupomUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize(new AtualizarCupomDtoFixtureCustom());
        _fixture.Customize(new CupomFixtureCustom());

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        _fixture.Register<IAtualizarCupomUseCase>(() => new AtualizarCupomUseCase(cupomRepositoryMock.Object));
    }

    [Fact]
    public async Task DeveAtualizarCupom()
    {
        // Arrange
        var cupomDto = _fixture.Create<AtualizarCupomDto>();
        var cupom = _fixture.Create<Cupom>();

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Atualizar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>())).Returns(It.IsAny<Cupom>());
        cupomRepositoryMock.Setup(x => x.Buscar(cupomDto.CupomId, It.IsAny<CancellationToken>())).ReturnsAsync(cupom);
        cupomRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        var useCase = _fixture.Create<IAtualizarCupomUseCase>();

        // Act
        var resultado = await useCase.Handle(cupomDto);

        // Assert
        cupomRepositoryMock.Verify(x => x.Atualizar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>()), Times.Once);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoAtualizarCupomInexistente()
    {
        // Arrange
        var cupomDto = _fixture.Create<AtualizarCupomDto>();

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Buscar(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cupom?)null);

        var useCase = _fixture.Create<IAtualizarCupomUseCase>();

        // Act
        var resultado = await useCase.Handle(cupomDto);

        // Assert
        cupomRepositoryMock.Verify(x => x.Atualizar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>()), Times.Never);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoAtualizarCupomEmVigencia()
    {
        // Arrange
        var cupomDto = _fixture.Create<AtualizarCupomDto>();
        var cupom = new Cupom(dataInicio: DateTime.Now,
                        dataFim: DateTime.Now.AddDays(Math.Abs(_fixture.Create<int>()) + 1),
                        codigoCupom: _fixture.Create<string>().PadRight(4, 'a'),
                        porcentagemDesconto: 0.15M,
                        status: _fixture.Create<CupomStatus>());

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Buscar(cupomDto.CupomId, It.IsAny<CancellationToken>())).ReturnsAsync(cupom);

        var useCase = _fixture.Create<IAtualizarCupomUseCase>();

        // Act
        var resultado = await useCase.Handle(cupomDto);

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
        var cupomDto = _fixture.Create<AtualizarCupomDto>();
        var cupom = _fixture.Create<Cupom>();

        var cupomRepositoryMock = _fixture.Freeze<Mock<ICupomRepository>>();
        cupomRepositoryMock.Setup(x => x.Atualizar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>())).Returns(It.IsAny<Cupom>());
        cupomRepositoryMock.Setup(x => x.Buscar(cupomDto.CupomId, It.IsAny<CancellationToken>())).ReturnsAsync(cupom);
        cupomRepositoryMock.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        var useCase = _fixture.Create<IAtualizarCupomUseCase>();

        // Act
        var resultado = await useCase.Handle(cupomDto);

        // Assert
        cupomRepositoryMock.Verify(x => x.Atualizar(It.IsAny<Cupom>(), It.IsAny<CancellationToken>()), Times.Once);
        cupomRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
