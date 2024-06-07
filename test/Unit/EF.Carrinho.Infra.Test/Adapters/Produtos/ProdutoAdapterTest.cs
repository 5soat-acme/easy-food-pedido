using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Carrinho.Domain.Models;
using EF.Carrinho.Infra.Adapters.Produtos;
using EF.Produtos.Application.DTOs.Responses;
using EF.Produtos.Application.UseCases.Interfaces;
using FluentAssertions;
using Moq;

namespace EF.Carrinho.Infra.Test.Adapters.Produtos;

public class ProdutoAdapterTest
{
    private readonly IFixture _fixture;

    public ProdutoAdapterTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task DeveBuscarItemPorProdutoId()
    {
        // Arrange
        var produtoDto = _fixture.Create<ProdutoDto>();
        var itemEsperado = new Item(produtoDto.Id, produtoDto.Nome, produtoDto.ValorUnitario, produtoDto.TempoPreparoEstimado);

        var consultarProdutoUseCaseMock = _fixture.Freeze<Mock<IConsultarProdutoUseCase>>();
        consultarProdutoUseCaseMock.Setup(x => x.BuscarPorId(produtoDto.Id)).ReturnsAsync(produtoDto);

        var adapter = _fixture.Create<ProdutoAdapter>();

        // Act
        var resultado = await adapter.ObterItemPorProdutoId(produtoDto.Id);

        // Assert
        resultado.Should().BeEquivalentTo(itemEsperado, options =>
            options.Excluding(x => x.Id)
                   .ComparingByMembers<Item>());
    }
}
