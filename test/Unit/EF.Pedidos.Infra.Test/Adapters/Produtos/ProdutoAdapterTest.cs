using AutoFixture;
using AutoFixture.AutoMoq;
using EF.Pedidos.Infra.Adapters.Produtos;
using EF.Produtos.Application.DTOs.Responses;
using EF.Produtos.Application.UseCases.Interfaces;
using FluentAssertions;
using Moq;

namespace EF.Pedidos.Infra.Test.Adapters.Produtos;

public class ProdutoAdapterTest
{
    private readonly IFixture _fixture;

    public ProdutoAdapterTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task DeveObterProdutoPorId()
    {
        // Arrange
        var produtoDto = _fixture.Create<ProdutoDto>();
        var produtoEsperado = new EF.Pedidos.Application.DTOs.Gateways.ProdutoDto()
        {
            Id = produtoDto.Id,
            Nome = produtoDto.Nome,
            Descricao = produtoDto.Descricao,
            ValorUnitario = produtoDto.ValorUnitario,
            TempoPreparoEstimado = produtoDto.TempoPreparoEstimado
        };

        var consultarProdutoUseCaseMock = _fixture.Freeze<Mock<IConsultarProdutoUseCase>>();
        consultarProdutoUseCaseMock.Setup(x => x.BuscarPorId(produtoDto.Id)).ReturnsAsync(produtoDto);

        var adapter = _fixture.Create<ProdutoAdapter>();

        // Act
        var resultado = await adapter.ObterPorId(produtoDto.Id);

        // Assert
        resultado.Should().BeEquivalentTo(produtoEsperado);
    }

    [Fact]
    public async Task DeveObterProdutoNuloPorId()
    {
        // Arrange
        var produtoDto = _fixture.Create<EF.Produtos.Application.DTOs.Responses.ProdutoDto>();

        var consultarProdutoUseCaseMock = _fixture.Freeze<Mock<IConsultarProdutoUseCase>>();
        consultarProdutoUseCaseMock.Setup(x => x.BuscarPorId(produtoDto.Id)).ReturnsAsync((ProdutoDto?)null);

        var adapter = _fixture.Create<ProdutoAdapter>();

        // Act
        var resultado = await adapter.ObterPorId(produtoDto.Id);

        // Assert
        resultado.Should().BeNull();
    }
}
