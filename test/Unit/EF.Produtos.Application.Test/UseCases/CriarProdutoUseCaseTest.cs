using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Produtos.Application.DTOs.Requests;
using EF.Produtos.Application.UseCases;
using EF.Produtos.Application.UseCases.Interfaces;
using EF.Produtos.Domain.Models;
using EF.Produtos.Domain.Repository;
using FluentAssertions;
using Moq;

namespace EF.Produtos.Application.Test.UseCases;

public class CriarProdutoUseCaseTest
{
    private readonly IFixture _fixture;

    public CriarProdutoUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        _fixture.Register<ICriarProdutoUseCase>(() => new CriarProdutoUseCase(produtoRepositoryMock.Object));
    }

    [Fact]
    public async Task DeveCriarProduto()
    {
        // Arrange
        var produtoDto = _fixture.Create<CriarProdutoDto>();

        var produtoRepositoryMock = _fixture.Freeze<Mock<IProdutoRepository>>();
        produtoRepositoryMock.Setup(x => x.Criar(It.IsAny<Produto>()));

        var useCase = _fixture.Create<ICriarProdutoUseCase>();

        // Act
        var resultado = await useCase.Handle(produtoDto);

        // Assert
        produtoRepositoryMock.Verify(x => x.Criar(It.IsAny<Produto>()), Times.Once);
        produtoRepositoryMock.Verify(x => x.UnitOfWork.Commit(), Times.Once);
        resultado.IsValid.Should().BeTrue();
        resultado.ValidationResult.IsValid.Should().BeTrue();
    }
}