using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Estoques.Application.DTOs.Requests;
using EF.Estoques.Application.UseCases;
using EF.Estoques.Application.UseCases.Interfaces;
using EF.Estoques.Domain.Models;
using EF.Estoques.Domain.Repository;
using FluentAssertions;
using Moq;

namespace EF.Estoques.Application.Test.UseCases;

public class AtualizarEstoqueUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IEstoqueRepository> _estoqueRepository;
    private readonly IAtualizarEstoqueUseCase _atualizarEstoqueUseCase;

    public AtualizarEstoqueUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _estoqueRepository = _fixture.Freeze<Mock<IEstoqueRepository>>();
        _atualizarEstoqueUseCase = _fixture.Create<AtualizarEstoqueUseCase>();
    }

    [Fact]
    public async Task DeveAtualizarEstoque()
    {
        // Arrange
        var atualizarEstoqueDto = _fixture.Build<AtualizarEstoqueDto>()
            .With(x => x.TipoMovimentacao, TipoMovimentacaoEstoque.Entrada)
            .Create();


        _estoqueRepository.Setup(x => x.Buscar(atualizarEstoqueDto.ProdutoId, It.IsAny<CancellationToken>())).ReturnsAsync((Estoque?)null);
        _estoqueRepository.Setup(x => x.Salvar(It.IsAny<Estoque>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Estoque>());
        _estoqueRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(true);

        // Act
        var resultado = await _atualizarEstoqueUseCase.Handle(atualizarEstoqueDto);

        // Assert
        _estoqueRepository.Verify(x => x.Buscar(atualizarEstoqueDto.ProdutoId, It.IsAny<CancellationToken>()), Times.Once);
        _estoqueRepository.Verify(x => x.Salvar(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Once);
        _estoqueRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoCommitGerarErro()
    {
        // Arrange
        var atualizarEstoqueDto = _fixture.Build<AtualizarEstoqueDto>()
            .With(x => x.TipoMovimentacao, TipoMovimentacaoEstoque.Entrada)
            .Create();


        _estoqueRepository.Setup(x => x.Buscar(atualizarEstoqueDto.ProdutoId, It.IsAny<CancellationToken>())).ReturnsAsync((Estoque?)null);
        _estoqueRepository.Setup(x => x.Salvar(It.IsAny<Estoque>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Estoque>());
        _estoqueRepository.Setup(x => x.UnitOfWork.Commit()).ReturnsAsync(false);

        // Act
        var resultado = await _atualizarEstoqueUseCase.Handle(atualizarEstoqueDto);

        // Assert
        _estoqueRepository.Verify(x => x.Buscar(atualizarEstoqueDto.ProdutoId, It.IsAny<CancellationToken>()), Times.Once);
        _estoqueRepository.Verify(x => x.Salvar(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Once);
        _estoqueRepository.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeFalse();
        resultado.ValidationResult.IsValid.Should().BeFalse();
    }
}
