using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Cupons.Application.UseCases.Interfaces;
using Moq;
using EF.Pedidos.Infra.Adapters.Cupons;
using FluentAssertions;
using EF.Pedidos.Application.DTOs.Gateways;
using EF.Cupons.Domain.Models;

namespace EF.Pedidos.Infra.Test.Adapters.Cupons;

public class CupomAdapterTest
{
    private readonly IFixture _fixture;

    public CupomAdapterTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task DeveBuscarCupomPorCodigo()
    {
        // Arrange
        var codigoCupom = "cupom1";
        var cupomDto = _fixture.Create<EF.Cupons.Application.DTOs.Responses.CupomDto>();
        var cupomEsperado = new CupomDto()
        {
            Id = cupomDto.Id,
            PorcentagemDesconto = cupomDto.PorcentagemDesconto,
            Produtos = cupomDto.Produtos.Select(x => new CupomDto.CupomProdutoDto()
            {
                ProdutoId = x.ProdutoId,
            }).ToList()
        };

        var consultarCupomUseCase = _fixture.Freeze<Mock<IConsultarCupomUseCase>>();
        consultarCupomUseCase.Setup(x => x.ObterCupom(codigoCupom, It.IsAny<CancellationToken>())).ReturnsAsync(cupomDto);

        var adapter = _fixture.Create<CupomAdapter>();

        // Act
        var resultado = await adapter.ObterCupomPorCodigo(codigoCupom);

        // Assert
        resultado.Should().BeEquivalentTo(cupomEsperado);
    }

    [Fact]
    public async Task DeveBuscarCupomNuloPorCodigo()
    {
        // Arrange
        var codigoCupom = "cupom1";
        var cupomDto = _fixture.Create<EF.Cupons.Application.DTOs.Responses.CupomDto>();

        var consultarCupomUseCase = _fixture.Freeze<Mock<IConsultarCupomUseCase>>();
        consultarCupomUseCase.Setup(x => x.ObterCupom(codigoCupom, It.IsAny<CancellationToken>())).ReturnsAsync((EF.Cupons.Application.DTOs.Responses.CupomDto?)null);


        var adapter = _fixture.Create<CupomAdapter>();

        // Act
        var resultado = await adapter.ObterCupomPorCodigo(codigoCupom);

        // Assert
        resultado.Should().BeNull();
    }
}
