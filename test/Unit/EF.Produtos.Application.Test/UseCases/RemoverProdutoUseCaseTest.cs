using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Produtos.Application.UseCases.Interfaces;
using EF.Produtos.Domain.Models;
using EF.Produtos.Domain.Repository;
using FluentAssertions;
using Moq;
using EF.Produtos.Application.UseCases;
using System.ComponentModel.DataAnnotations;

namespace EF.Produtos.Application.Test.UseCases;

public class RemoverProdutoUseCaseTest
{
    private readonly IFixture _fixture;

    public RemoverProdutoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        _fixture.Register<IRemoverProdutoUseCase>(() => new RemoverProdutoUseCase(produtoRepositoryMock.Object));
    }

    [Fact]
    public async Task DeveRemoverProduto()
    {
        // Arrange
        var produto = _fixture.Create<Produto>();

        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        produtoRepositoryMock.Setup(x => x.BuscarPorId(produto.Id)).ReturnsAsync(produto);

        var useCase = _fixture.Create<IRemoverProdutoUseCase>();

        // Act
        var resultado = await useCase.Handle(produto.Id);

        // Assert
        produtoRepositoryMock.Verify(x => x.BuscarPorId(produto.Id), Times.Once);
        produtoRepositoryMock.Verify(x => x.Remover(It.IsAny<Produto>()), Times.Once);
        produtoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task DeveGerarExcecao_QuandoRemoverProdutoInexistente()
    {
        // Arrange
        var produtoId = _fixture.Create<Guid>();

        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        produtoRepositoryMock.Setup(x => x.BuscarPorId(produtoId)).ReturnsAsync((Produto?)null);

        var useCase = _fixture.Create<IRemoverProdutoUseCase>();

        // Act
        Func<Task> act = async () => await useCase.Handle(produtoId);

        // Assert
        produtoRepositoryMock.Verify(x => x.BuscarPorId(produtoId), Times.Never);
        produtoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<Produto>()), Times.Never);
        produtoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Never);
        await act.Should().ThrowAsync<ValidationException>().WithMessage("Produto não existe");
    }
}
