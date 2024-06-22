using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Produtos.Application.DTOs.Requests;
using EF.Produtos.Application.UseCases.Interfaces;
using EF.Produtos.Domain.Repository;
using Moq;
using EF.Produtos.Application.UseCases;
using EF.Produtos.Domain.Models;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace EF.Produtos.Application.Test.UseCases;

public class AtualizarProdutoUseCaseTest
{
    private readonly IFixture _fixture;

    public AtualizarProdutoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        _fixture.Register<IAtualizarProdutoUseCase>(() => new AtualizarProdutoUseCase(produtoRepositoryMock.Object));
    }

    [Fact]
    public async Task DeveAtualizarProduto()
    {
        // Arrange
        var produtoDto = _fixture.Create<AtualizarProdutoDto>();
        var produto = _fixture.Create<Produto>();

        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        produtoRepositoryMock.Setup(x => x.BuscarPorId(produtoDto.ProdutoId)).ReturnsAsync(produto);

        var useCase = _fixture.Create<IAtualizarProdutoUseCase>();

        // Act
        var resultado = await useCase.Handle(produtoDto);

        // Assert
        produtoRepositoryMock.Verify(x => x.BuscarPorId(produtoDto.ProdutoId), Times.Once);
        produtoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<Produto>()), Times.Once);
        produtoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoAtualizarProdutoInexistente()
    {
        // Arrange
        var produtoDto = _fixture.Create<AtualizarProdutoDto>();

        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        produtoRepositoryMock.Setup(x => x.BuscarPorId(produtoDto.ProdutoId)).ReturnsAsync((Produto?)null);

        var useCase = _fixture.Create<IAtualizarProdutoUseCase>();

        // Act
        Func<Task> act = async () => await useCase.Handle(produtoDto);

        // Assert        
        produtoRepositoryMock.Verify(x => x.BuscarPorId(produtoDto.ProdutoId), Times.Never);
        produtoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<Produto>()), Times.Never);
        produtoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        await act.Should().ThrowAsync<ValidationException>().WithMessage("Produto não existe");
    }
}
