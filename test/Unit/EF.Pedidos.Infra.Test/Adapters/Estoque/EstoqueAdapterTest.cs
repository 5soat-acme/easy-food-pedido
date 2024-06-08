using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Pedidos.Infra.Adapters.Cupons;
using Moq;
using EF.Estoques.Application.UseCases.Interfaces;
using EF.Pedidos.Infra.Adapters.Estoque;
using FluentAssertions;

namespace EF.Pedidos.Infra.Test.Adapters.Estoque;

public class EstoqueAdapterTest
{
    private readonly IFixture _fixture;

    public EstoqueAdapterTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeveVerificarEstoque(bool estoqueValido)
    {
        // Arrange
        var consultaEstoqueUseCase = _fixture.Freeze<Mock<IConsultaEstoqueUseCase>>();
        consultaEstoqueUseCase.Setup(x => x.ValidarEstoque(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(estoqueValido);
        var adapter = _fixture.Create<EstoqueAdapter>();

        // Act
        var resultado = await adapter.VerificarEstoque(_fixture.Create<Guid>(), _fixture.Create<int>());

        // Assert
        resultado.Should().Be(estoqueValido);
    }
}
